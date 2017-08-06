using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.eShopOnContainers.BuildingBlocks.Resilience.Http;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using DriveDrop.Web.Services;
using DriveDrop.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using DriveDrop.Web.Infrastructure;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using Polly.CircuitBreaker;

namespace DriveDrop.Web.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {

        private IHttpClient _apiClient;
        private readonly string _remoteServiceBaseUrl;
        private readonly string _remoteServiceCommonUrl;
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly IHttpContextAccessor _httpContextAccesor;
        private readonly IIdentityParser<ApplicationUser> _appUserParser;
         

        public AdminController(IOptionsSnapshot<AppSettings> settings, IHttpContextAccessor httpContextAccesor, 
            IHttpClient httpClient, IIdentityParser<ApplicationUser> appUserParser)
        {
            _remoteServiceCommonUrl = $"{settings.Value.DriveDropUrl}/api/v1/common/";
            _remoteServiceBaseUrl = $"{settings.Value.DriveDropUrl}/api/v1/admin"; 
            _settings = settings;
            _httpContextAccesor = httpContextAccesor;
            _apiClient = httpClient;
            _appUserParser = appUserParser;
             
        }

        public async Task<IActionResult> Index(int? TypeFilterApplied, int? StatusFilterApplied, int? TransportFilterApplied, int? page, string LastName = null)
        {
            try
            {
                var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var isAdminUri = API.Common.IsAdmin(_remoteServiceCommonUrl, user.Email);
            var isAdminString =await  _apiClient.GetStringAsync(isAdminUri, token);
            var isAdminResponse = JsonConvert.DeserializeObject<bool>(isAdminString);

            //if (!isAdminResponse)
            //    return RedirectToAction("myAccount", "home");

            var itemsPage = 3;
            if (page < 0)
                page = 0;

            var allCustomersUri = API.Admin.GetAllCustomers(_remoteServiceBaseUrl, page ?? 0, itemsPage, StatusFilterApplied, TypeFilterApplied, TransportFilterApplied, LastName);

            var dataString = await _apiClient.GetStringAsync(allCustomersUri, token);


            var response = JsonConvert.DeserializeObject<CustomerIndex>(dataString);

             

            return View(response);

        }
            catch (BrokenCircuitException)
            {
                // Catch error when Basket.api is in circuit-opened mode                 
                HandleBrokenCircuitException();
    }


            return View(new List<CustomerIndex>());

        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var getById = API.Admin.GetbyId(_remoteServiceBaseUrl, id ?? 0);


            var dataString = await _apiClient.GetStringAsync(getById, token);


            var response = JsonConvert.DeserializeObject<Customer>((dataString));


            
          //       //call shipping api service
               
          //  var allnotassignedshipings = API.Shipping.GetNotAssignedShipping(_remoteServiceCommonUrl);

          //dataString = await _apiClient.GetStringAsync(allnotassignedshipings, token);

          //  var shippings = JsonConvert.DeserializeObject<List<Shipment>>((dataString));


          //  response.ShipmentSenders = shippings;

            return View(response);

        }
            public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var getById = API.Admin.GetbyId(_remoteServiceBaseUrl, id ?? 0);


            var dataString = await _apiClient.GetStringAsync(getById, token);


            var response = JsonConvert.DeserializeObject<Customer>(dataString);


            //var customer = await _context.Customers
            //    .Where(x => x.CustomerTypeId == CustomerType.Driver.Id)
            //    .Include(d => d.ShipmentDrivers).ThenInclude(ShipmentDrivers => ShipmentDrivers.PriorityType)
            //   .Include(d => d.ShipmentDrivers).ThenInclude(ShipmentDrivers => ShipmentDrivers.ShippingStatus)
            //   .Include(d => d.ShipmentDrivers).ThenInclude(ShipmentDrivers => ShipmentDrivers.PickupAddress)
            //   .Include(d => d.ShipmentDrivers).ThenInclude(ShipmentDrivers => ShipmentDrivers.DeliveryAddress)
            //   .Include(s => s.TransportType).Include(t => t.CustomerStatus).Include(s => s.CustomerType)
            //   .SingleOrDefaultAsync(m => m.Id == id);


            //var tttp = customer.ShipmentSenders;


            //if (customer == null)
            //{
            //    return NotFound();
            //}

            //ViewBag.DriverId = id;

            //ViewBag.ShippingStatuses = _context.ShippingStatuses.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();




            return View(response);
        }

        public async Task<CustomerIndex> PrepareCustomerModel(CustomerIndex model)
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
            model.CustomerType = CustomerTypes;
           
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
            model.CustomerStatus = customerStatus;

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
            model.TransportType = transportTypes;

            return model;
        }





        async Task<string> GetUserTokenAsync()
        {
            var context = _httpContextAccesor.HttpContext;

            return await context.Authentication.GetTokenAsync("access_token");
        }

        private void HandleBrokenCircuitException()
        {
            TempData["DriveDropInoperativeMsg"] = "DriveDrop Service is inoperative, please try later on. (Business Msg Due to Circuit-Breaker)";
        }
    }
}