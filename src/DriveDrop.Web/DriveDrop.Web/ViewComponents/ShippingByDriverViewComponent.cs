
using DriveDrop.Web.Infrastructure;
using DriveDrop.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.eShopOnContainers.BuildingBlocks.Resilience.Http;
using Microsoft.AspNetCore.Http;
using DriveDrop.Web.Services;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json.Linq;

namespace DriveDrop.Web.ViewComponents
{
    public class ShippingByDriverViewComponent : ViewComponent
    {



        private IHttpClient _apiClient;
        private readonly string _remoteServiceBaseUrl;
        private readonly string _remoteServiceCommonUrl;
        private readonly string _remoteServiceShippingUrl;
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly IHttpContextAccessor _httpContextAccesor;
        private readonly IIdentityParser<ApplicationUser> _appUserParser;


        public ShippingByDriverViewComponent(IOptionsSnapshot<AppSettings> settings, 
            IHttpContextAccessor httpContextAccesor,
            IHttpClient httpClient, IIdentityParser<ApplicationUser> appUserParser)
        {
            _remoteServiceCommonUrl = $"{settings.Value.DriveDropUrl}/api/v1/common/";
            _remoteServiceBaseUrl = $"{settings.Value.DriveDropUrl}/api/v1/admin";
            _remoteServiceShippingUrl = $"{settings.Value.DriveDropUrl}/api/v1/shippings";
            _settings = settings;
            _httpContextAccesor = httpContextAccesor;
            _apiClient = httpClient;
            _appUserParser = appUserParser;

        }


         

        public async Task<IViewComponentResult> InvokeAsync(int driverId)
        {
            //call shipping api service
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var allnotassignedshipings = API.Shipping.GetShippingByDriverId(_remoteServiceShippingUrl, driverId);

            var dataString = await _apiClient.GetStringAsync(allnotassignedshipings, token);

            var shippings = JsonConvert.DeserializeObject<PaginatedShippings>((dataString));
             if(shippings==null)
                return View(new PaginatedShippings());
            

            //var model = shippings.Select(x => new Shipment
            //{
            //    Amount = x.Amount,
            //    DeliveredPictureUri = x.DeliveredPictureUri,
            //    DeliveryAddress = x.DeliveryAddress,
            //    Discount = x.Discount,
            //    Driver = x.Driver,
            //    DriverId = x.DriverId,
            //    Id = x.Id,
            //    IdentityCode = x.IdentityCode,
            //    Note = x.Note,
            //    PickupAddress = x.PickupAddress,
            //    PickupPictureUri = x.PickupPictureUri,
            //    PriorityType = x.PriorityType,
            //    PriorityTypeId = x.PriorityTypeId,
            //    PriorityTypeLevel = x.PriorityTypeLevel,
            //    PromoCode = x.PromoCode,
            //    Sender = x.Sender,
            //    SenderId = x.SenderId,
            //    ShippingCreateDate = x.ShippingCreateDate,
            //    ShippingStatus = x.ShippingStatus,
            //    ShippingStatusId = x.ShippingStatusId,
            //    ShippingUpdateDate = x.ShippingUpdateDate,
            //    Tax = x.Tax,
            //    TransportType = x.TransportType,
            //    TransportTypeId = x.TransportTypeId

                //}).ToList();




            return View(shippings);

            //var model = await _context.Shipments
            //    .Where(x => x.ShippingStatusId == ShippingStatus.PendingPickUp.Id && x.DriverId == null)
            //    .Include(d => d.DeliveryAddress)
            //    .Include(d => d.PickupAddress)
            //    .Include(d => d.ShippingStatus)
            //    .ToListAsync();

            //return View(model);
        }


        public async Task<NewShipment> PrepareCustomerModelAsync(NewShipment model)
        {
            var getUri = API.Common.GetAllCustomerTypes(_remoteServiceCommonUrl);
            var dataString = await _apiClient.GetStringAsync(getUri);
            var CustomerTypes = new List<SelectListItem>();
            CustomerTypes.Add(new SelectListItem() { Value = null, Text = "All", Selected = true });

            var gets = JArray.Parse(dataString);

            foreach (var brand in gets.Children<JObject>())
            {
                CustomerTypes.Add(new SelectListItem()
                {
                    Value = brand.Value<string>("id"),
                    Text = brand.Value<string>("name")
                });
            }
            model.CustomerTypeList = CustomerTypes;

            getUri = API.Common.GetAllCustomerStatus(_remoteServiceCommonUrl);
            dataString = await _apiClient.GetStringAsync(getUri);
            var customerStatus = new List<SelectListItem>();
            customerStatus.Add(new SelectListItem() { Value = null, Text = "All", Selected = true });

            gets = JArray.Parse(dataString);

            foreach (var brand in gets.Children<JObject>())
            {
                customerStatus.Add(new SelectListItem()
                {
                    Value = brand.Value<string>("id"),
                    Text = brand.Value<string>("name")
                });
            }
            model.CustomerStatusList = customerStatus;

            getUri = API.Common.GetAllTransportTypes(_remoteServiceCommonUrl);
            dataString = await _apiClient.GetStringAsync(getUri);
            var transportTypes = new List<SelectListItem>();
            transportTypes.Add(new SelectListItem() { Value = null, Text = "All", Selected = true });

            gets = JArray.Parse(dataString);

            foreach (var brand in gets.Children<JObject>())
            {
                transportTypes.Add(new SelectListItem()
                {
                    Value = brand.Value<string>("id"),
                    Text = brand.Value<string>("name")
                });
            }
            model.TransportTypeList = transportTypes;

            return model;
        }
        async Task<string> GetUserTokenAsync()
        {
            var context = _httpContextAccesor.HttpContext;

            return await context.Authentication.GetTokenAsync("access_token");
        }
    }
}
