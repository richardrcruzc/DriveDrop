using DriveDrop.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static IdentityModel.OidcConstants;

namespace DriveDrop.Api.Services
{
    public class PayPalStandardPaymentProcessor: IPayPalStandardPaymentProcessor
    {
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly DriveDropContext _context;
        private const string BN_CODE = "DriveDrop_SP";
        public PayPalStandardPaymentProcessor(IOptionsSnapshot<AppSettings> settings, DriveDropContext context)
        {
            _settings= settings;
            _context = context;
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public async Task  PostProcessPayment(int shipmentId)
        {
            var urlToRedirect = await GenerationRedirectionUrl(shipmentId);
            if (urlToRedirect == null)
                throw new Exception("PayPal URL cannot be generated");
            //ensure URL doesn't exceed 2K chars. Otherwise, customers can get "too long URL" exception
            if (urlToRedirect.Length > 2048)
                throw new Exception("PayPal URL cannot be generated");

            // 'HTTP Basic Auth Post' <http://stackoverflow.com/questions/21066622/how-to-send-a-http-basic-auth-post>
            string clientId = _settings.Value.ClientId;
            string secret = _settings.Value.Secret;
            string oAuthCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(clientId + ":" + secret));

            //base uri to PayPAl 'live' or 'stage' based on 'productionMode'
            string uriString = GetPaypalUrl();

            HttpClient client = new HttpClient();

            //construct request message
            var h_request = new HttpRequestMessage(HttpMethod.Post, uriString);
            h_request.Headers.Authorization = new AuthenticationHeaderValue("Basic", oAuthCredentials);
            h_request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            h_request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("en_US"));

            h_request.Content = new StringContent("grant_type=client_credentials", UTF8Encoding.UTF8, "application/x-www-form-urlencoded");


            try
            {
                HttpResponseMessage response = await client.SendAsync(h_request);

                //if call failed ErrorResponse created...simple class with response properties
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    //ErrorResponse errResp = JsonConvert.DeserializeObject<ErrorResponse>(error);
                    //throw new PayPalException { error_name = errResp.name, details = errResp.details, message = errResp.message };

                }

                var success = await response.Content.ReadAsStringAsync();
                //result = JsonConvert.DeserializeObject<ResponseT>(success);
            }
            catch (Exception)
            {
                throw new HttpRequestException("Request to PayPal Service failed.");
            }


            HttpClientHandler handler = new HttpClientHandler();
            handler.AllowAutoRedirect = true;
            client = new HttpClient(handler);
            HttpResponseMessage response2 = await client.GetAsync(urlToRedirect);


        }

        private async Task<string> GenerationRedirectionUrl(int shipmentId)
        {
            var packages = await _context.Shipments
                .Include(p=>p.PickupAddress)
                .Include(d => d.DeliveryAddress)
                .Include(r => r.Driver)
                .Include(t => t.PriorityType)
                .Include(s => s.Sender)
                .Include("Sender.DefaultAddress")
                .Where(i => i.Id == shipmentId)
                .FirstOrDefaultAsync(); 

            var passProductNamesAndTotals = true;
            var builder = new StringBuilder();
            builder.Append(GetPaypalUrl());
            var cmd = passProductNamesAndTotals
                ? "_cart"
                : "_xclick";

            builder.AppendFormat("?cmd={0}&business={1}", cmd,  WebUtility.UrlEncode(_settings.Value.BusinessAccountKey));
 
                builder.AppendFormat("&upload=1");
                //get the items in the cart
                decimal cartTotal = decimal.Zero;
                var cartTotalRounded = decimal.Zero;
               
                int x = 1;
                 
                    var unitPriceExclTax = packages.ChargeAmount;
                    var priceExclTax = packages.ChargeAmount - packages.Tax;
                    //round
                    var unitPriceExclTaxRounded = Math.Round(unitPriceExclTax, 2);
                    builder.AppendFormat("&item_name_" + x + "={0}", WebUtility.UrlEncode("DropShipment Service fee"));
                    builder.AppendFormat("&amount_" + x + "={0}", unitPriceExclTaxRounded.ToString("0.00", CultureInfo.InvariantCulture));
                    builder.AppendFormat("&quantity_" + x + "={0}", packages.Quantity);
                    x++;
                    cartTotal += priceExclTax;
                    cartTotalRounded += unitPriceExclTaxRounded * packages.Quantity;
                 

            
            //order totals
            //tax
            var orderTax = packages.Tax;
            var orderTaxRounded = Math.Round(orderTax, 2);
            if (orderTax > decimal.Zero)
            {
                //builder.AppendFormat("&tax_1={0}", orderTax.ToString("0.00", CultureInfo.InvariantCulture));

                //add tax as item
                builder.AppendFormat("&item_name_1={0}", WebUtility.UrlEncode("Sales Tax")); //name
                builder.AppendFormat("&amount_1={0}", orderTaxRounded.ToString("0.00", CultureInfo.InvariantCulture)); //amount
                builder.AppendFormat("&quantity_1={0}", 1); //quantity
                 
            }

           
                /* Take the difference between what the order total is and what it should be and use that as the "discount".
                 * The difference equals the amount of the gift card and/or reward points used. 
                 */
                decimal discountTotal = packages.Discount;
                discountTotal = Math.Round(discountTotal, 2);
                 cartTotalRounded -= discountTotal;
                //gift card or rewared point amount applied to cart in nopCommerce - shows in Paypal as "discount"
                builder.AppendFormat("&discount_amount_cart={0}", discountTotal.ToString("0.00", CultureInfo.InvariantCulture));


            builder.AppendFormat("&custom={0}", packages.Sender.Id);
            builder.AppendFormat("&charset={0}", "utf-8");
            builder.AppendFormat("&bn={0}", BN_CODE);
            builder.Append(string.Format("&no_note=1&currency_code={0}", WebUtility.UrlEncode(_settings.Value.CurrencyCode)));
            builder.AppendFormat("&invoice={0}", packages.Id);
            builder.AppendFormat("&rm=2", new object[0]);
            
            builder.AppendFormat("&no_shipping=1", new object[0]);

            string returnUrl = _settings.Value.ReturnURL;
            string cancelReturnUrl =_settings.Value.CancelURL;
            builder.AppendFormat("&return={0}&cancel_return={1}", WebUtility.UrlEncode(returnUrl), WebUtility.UrlEncode(cancelReturnUrl));

            builder.AppendFormat("&notify_url={0}", _settings.Value.NotifyURL);

                //address
                builder.AppendFormat("&address_override={0}","1" );
                builder.AppendFormat("&first_name={0}", WebUtility.UrlEncode(packages.Sender.FirstName));
                builder.AppendFormat("&last_name={0}", WebUtility.UrlEncode(packages.Sender.FirstName));
                builder.AppendFormat("&address1={0}", WebUtility.UrlEncode(packages.Sender.DefaultAddress.Street));
                builder.AppendFormat("&address2={0}", WebUtility.UrlEncode(""));
                builder.AppendFormat("&city={0}", WebUtility.UrlEncode(packages.Sender.DefaultAddress.City));
                builder.AppendFormat("&state={0}", WebUtility.UrlEncode(packages.Sender.DefaultAddress.State));
                builder.AppendFormat("&country={0}", WebUtility.UrlEncode(packages.Sender.DefaultAddress.Country));
                builder.AppendFormat("&zip={0}", WebUtility.UrlEncode(packages.Sender.DefaultAddress.ZipCode));
                builder.AppendFormat("&email={0}", WebUtility.UrlEncode(packages.Sender.Email));



                return builder.ToString();
        }
         
        /// <summary>
        /// Gets Paypal URL
        /// </summary>
        /// <returns></returns>
        private string GetPaypalUrl()
        {
            return _settings.Value.UseSandbox ? "https://www.sandbox.paypal.com/us/cgi-bin/webscr" :
                "https://www.paypal.com/us/cgi-bin/webscr";
        }

        /// <summary>
        /// Gets IPN Paypal URL
        /// </summary>
        /// <returns></returns>
        private string GetIpnPaypalUrl()
        {
            return _settings.Value.UseSandbox ? "https://ipnpb.sandbox.paypal.com/cgi-bin/webscr" :
                "https://ipnpb.paypal.com/cgi-bin/webscr";
        }

    }
}
