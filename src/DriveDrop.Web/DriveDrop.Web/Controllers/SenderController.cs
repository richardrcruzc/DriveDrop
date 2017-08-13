﻿
using DriveDrop.Web.Infrastructure;
using DriveDrop.Web.Services;
using DriveDrop.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopOnContainers.BuildingBlocks.Resilience.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Web.Controllers
{
    [Authorize]
    public class SenderController : Controller
    {


        private IHttpClient _apiClient;
        private readonly string _remoteServiceBaseUrl;
        private readonly string _remoteServiceCommonUrl;
        private readonly string _remoteServiceShippingsUrl;
        private readonly string _remoteServiceRatesUrl;
        private readonly string _remoteServiceIdentityUrl;
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly IHttpContextAccessor _httpContextAccesor;
        private readonly IIdentityParser<ApplicationUser> _appUserParser;

        private readonly IHostingEnvironment _env;


        public SenderController(IOptionsSnapshot<AppSettings> settings, IHttpContextAccessor httpContextAccesor,
            IHttpClient httpClient, IIdentityParser<ApplicationUser> appUserParser,
            IHostingEnvironment env)
        {
            _remoteServiceCommonUrl = $"{settings.Value.DriveDropUrl}/api/v1/common/";
            _remoteServiceBaseUrl = $"{settings.Value.DriveDropUrl}/api/v1/sender";
            _remoteServiceShippingsUrl = $"{settings.Value.DriveDropUrl}/api/v1/shippings";
            _remoteServiceRatesUrl = $"{settings.Value.DriveDropUrl}/api/v1/rates/";
            _remoteServiceIdentityUrl= $"{settings.Value.IdentityUrl}/account/";

            _settings = settings;
            _httpContextAccesor = httpContextAccesor;
            _apiClient = httpClient;
            _appUserParser = appUserParser;

            _env = env;

        

        }

        public IActionResult Index()
        {
            return View();
        }

       
        public IActionResult AddressAdd(int id)
        {
            ViewBag.Id = id;
            var model = new AddressModel { CustomerId = id };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]       
        public async Task<string> AddressAdd(AddressModel model)
        {
            var result = "Address added";
            ViewBag.Id = model.CustomerId;
            //call shipping api service
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var updateInfo = API.Sender.AddAddress(_remoteServiceBaseUrl);

            var response = await _apiClient.PostAsync(updateInfo, model, token);
            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                //throw new Exception("Error creating Shipping, try later.");

                ModelState.AddModelError("", "Error creating Shipping, try later.");
                result = "Error creating Shipping, try later.";
            }

            return result;
        }
            public async Task<IActionResult> Address(int id)
        {
            ViewBag.Id = id;
            //call shipping api service
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var getById = API.Sender.GetbyId(_remoteServiceBaseUrl, id  );


            var dataString = await _apiClient.GetStringAsync(getById, token);


            var response = JsonConvert.DeserializeObject<Customer>((dataString));

             

            return View(response.Addresses);

                    }

        public async Task<IActionResult> Shippings(int id)
        {
            //call shipping api service
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var allnotassignedshipings = API.Shipping.GetShippingByCustomerId(_remoteServiceShippingsUrl, id);

            var dataString = await _apiClient.GetStringAsync(allnotassignedshipings, token);

            ViewBag.Id = id;
            var shippings = JsonConvert.DeserializeObject<List<Shipment>>((dataString));
            if (shippings == null)
                return View(new List<Shipment>());
            return View(shippings);
            
        }

        public async Task<IActionResult> Result(int? id)
        {

            ViewBag.Url = _remoteServiceRatesUrl;

            if (id == null)
            {
                return NotFound();
            }

            if (id == null)
            {
                return NotFound();
            }

            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();


            var idIsUserUri = API.Common.IdIsUser(_remoteServiceCommonUrl, user.Email, id ?? 0);

            var idIsUserdataString = await _apiClient.GetStringAsync(idIsUserUri, token);

            var idIsUserresponse = JsonConvert.DeserializeObject<bool>((idIsUserdataString));

            if (!idIsUserresponse)
            {
                return NotFound();
            }





            var getById = API.Sender.GetbyId(_remoteServiceBaseUrl, id ?? 0);

            var dataString = await _apiClient.GetStringAsync(getById, token); 

            var response = JsonConvert.DeserializeObject<Customer>((dataString));
            var model = new CustomerInfoModel {
                    CustomerStatus = response.CustomerStatus.Name,
                    Email = response.Email,
                    FirstName = response.FirstName,
                    LastName = response.LastName,
                    Id = id??0,
                    Phone = response.Phone, 
                    PhotoUrl = response.PersonalPhotoUri,
                    PrimaryPhone = response.PrimaryPhone,
                    StatusId = response.CustomerStatusId, 
            };
            if (string.IsNullOrWhiteSpace(model.PhotoUrl))
                model.PhotoUrl = _settings.Value.CallBackUrl + "/images/DefaultProfileImage.png";
             else
                model.PhotoUrl =  model.PhotoUrl ;

            ViewBag.PhotoUrl = _settings.Value.CallBackUrl + "/" + model.PhotoUrl;
            



            return View(model);
        }
 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<string> DeleteAddress(AddressModel model)
        {
            var result = "Address deleted";
            ViewBag.Id = model.CustomerId;
            //call shipping api service
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var updateInfo = API.Sender.DeleteAddress(_remoteServiceBaseUrl);

            var response = await _apiClient.PostAsync(updateInfo, model, token);
            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                //throw new Exception("Error creating Shipping, try later.");

                ModelState.AddModelError("", "Error creating deleting, try later.");
                result = "Error deleting address, try later.";
            }

            return result;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<string> UpdateInfo(CustomerInfoModel model, List<IFormFile> photoUrl)
        {
            var result = "Info updated";
            if (ModelState.IsValid)
            {
                try
                {

                var fileName = await SaveFile(photoUrl, "sender");
 
                    if (!string.IsNullOrWhiteSpace(fileName))
                model.PhotoUrl = fileName;

                var user = _appUserParser.Parse(HttpContext.User);
                var token = await GetUserTokenAsync();

                var updateInfo = API.Sender.UpdateInfo(_remoteServiceBaseUrl);

                var response = await _apiClient.PostAsync(updateInfo, model, token);
                if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    //throw new Exception("Error creating Shipping, try later.");

                    ModelState.AddModelError("", "Error creating Shipping, try later.");

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

            return result;
        }

        public async Task<IActionResult> NewShipping(int id)
        {  
           

            var model = new NewShipment();
            await PrepareCustomerModel(model);
            model.CustomerId = id;

            await PrepareCustomerAddresses(model, id);            

            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewShipping(NewShipment c, List<IFormFile> photoUrl)
        {

            try
            {
                ModelState.Clear();
              
                //foreach (var state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
                //{
                //    var tt = state.Errors.ToString();
                //    ModelState.AddModelError("", state.Errors[0].ErrorMessage);
                //}
                if (c.PickupAddressId == 0)
                {                  
                    if(string.IsNullOrEmpty(c.PickupStreet))
                        ModelState.AddModelError("", "Select a pickup address");
                    if (string.IsNullOrEmpty(c.PickupPhone))
                        ModelState.AddModelError("", "Select a pickup Phone");
                    if (string.IsNullOrEmpty(c.PickupContact))
                        ModelState.AddModelError("", "Select a pickup Contact");
                     
                }
                 
                if (c.DropAddressId == 0)
                {

                    if (string.IsNullOrEmpty(c.DeliveryStreet))
                        ModelState.AddModelError("", "Select a drop address");
                    if (string.IsNullOrEmpty(c.DeliveryPhone))
                        ModelState.AddModelError("", "Select a drop Phone");
                    if (string.IsNullOrEmpty(c.DeliveryContact))
                        ModelState.AddModelError("", "Select a drop Contact");
                     
                }

                if (photoUrl.Count() == 0)
                {
                    ModelState.AddModelError("", "Select package picture");
                }
                if (c.PackageSizeId == 0)
                {
                    ModelState.AddModelError("", "Select package size");
                }

                if (c.PriorityTypeId == 0)
                { ModelState.AddModelError("", "Select package priority"); }
                 
                if (c.Amount == 0)
                {
                    ModelState.AddModelError("", "Select package value");
                }
                if (c.ShippingWeight == 0)
                {
                    ModelState.AddModelError("", "Select package Weight");
                }
                var errors = ViewData.ModelState.Values.Count();
                if (errors == 0)
                {
                    var user = _appUserParser.Parse(HttpContext.User);
                    var token = await GetUserTokenAsync();

                    c.PickupPictureUri = await SaveFile(photoUrl, "Shipment");


                    var addNewShippingUri = API.Sender.SaveNewShipment(_remoteServiceShippingsUrl);

                    var response = await _apiClient.PostAsync(addNewShippingUri, c, token);
                    if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        //throw new Exception("Error creating Shipping, try later.");
                        await PrepareCustomerModel(c);

                        ModelState.AddModelError("", "Error creating Shipping, try later.");
                        return View(c);
                    }

                    // try to process payment with  paypal

                    return RedirectToAction("PostToPayPalAsync", new { item = "Charge per Shipping Service", amount = 100, customerId = c.CustomerId });
                }
                    // response.EnsureSuccessStatusCode();
                    //return RedirectToAction("result", new { id = c.CustomerId });
                    // return CreatedAtAction(nameof(Result), new { id = c.CustomerId }, null);

                }
            catch (DbUpdateException ex)
            {
                //Log the error (uncomment ex variable name and write a log.
                var error = string.Format("Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator. {0}", ex.Message);

                ModelState.AddModelError("", error);
            }


            await PrepareCustomerModel(c);
            await PrepareCustomerAddresses(c, c.CustomerId);

            return View(c);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveNewShipment(NewShipment c, List<IFormFile> files)
        {
            try
            {


                foreach (var state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
                {
                    var tt = state.Errors.ToString();
                }

                if (ModelState.IsValid)
                {

                    var user = _appUserParser.Parse(HttpContext.User);
                    var token = await GetUserTokenAsync();



                    Guid extName = Guid.NewGuid();
                    //saving files
                    long size = files.Sum(f => f.Length);

                    // full path to file in temp location
                    var filePath = Path.GetTempFileName();
                    var uploads = Path.Combine(_env.WebRootPath, "uploads\\img\\Shipment");
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
                            fileName = string.Format("uploads\\img\\Shipment\\{0}{1}", extName, extension);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await formFile.CopyToAsync(stream);
                            }
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(fileName))
                    {
                        c.PickupPictureUri = fileName;
                    }

                    var addNewShippingUri = API.Sender.SaveNewShipment(_remoteServiceShippingsUrl);

                    var response = await _apiClient.PostAsync(addNewShippingUri, c, token);
                    if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        throw new Exception("Error creating Shipping, try later.");
                    }


                    // response.EnsureSuccessStatusCode();
                   return RedirectToAction("result", new { id = c.CustomerId });
                   // return CreatedAtAction(nameof(Result), new { id = c.CustomerId }, null);
                }
            }
            catch (DbUpdateException ex)
            {
                //Log the error (uncomment ex variable name and write a log.
                var error = string.Format("Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator. {0}", ex.Message);

                ModelState.AddModelError("", error);
            }


            await PrepareCustomerModel(c);

            return View(c);

        }
        
        async Task<string> GetUserTokenAsync()
        {
            var context = _httpContextAccesor.HttpContext;

            return await context.Authentication.GetTokenAsync("access_token");
        }
        [AllowAnonymous]
        public async Task<IActionResult> NewSender()
        {
            //await HttpContext.Authentication.SignOutAsync("Cookies");
            //await  HttpContext.Authentication.SignOutAsync("oidc");

            CustomerModel c = new CustomerModel();
            await PrepareCustomerModel(c);
            c.CustomerTypeId = 2;
            c.Distance = 0;
            return View(c);
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewSender(CustomerModel c, List<IFormFile> packageImage, List<IFormFile> imgeFoto)
        {
            try
            {
                foreach (var state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
                {
                    var tt = state.Errors.ToString();
                }

                if (ModelState.IsValid)
                {
                    //var token = await GetUserTokenAsync();

                    //try register new user

                    var addNewUserUri = API.Identity.RegisterUser(_remoteServiceIdentityUrl,c.UserEmail, c.Password);

                    var dataString = await _apiClient.GetStringAsync(addNewUserUri);

                    //var isCreated = JsonConvert.DeserializeObject<string>((dataString));

                    if (dataString == null)
                    {
                        ModelState.AddModelError("", "Unable to register user");
                        await PrepareCustomerModel(c);
                        return View(c);
                    }

                    if (!dataString.Contains("IsAuthenticated") && !dataString.Contains("IsNotAuthenticated"))
                    {

                        ModelState.AddModelError("", "Unable to register user");
                        await PrepareCustomerModel(c);
                        return View(c);
                    }

                    //var user = _appUserParser.Parse(HttpContext.User);

                    var packageUri = await SaveFile(packageImage, "Shipment");
                    var ppersonalUri = await SaveFile(imgeFoto, "Sender");

                    c.FilePath = packageUri;
                    c.PersonalPhotoUri = ppersonalUri;


                    var addNewSenderUri = API.Sender.NewSender(_remoteServiceBaseUrl);

                    c.CustomerTypeId = 2;

                    var response = await _apiClient.PostAsync(addNewSenderUri, c);

                    if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        throw new Exception("Error creating Shipping, try later.");
                    }

                    // return RedirectToAction("result", new { id = c.CustomerId });
                    //return RedirectToAction("SignIn", "Account");
                   // var results = new NewSenderResult { Amount = "", Message="", UserName =c.UserEmail };

                    return RedirectToAction("NewSenderResults", new { user = c.UserEmail });
                }
            }               
                catch (DbUpdateException ex)
                {

                var error = string.Format("Unable to save changes. " +
                   "Try again, and if the problem persists " +
                   "see your system administrator. {0}", ex.Message);

                ModelState.AddModelError("", error);
            }


           await  PrepareCustomerModel(c);
             return View(c);

        } 
        [AllowAnonymous]
        public IActionResult NewSenderResults(string user)
        {
            return View("NewSenderResults",user);
        }

        [AllowAnonymous]
        public async Task<JsonResult> ValidateUserName(string UserEmail)
        {
            var validateUri = API.Common.ValidateUserName(_remoteServiceCommonUrl, UserEmail);

            var response = await _apiClient.GetStringAsync(validateUri); 

            return Json(!response.Equals("duplicate"));
        }

        private ActionResult Json(bool v, object allowGet)
        {
            throw new NotImplementedException();
        }

         
        public async Task<IActionResult> PostToPayPalAsync(string item, string amount, int customerId)
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            ViewBag.CustomerId = customerId;

            var paypal = new Paypal();
            paypal.cmd = "_xclick";
            paypal.business = _settings.Value.BusinessAccountKey;
            bool useSandBox = _settings.Value.UseSandbox;
            if (useSandBox)
                ViewBag.actionURL = "https://www.sandbox.paypal.com/cgi-bin/webscr?";
            else
                ViewBag.actionURL = "https://www.paypal.com/cgi-bin/webscr?";

            paypal.cancel_return = string.Format(_settings.Value.CancelURL, customerId);
            paypal.returN = string.Format(_settings.Value.ReturnURL, customerId);
            paypal.notify_url = _settings.Value.NotifyURL;
            paypal.currency_code = _settings.Value.CurrencyCode;
            paypal.item_name = item;
            paypal.item_number = string.Format("Shipping-{0}", customerId);
            paypal.amount = amount;

            return View(paypal);

            //        paypal.url = "https://www.paypal.com/cgi-bin/webscr?",
            //       
            //        var business = Your Business Email;
            //        var currency_code = "AUD";
            //        var amount = 100;
            //        var item_name = Name Of Your Item;
            //        var item_number = Some Identifier;
            //        var returnurl = "http://somepage?info=success";
            //        var cancel_return = "http://somepage?info=failed";
            //        var notify_url = "http://WebFacingSite/API/PayPalReg";
            //        var tax = (amount * 0.10);
            //    }
            //        var fullURL = URL + "cmd=" + cmd + "&business=" + business + "&currency_code=" + currency_code + "&amount=" + amount + "&tax=" + tax + "&item_name=" + item_name + "&item_number=" + item_number + "&return=" + returnurl + "&cancel_return=" + cancel_return + "&notify_url=" + notify_url;

            /////// this ajax bit I use to record the transaction has started
            //$.ajax({
            //            contentType: "application/json; charset=utf-8",
            //    dataType: "json",
            //    url: "/API/PaymentStarted?eventid=" + eventid + "&UserID=" + UserID + "&paymentID" + paymentID,
            //    error: function() {
            //                SetMessage("error", "Something has gone horribly, horribly wrong")
            //    },
            //    success: function(data) {

            //                window.location.href = fullURL;

            //            },
            //    type: "POST"
            //});


        }

        public IActionResult NotifyFromPaypal()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Return(string info, int customerId)
        {
            ViewBag.CustomerId = customerId;
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [NonAction]
        public async Task<NewShipment> PrepareCustomerAddresses(NewShipment model, int id)
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var getById = API.Sender.GetbyId(_remoteServiceBaseUrl, id);

            var dataString = await _apiClient.GetStringAsync(getById, token);

            var response = JsonConvert.DeserializeObject<Customer>((dataString));
            var addressesP = new List<SelectListItem>();
            var addressesD = new List<SelectListItem>();

            foreach (var a in response.Addresses)
            {
                var stringAddress = string.Format("{0},{1},{2},{3},{4} ",
                     a.Street, a.City, a.State, a.ZipCode, a.Country);

                if (a.TypeAddress.ToLower() == "pickup")
                    addressesP.Add(new SelectListItem()
                    {
                        Value = a.Id.ToString(),
                        Text = stringAddress
                    });

                else
                    addressesD.Add(new SelectListItem()
                    {
                        Value = a.Id.ToString(),
                        Text = stringAddress
                    });

            }
            addressesP.Add(new SelectListItem() { Value = null, Text = "Add new address", Selected = true });
            addressesD.Add(new SelectListItem() { Value = null, Text = "Add new address", Selected = true });

            model.PickupAddresses = addressesP;
            model.DropAddresses = addressesD;

            if (response.Addresses.Where(pa=>pa.TypeAddress.ToLower() == "pickup").Any()) {
              var a= response.Addresses.Where(p => p.TypeAddress.ToLower() == "pickup").FirstOrDefault();
                model.PickupAddressId = a.Id;
                model.PickupStreet = a.Street;
                model.PickupCity = a.City;
                model.PickupState = a.State;
                model.PickupCountry = a.Country;
                model.PickupZipCode = a.ZipCode;
                model.PickupPhone = a.Phone;
                model.PickupContact = a.Contact;
            }
            if (response.Addresses.Where(a => a.TypeAddress.ToLower() == "drop").Any())
            {
                var d = response.Addresses.Where(a => a.TypeAddress.ToLower() == "drop").FirstOrDefault();
                model.DropAddressId = d.Id;
                model.DeliveryStreet = d.Street;
                model.DeliveryCity = d.City;
                model.DeliveryState = d.State;
                model.DeliveryCountry = d.Country;
                model.DeliveryZipCode = d.ZipCode;
                model.DeliveryPhone = d.Phone;
                model.DeliveryContact = d.Contact;
            }


            return model;
        }

        [NonAction]
        public async Task<NewShipment> PrepareCustomerModel(NewShipment model)
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


            getUri = API.Common.GetAllPriorityTypes(_remoteServiceCommonUrl);
            dataString = await _apiClient.GetStringAsync(getUri);
            var priority = new List<SelectListItem>();
            priority.Add(new SelectListItem() { Value = null, Text = "Select a priority", Selected = true });

            gets = JArray.Parse(dataString);

            foreach (var brand in gets.Children<JObject>())
            {
                priority.Add(new SelectListItem()
                {
                    Value = brand.Value<string>("id"),
                    Text = brand.Value<string>("name")
                });
            }
            model.PriorityTypeList = priority;

            getUri = API.Common.GetAllPackageSizes(_remoteServiceCommonUrl);
            dataString = await _apiClient.GetStringAsync(getUri);
            var packageSize = new List<SelectListItem>();
            packageSize.Add(new SelectListItem() { Value = null, Text = "PackageSize", Selected = true });

            gets = JArray.Parse(dataString);

            foreach (var brand in gets.Children<JObject>())
            {
                packageSize.Add(new SelectListItem()
                {
                    Value = brand.Value<string>("id"),
                    Text = brand.Value<string>("name")
                });
            }
            model.PackageSizeList = packageSize;

            return model;
        }

        [AllowAnonymous]
        [NonAction]
        public async Task<CustomerModel> PrepareCustomerModel(CustomerModel model)
        {
            var getUri = API.Common.GetAllCustomerTypes(_remoteServiceCommonUrl);
            var dataString = await _apiClient.GetStringAsync(getUri);
            var CustomerTypes = new List<SelectListItem>();
            CustomerTypes.Add(new SelectListItem() { Value = null, Text = "Customer Type", Selected = true });

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
            customerStatus.Add(new SelectListItem() { Value = null, Text = "Customer Status", Selected = true });

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
            transportTypes.Add(new SelectListItem() { Value = null, Text = "transport Types", Selected = true });

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


            getUri = API.Common.GetAllPriorityTypes(_remoteServiceCommonUrl);
            dataString = await _apiClient.GetStringAsync(getUri);
            var priority = new List<SelectListItem>();
            priority.Add(new SelectListItem() { Value = null, Text = "Priority", Selected = true });

            gets = JArray.Parse(dataString);

            foreach (var brand in gets.Children<JObject>())
            {
                priority.Add(new SelectListItem()
                {
                    Value = brand.Value<string>("id"),
                    Text = brand.Value<string>("name")
                });
            }
            model.PriorityTypeList = priority;




            getUri = API.Common.GetAllPackageSizes(_remoteServiceCommonUrl);
            dataString = await _apiClient.GetStringAsync(getUri);
            var packageSize = new List<SelectListItem>();
            packageSize.Add(new SelectListItem() { Value = null, Text = "PackageSize", Selected = true });

            gets = JArray.Parse(dataString);

            foreach (var brand in gets.Children<JObject>())
            {
                packageSize.Add(new SelectListItem()
                {
                    Value = brand.Value<string>("id"),
                    Text = brand.Value<string>("name")
                });
            }
            model.PackageSizeList = packageSize;




        //    GetAllShippingStatus


            return model;
             
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
                    fileName = string.Format("uploads/img/{0}/{1}{2}", belong, extName, extension);

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
