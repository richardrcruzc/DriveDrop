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
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace DriveDrop.Web.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IHostingEnvironment _env;
        private readonly string _remoteServiceShippingsUrl;
        private IHttpClient _apiClient;
        private readonly string _remoteServiceBaseUrl;
        private readonly string _remoteServiceCommonUrl;
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly IHttpContextAccessor _httpContextAccesor;
        private readonly IIdentityParser<ApplicationUser> _appUserParser;

        private readonly string _remoteServiceDriversUrl;

        public AdminController(IHostingEnvironment env, IOptionsSnapshot<AppSettings> settings, 
            IHttpContextAccessor httpContextAccesor, IHttpClient httpClient, IIdentityParser<ApplicationUser> appUserParser)
        {
            _env = env;

            _remoteServiceCommonUrl = $"{settings.Value.DriveDropUrl}/api/v1/common/";
            _remoteServiceBaseUrl = $"{settings.Value.DriveDropUrl}/api/v1/admin"; 
            _settings = settings;
            _httpContextAccesor = httpContextAccesor;
            _apiClient = httpClient;
            _appUserParser = appUserParser;
            _remoteServiceShippingsUrl = $"{settings.Value.DriveDropUrl}/api/v1/shippings";
            _remoteServiceDriversUrl = $"{settings.Value.DriveDropUrl}/api/v1/drivers";
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateInfo(CustomerInfoModel model, List<IFormFile> PersonalPhotoUri)
        {
            var result = "Info updated";
            if (ModelState.IsValid)
            {
                try
                {

                    var fileName = await SaveFile(PersonalPhotoUri, "driver");

                    if (!string.IsNullOrWhiteSpace(fileName))
                        model.PhotoUrl = fileName;
                    else
                        model.PhotoUrl = model.PersonalPhotoUri;

                    var user = _appUserParser.Parse(HttpContext.User);
                    var token = await GetUserTokenAsync();

                    var getUserUri = API.Admin.GetbyUserName(_remoteServiceBaseUrl, user.Email);
                    var userString = await _apiClient.GetStringAsync(getUserUri, token);
                    var customer = JsonConvert.DeserializeObject<CurrentCustomerModel>(userString);
                    if (customer != null)
                    {



                        var updateInfo = API.Driver.UpdateInfo(_remoteServiceDriversUrl);

                        var response = await _apiClient.PostAsync(updateInfo, model, token);
                        if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                        {
                            //throw new Exception("Error creating Shipping, try later.");

                            ModelState.AddModelError("", "Error creating Shipping, try later.");

                        }
                        else
                            return RedirectToAction("details", new { id = 1 });
                    }
                }
                catch (DbUpdateException ex)
                {
                    //Log the error (uncomment ex variable name and write a log.
                    var error = string.Format("Unable to save changes. " +
                        "Try again, and if the problem persists " +
                        "see your system administrator. {0}", ex.Message);

                    ModelState.AddModelError("", error);
                    result = error;
                }
            }

           
           return View(model);
        }

        public async Task<IActionResult> AssignDriver(int shipingId, int driverId)
        {


            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var isAdminUri = API.Common.IsAdmin(_remoteServiceCommonUrl, user.Email);
            var isAdminString = await _apiClient.GetStringAsync(isAdminUri, token);
            var isAdminResponse = JsonConvert.DeserializeObject<bool>(isAdminString);

            if (!isAdminResponse)
                return Json("Invalid entry");


            var assign = API.Driver.AssignDriver(_remoteServiceDriversUrl, driverId, shipingId);


            var dataString = await _apiClient.GetStringAsync(assign, token);
             

            return Json("Ok");
             

        }



        public async Task<IActionResult> DriverAutoComplete(string value)
        {

            if(value==null)
                return Json(new List<CustomerInfoModel>());

            if (value.Length<3)
                return Json(new List<CustomerInfoModel>());


            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var isAdminUri = API.Common.IsAdmin(_remoteServiceCommonUrl, user.Email);
            var isAdminString = await _apiClient.GetStringAsync(isAdminUri, token);
            var isAdminResponse = JsonConvert.DeserializeObject<bool>(isAdminString);

            if (!isAdminResponse)
                return Json("Invalid entry");

            var auto = API.Driver.AutoComplete(_remoteServiceDriversUrl, value);

            var dataString = await _apiClient.GetStringAsync(auto, token);

            var impersonateResponse = JsonConvert.DeserializeObject<List<CustomerInfoModel>>((dataString));


            return Json(impersonateResponse);


        }



        public async Task<IActionResult> EndImpersonated()
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();


            var getcurrent = API.Admin.GetbyUserName(_remoteServiceBaseUrl, user.Email);
            var currentDataString = await _apiClient.GetStringAsync(getcurrent, token);
            var currentAdmin = JsonConvert.DeserializeObject<CurrentCustomerModel>((currentDataString));


            if (currentAdmin == null)
            {
                return NotFound();
            }
            if (currentAdmin.CanBeUnImpersonate)
            {
                var impersonate = API.Admin.EndImpersonated(_remoteServiceBaseUrl, user.Email);
                var impersonateString = await _apiClient.GetStringAsync(impersonate, token);
                var impersonateResponse = JsonConvert.DeserializeObject<bool>((impersonateString));
            }

            return RedirectToAction("Index", "home");
        }
            public async Task<IActionResult> SetImpersonate(string userToImpersonate, string code)
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();


            var getcurrent = API.Admin.GetbyUserName(_remoteServiceBaseUrl, user.Email);
            var currentDataString = await _apiClient.GetStringAsync(getcurrent, token);
            var currentAdmin = JsonConvert.DeserializeObject<CurrentCustomerModel>((currentDataString));


            if (currentAdmin == null)
            {
                return Json("Invalid entry");
            }

            if (!currentAdmin.IsAdmin)
                return Json("Invalid entry");


            var getUser = API.Admin.GetbyUserName(_remoteServiceBaseUrl, userToImpersonate);
            var userDataString = await _apiClient.GetStringAsync(getUser, token);
            var currentUser = JsonConvert.DeserializeObject<CurrentCustomerModel>((userDataString));

            if (currentUser == null || currentUser.UserName == null)
            {
                return Json("Invalid entry: User Name invalid!"+ userToImpersonate);
            }
            if (currentUser.VerificationId==null || currentUser.VerificationId.ToLower() != code.ToLower())
            {
                return Json("Invalid Impersonate code"); ;
            }

            var impersonateUri = API.Admin.SetImpersonate(_remoteServiceBaseUrl, user.Email, userToImpersonate);
            var impersonateString = await _apiClient.GetStringAsync(impersonateUri, token);
            var isImpersonatedResponse = JsonConvert.DeserializeObject<bool>(impersonateString);

            if (!isImpersonatedResponse)
                return Json("Invalid entry");          


            return Json("User Impersonated");

        }

        public async Task<IActionResult> ChangShippingStatus(int shippingId, int statusId)
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var isAdminUri = API.Common.IsAdmin(_remoteServiceCommonUrl, user.Email);
            var isAdminString = await _apiClient.GetStringAsync(isAdminUri, token);
            var isAdminResponse = JsonConvert.DeserializeObject<bool>(isAdminString);

            if (!isAdminResponse)
                return Json("Invalid entry");

            var assign = API.Shipping.UpdatePackageStatus(_remoteServiceShippingsUrl, shippingId, statusId);


            var dataString = await _apiClient.GetStringAsync(assign, token);
             
           
            return Json("Status Updated");


        }

        public async Task<IActionResult> ChangeCustomerStatus(int customerId, int statusId)
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var isAdminUri = API.Common.IsAdmin(_remoteServiceCommonUrl, user.Email);
            var isAdminString = await _apiClient.GetStringAsync(isAdminUri, token);
            var isAdminResponse = JsonConvert.DeserializeObject<bool>(isAdminString);

            if (!isAdminResponse)
                return Json("Invalid entry");

            var changeCustomerStatusUri = API.Admin.ChangeCustomerStatus(_remoteServiceBaseUrl, customerId, statusId);

            var dataString = await _apiClient.GetStringAsync(changeCustomerStatusUri, token);


            //var response = JsonConvert.DeserializeObject<string>(dataString);


            return Json(dataString);

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

             if (!isAdminResponse)
                    return NotFound("Invalid entry");

                var itemsPage = 10;
            if (page < 0)
                page = 0;

            var allCustomersUri = API.Admin.GetAllCustomers(_remoteServiceBaseUrl, page ?? 0, itemsPage, StatusFilterApplied, TypeFilterApplied, TransportFilterApplied, LastName);

            var dataString = await _apiClient.GetStringAsync(allCustomersUri, token);


            var response = JsonConvert.DeserializeObject<CustomerIndex>(dataString);

                ViewBag.CustomerStatus = response.CustomerStatus;
                if (response.LastName == null || response.LastName == "null")
                    response.LastName = string.Empty;
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

            var getcurrent = API.Admin.GetbyUserName(_remoteServiceBaseUrl, user.Email);
            var currentDataString = await _apiClient.GetStringAsync(getcurrent, token);
            var currentUser = JsonConvert.DeserializeObject<CurrentCustomerModel>((currentDataString));
             

            if (currentUser == null)
            {
                return NotFound();
            }

            if (!currentUser.IsAdmin)
                return NotFound("Invalid entry"); 

            var getById = API.Admin.GetbyId(_remoteServiceBaseUrl, id ?? 0);
            var dataString = await _apiClient.GetStringAsync(getById, token);
            var response = JsonConvert.DeserializeObject<CurrentCustomerModel>((dataString));


            if (string.IsNullOrWhiteSpace(response.PersonalPhotoUri))
                response.PersonalPhotoUri = _settings.Value.CallBackUrl + "/images/DefaultProfileImage.png";
          
            if (string.IsNullOrWhiteSpace(response.DriverLincensePictureUri))
                response.DriverLincensePictureUri = _settings.Value.CallBackUrl + "/images/DefaultProfileImage.png";
          
            if (string.IsNullOrWhiteSpace(response.VehiclePhotoUri))
                response.VehiclePhotoUri = _settings.Value.CallBackUrl + "/images/DefaultProfileImage.png";
            
          
            if (string.IsNullOrWhiteSpace(response.InsurancePhotoUri))
                response.InsurancePhotoUri = _settings.Value.CallBackUrl + "/images/DefaultProfileImage.png";

            response.CustomerType = response.CustomerType.ToTitleCase();
            response.CustomerStatus = response.CustomerStatus.ToTitleCase();

            

            return View(response);


        }
        public async Task<IActionResult> Shippings(int? PriorityTypeFilterApplied, int? ShippingStatusFilterAApplied, string IdentityCode,   int? page)
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();
            var isAdminUri = API.Common.IsAdmin(_remoteServiceCommonUrl, user.Email);
            var isAdminString = await _apiClient.GetStringAsync(isAdminUri, token);
            var isAdminResponse = JsonConvert.DeserializeObject<bool>(isAdminString);

            if (!isAdminResponse)
                return NotFound("Invalid entry");

            var itemsPage = 10;
            if (page < 0)
                page = 0;

            @ViewBag.CustomerId = 1;
            //call shipping api service     

            var allnotassignedshipings = API.Shipping.GetShipping(_remoteServiceShippingsUrl, ShippingStatusFilterAApplied??0, PriorityTypeFilterApplied??0, IdentityCode, page ?? 0, itemsPage);

            var dataString = await _apiClient.GetStringAsync(allnotassignedshipings, token);

            var shippings = JsonConvert.DeserializeObject<ShipingIndex>((dataString));
            if (shippings == null)
                return View(new ShipingIndex());
            
            return View(shippings);
        }
        public async Task<IActionResult> ShippingDetails(int id)
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();
            var isAdminUri = API.Common.IsAdmin(_remoteServiceCommonUrl, user.Email);
            var isAdminString = await _apiClient.GetStringAsync(isAdminUri, token);
            var isAdminResponse = JsonConvert.DeserializeObject<bool>(isAdminString);

            if (!isAdminResponse)
                return NotFound("Invalid entry");

            var shiipingDetailUri = API.Shipping.GetById(_remoteServiceShippingsUrl, id);
            var dataString = await _apiClient.GetStringAsync(shiipingDetailUri, token);
            var shipping = JsonConvert.DeserializeObject<Shipment>((dataString));



            shipping.ShippingStatusList = await PrepareShippingStatus();


            return View(shipping);
        }

        public async Task<IActionResult> ReadyToPickUp(int id)
        {
            @ViewBag.CustomerId = id;
            //call shipping api service
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var allnotassignedshipings = API.Shipping.GetNotAssignedShipping(_remoteServiceShippingsUrl);

            var dataString = await _apiClient.GetStringAsync(allnotassignedshipings, token);

            var shippings = JsonConvert.DeserializeObject<PaginatedShippings>((dataString));
            if (shippings == null)
                return View(new PaginatedShippings());
            shippings.ShippingStatusList = await PrepareShippingStatus();
            return View(shippings);
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
             


            return View(response);
        }

        public async Task<CustomerIndex> PrepareCustomerModel(CustomerIndex model)
        {
           var getUri = API.Common.GetAllCustomerTypes(_remoteServiceCommonUrl);
            var dataString = await _apiClient.GetStringAsync(getUri);
            var CustomerTypes = new List<SelectListItem>();
            CustomerTypes.Add(new SelectListItem() { Value = null, Text = "All Types", Selected = true });

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
            customerStatus.Add(new SelectListItem() { Value = null, Text = "All Status", Selected = true });

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
            transportTypes.Add(new SelectListItem() { Value = null, Text = "All Vehicle Types", Selected = true });

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



        public async Task<List<SelectListItem>> PrepareShippingStatus()
        {
            var getUri = API.Common.GetAllShippingStatus(_remoteServiceCommonUrl);
            var dataString = await _apiClient.GetStringAsync(getUri);
            var shippingStatus = new List<SelectListItem>();
            shippingStatus.Add(new SelectListItem() { Value = null, Text = "All", Selected = true });

            var gets = JArray.Parse(dataString);

            foreach (var brand in gets.Children<JObject>())
            {
                shippingStatus.Add(new SelectListItem()
                {
                    Value = brand.Value<string>("id"),
                    Text = brand.Value<string>("name")
                });
            }

            return shippingStatus;
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
        [NonAction]
        public async Task<string> SaveFile(List<IFormFile> files, string belong)
        {

            Guid extName = Guid.NewGuid();
            //saving files
            long size = files.Sum(f => f.Length);

            // full path to file in temp location
            var filePath = Path.GetTempFileName();
            var uploads = Path.Combine(_env.WebRootPath, string.Format("uploads\\img\\{0}", belong));
            var fileName = "";

            foreach (var formFile in files)
            {

                if (formFile.Length > 0)
                {
                    var extension = ".jpg";
                    if (formFile.FileName.ToLower().EndsWith(".jpg"))
                        extension = ".jpg";
                    if (formFile.FileName.ToLower().EndsWith(".tif"))
                        extension = ".tif";
                    if (formFile.FileName.ToLower().EndsWith(".png"))
                        extension = ".png";
                    if (formFile.FileName.ToLower().EndsWith(".gif"))
                        extension = ".gif";




                    filePath = string.Format("{0}\\{1}{2}", uploads, extName, extension);
                    fileName = string.Format("/uploads/img/{0}/{1}{2}", belong, extName, extension);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }
            return fileName;

        }

    }
}