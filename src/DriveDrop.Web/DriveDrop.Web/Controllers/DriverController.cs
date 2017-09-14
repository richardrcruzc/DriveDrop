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
    public class DriverController : Controller
    {
        private IHttpClient _apiClient;
        private readonly string _remoteServiceBaseUrl;
        private readonly string _remoteServiceCommonUrl;
        private readonly string _remoteServiceShippingsUrl;
        private readonly string _remoteServiceDriversUrl;

        private readonly string _remoteServiceIdentityUrl;

        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly IHttpContextAccessor _httpContextAccesor;
        private readonly IIdentityParser<ApplicationUser> _appUserParser;

        private readonly IHostingEnvironment _env;

        public DriverController(IOptionsSnapshot<AppSettings> settings, IHttpContextAccessor httpContextAccesor,
            IHttpClient httpClient, IIdentityParser<ApplicationUser> appUserParser, IHostingEnvironment env)
        {
            _remoteServiceCommonUrl = $"{settings.Value.DriveDropUrl}/api/v1/common/";
            _remoteServiceBaseUrl = $"{settings.Value.DriveDropUrl}/api/v1/sender";
            _remoteServiceShippingsUrl = $"{settings.Value.DriveDropUrl}/api/v1/shippings";
            _remoteServiceDriversUrl = $"{settings.Value.DriveDropUrl}/api/v1/drivers";
            _remoteServiceIdentityUrl = $"{settings.Value.IdentityUrl}/account/";
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


        
            public async Task<IActionResult> Canceled(int id)
        {
            @ViewBag.CustomerId = id;
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var allnotassignedshipings = API.Shipping.GetByDriverIdAndStatusId(_remoteServiceShippingsUrl, id, 5);

            var dataString = await _apiClient.GetStringAsync(allnotassignedshipings, token);

            var shippings = JsonConvert.DeserializeObject<PaginatedShippings>((dataString));
            if (shippings == null)
                return View(new PaginatedShippings ());

            shippings.ShippingStatusList = await PrepareShippingStatus();

            return View(shippings);
        }
        public async Task<IActionResult> Deliver(int id)
        {
            @ViewBag.CustomerId = id;
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var allnotassignedshipings = API.Shipping.GetByDriverIdAndStatusId(_remoteServiceShippingsUrl, id, 4);

            var dataString = await _apiClient.GetStringAsync(allnotassignedshipings, token);

            var shippings = JsonConvert.DeserializeObject<PaginatedShippings>((dataString));
            if (shippings == null)
                return View(new PaginatedShippings { Count = 0, Data = new List<Shipment>(), PageIndex = 0, PageSize = 0 });

            shippings.ShippingStatusList = await PrepareShippingStatus();

            return View(shippings);
        }
        public async Task<IActionResult> DeliveryInProcess(int id)
        {
            @ViewBag.CustomerId = id;
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var allnotassignedshipings = API.Shipping.GetByDriverIdAndStatusId(_remoteServiceShippingsUrl, id, 3);

            var dataString = await _apiClient.GetStringAsync(allnotassignedshipings, token);

            var shippings = JsonConvert.DeserializeObject<PaginatedShippings>((dataString));
            if (shippings == null)
                return View(new PaginatedShippings { Count = 0, Data = new List<Shipment>(), PageIndex = 0, PageSize = 0 });

            shippings.ShippingStatusList = await PrepareShippingStatus();

            return View(shippings);
        }

        public async Task<IActionResult> PickedUp(int id)
        {
            @ViewBag.CustomerId = id;
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var allnotassignedshipings = API.Shipping.GetByDriverIdAndStatusId(_remoteServiceShippingsUrl, id,2);

            var dataString = await _apiClient.GetStringAsync(allnotassignedshipings, token);

            var shippings = JsonConvert.DeserializeObject<PaginatedShippings>((dataString));
            if (shippings == null)
                return View(new PaginatedShippings { Count=0, Data= new List<Shipment>(), PageIndex=0, PageSize=0 });

            shippings.ShippingStatusList = await PrepareShippingStatus();

            return View(shippings);
        }

        public async Task<IActionResult> PendingPickUp(int id)
        {
            @ViewBag.CustomerId = id;
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var allnotassignedshipings = API.Shipping.GetByDriverIdAndStatusId(_remoteServiceShippingsUrl, id, 1);

            var dataString = await _apiClient.GetStringAsync(allnotassignedshipings, token);

            var shippings = JsonConvert.DeserializeObject<PaginatedShippings>((dataString));
            if (shippings == null)
                return View(new PaginatedShippings());

            shippings.ShippingStatusList =await PrepareShippingStatus();

            return View(shippings);
        }
        public async Task<IActionResult> ReadyToPickUp(int id)
        {

            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();


            var getUserUri = API.Driver.GetByUserName(_remoteServiceDriversUrl, user.Email);
            var userString = await _apiClient.GetStringAsync(getUserUri, token);
            var customer = JsonConvert.DeserializeObject<CurrentCustomerModel>(userString);
            if (customer == null)
                return View(new PaginatedShippings());

            @ViewBag.CustomerId = id;
            //call shipping api service
            
            var allnotassignedshipings = API.Shipping.GetPackagesReadyForDriver(_remoteServiceShippingsUrl, customer.Id);

            var dataString = await _apiClient.GetStringAsync(allnotassignedshipings, token);

            var shippings = JsonConvert.DeserializeObject<PaginatedShippings>((dataString));
            if (shippings == null)
                return View(new PaginatedShippings());
            shippings.ShippingStatusList = await PrepareShippingStatus();

            shippings.DeliverDistance = customer.DeliverRadius??0;
            shippings.PickupDistance = customer.PickupRadius??0;

            return View(shippings);
        }

        [AllowAnonymous]
        public async Task<IActionResult> NewDriver()
        {

            var model = new DriverModel();

            await PrepareCustomerModel(model);


            return View(model);
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> NewDriver(DriverModel c) //, List<IFormFile> personalfiles, List<IFormFile> licensefiles, List<IFormFile> Vehiclefiles, List<IFormFile> insurancefiles
        {
            try
            {
                c.Email = c.UserEmail;

                foreach (var state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
                {

                    var tt = state.Errors.ToString();
                   // ModelState.AddModelError("", state.Errors[0].ErrorMessage);
                }

                if (ModelState.IsValid)
                { 

                    //try register new user

                    var addNewUserUri = API.Identity.RegisterUser(_remoteServiceIdentityUrl, c.UserEmail, c.Password);

                    var dataString = await _apiClient.GetStringAsync(addNewUserUri);

                    // var isCreated = JsonConvert.DeserializeObject<string>((dataString));

                    if (dataString == null)
                    {
                        ModelState.AddModelError("", "Unable to register user 1");
                        await PrepareCustomerModel(c);
                        return View(c);
                    }
                        if(!dataString.Contains("IsAuthenticated") && !dataString.Contains("IsNotAuthenticated"))
                    {
                        ModelState.AddModelError("", "Unable to register user 2");
                        await PrepareCustomerModel(c);
                        return View(c);
                    }

                   c.PersonalPhotoUri =await SaveFile(c.Personalfiles, "driver");
                    c.DriverLincensePictureUri = await SaveFile(c.Licensefiles, "driver");
                   c.VehiclePhotoUri = await SaveFile(c.Vehiclefiles, "driver");
                   c.InsurancePhotoUri = await SaveFile(c.Insurancefiles, "driver");
                    
                    var user = _appUserParser.Parse(HttpContext.User);
                    var token = await GetUserTokenAsync();

                    var addNewDriverUri = API.Driver.NewDriver(_remoteServiceDriversUrl);

                    var response = await _apiClient.PostAsync(addNewDriverUri, c, token);

                    if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        //throw new Exception("Error creating Shipping, try later.");
                        ModelState.AddModelError("", "Error creating Shipping, try later.");
                        await PrepareCustomerModel(c);
                        return View(c);
                    }


                    return RedirectToAction("NewDriverResults", new { user = c.UserEmail });

                    //return RedirectToAction("index", "admin");

                    // response.EnsureSuccessStatusCode(); 

                    // return RedirectToAction("result", new { id = c.CustomerId });
                    //  return CreatedAtAction(nameof(Result), new {  }, null);
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
        [AllowAnonymous]
        public IActionResult NewDriverResults(string user)
        {
            return View("NewDriverResults", user);
        }
        
        public async Task<IActionResult> DashBoard(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
             

            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync(); 

            var idIsUserUri = API.Common.IdIsUser(_remoteServiceCommonUrl, user.Email, id??0);

            var idIsUserdataString = await _apiClient.GetStringAsync(idIsUserUri, token);

            var idIsUserresponse = JsonConvert.DeserializeObject<bool>((idIsUserdataString));

            if (!idIsUserresponse)
            {
                return NotFound();
            }


            var getById = API.Driver.GetbyId(_remoteServiceDriversUrl, id ?? 0);

             var dataString = await _apiClient.GetStringAsync(getById, token);

             var response = JsonConvert.DeserializeObject<Customer>((dataString));

            response.DriverLincensePictureUri = string.Format("{0}{1}",_settings.Value.CallBackUrl, response.DriverLincensePictureUri);
            ViewBag.DriverId = id;

            ViewBag.Uri = _settings.Value.CallBackUrl;

            return View(response);


            //var model = new Customer();
            //model.FirstName = getById;
            //model.Id = 9;
            //return View(model);
        }


        public async Task<IActionResult> Vehicle(int? id)
        {
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


            var getById = API.Driver.GetbyId(_remoteServiceDriversUrl, id ?? 0);

            var dataString = await _apiClient.GetStringAsync(getById, token);

            var response = JsonConvert.DeserializeObject<Customer>((dataString));

            //response.DriverLincensePictureUri = string.Format("{0}{1}", _settings.Value.CallBackUrl, response.DriverLincensePictureUri);
            ViewBag.DriverId = id;

            ViewBag.Uri = _settings.Value.CallBackUrl;
            var model = new VehicleInfoModel
            {
                Id = response.Id,
                VehicleMake = response.VehicleMake,
                VehicleModel=response.VehicleModel,
                VehicleColor=response.VehicleColor,
                VehicleYear = response.VehicleYear,

                DriverLincensePictureUri = _settings.Value.CallBackUrl + "/" + response.DriverLincensePictureUri,          
                vehiclePhotoUri = _settings.Value.CallBackUrl + "/" + response.VehiclePhotoUri,
                insurancePhotoUri = _settings.Value.CallBackUrl + "/" + response.InsurancePhotoUri,
                
                TransportTypeId = response.TransportTypeId,
                TransportType = response.TransportType,

            };

            var tmp = new DriverModel();
            tmp = await PrepareCustomerModel(tmp);

            model.TransportTypeList = tmp.TransportTypeList;

            return View(model);


            //var model = new Customer();
            //model.FirstName = getById;
            //model.Id = 9;
            //return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<string> UpdateVehicleInfo(VehicleInfoModel model )
        {
            var result = "Info updated";
            if (ModelState.IsValid)
            {
                try
                {

                    //var fileName = await SaveFile(photoUrl, "driver");

                    //if (!string.IsNullOrWhiteSpace(fileName))
                    //    model.PhotoUrl = fileName;

                    var user = _appUserParser.Parse(HttpContext.User);
                    var token = await GetUserTokenAsync();

                    var updateInfo = API.Driver.UpdateInfo(_remoteServiceBaseUrl);

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
        public async Task<IActionResult> Result(int? id)
        {
            if (id == null)
            {
                return NotFound("Invalid entry");
            }

            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var currenUserUri = API.Driver.GetByUserName(_remoteServiceDriversUrl, user.Email);
            var currentUserString = await _apiClient.GetStringAsync(currenUserUri, token);
            var currentUser = JsonConvert.DeserializeObject<CurrentCustomerModel>((currentUserString));

            if (currentUser == null)
            {
                return NotFound();
            }


            if (string.IsNullOrWhiteSpace(currentUser.PersonalPhotoUri))
                currentUser.PersonalPhotoUri = _settings.Value.CallBackUrl + "/images/DefaultProfileImage.png";

            currentUser.CustomerStatus=  currentUser.CustomerStatus.ToTitleCase();

            return View(currentUser);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<string> UpdateInfo(CustomerInfoModel model, List<IFormFile> PersonalPhotoUri)
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

                    var getUserUri = API.Driver.GetByUserName(_remoteServiceDriversUrl, user.Email);
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

        

        public async Task<IActionResult> AssignDriver(int shipingId)
        {
             
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var getUserUri = API.Driver.GetByUserName(_remoteServiceDriversUrl, user.Email);
            var userString = await _apiClient.GetStringAsync(getUserUri, token);
            var customer = JsonConvert.DeserializeObject<CurrentCustomerModel>(userString);

            var assign = API.Driver.AssignDriver(_remoteServiceDriversUrl, customer.Id, shipingId);


            var dataString = await _apiClient.GetStringAsync(assign, token);


            //var response = JsonConvert.DeserializeObject<CustomerViewModel>((dataString));



            return RedirectToAction("PendingPickUp", new { id = customer.Id });

        }

        public async Task<IActionResult> UpdatePackageStatus(UpdatePackageStatusModel model )
        {
            int shippingId = model.ShippingId;
            int shippingStatusId = model.Item.ShippingStatusId;

            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var getUserUri = API.Driver.GetByUserName(_remoteServiceDriversUrl, user.Email);
            var userString = await _apiClient.GetStringAsync(getUserUri, token);
            var customer = JsonConvert.DeserializeObject<CurrentCustomerModel>(userString);

            var assign = API.Shipping.UpdatePackageStatus(_remoteServiceShippingsUrl, shippingId, shippingStatusId);


                var dataString = await _apiClient.GetStringAsync(assign, token);
            

            return RedirectToAction("PendingPickUp", new { id = customer.Id });

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

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ChangeShippingStatus(int customerId, int shipingId, int ShippingStatusId)
        //{
        //    var shipping = _context.Shipments.Find(shipingId);

        //    if (shipping == null)
        //        return RedirectToAction("Result", new { id = customerId });

        //    var driver = _context.Customers.Find(customerId);
        //    if (driver == null)
        //        return RedirectToAction("Result", new { id = customerId });

        //    shipping.ChangeStatus(ShippingStatusId);
        //    _context.Update(shipping);
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction("Result", new { id = customerId });
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken] 
        //public async Task<IActionResult> New(DriverModel c, List<IFormFile> files)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var defaultAddres = new Address(c.DeliveryStreet, c.DeliveryCity, "WA", "USA", c.DeliveryZipCode, c.DeliveryPhone, c.DeliveryContact, 0, 0);
        //        //    var pickUpAddres = new Address(c.PickupStreet, c.PickupCity, "WA", "USA", c.PickupZipCode, c.PickupPhone, c.PickupContact, 0, 0);

        //            var tmpUser = Guid.NewGuid().ToString();

        //            var newCustomer = new Customer(tmpUser, c.FirstName, c.LastName, c.TransportTypeId, CustomerStatus.WaitingApproval.Id, c.Email, c.Phone, CustomerType.Driver.Id,c.MaxPackage??0,c.PickupRadius ?? 0, c.DeliverRadius ?? 0);

        //            _context.Add(newCustomer);
        //              _context.SaveChanges();

        //            newCustomer.AddDefaultAddress(defaultAddres);
        //            _context.Update(newCustomer); 


        //            await _context.SaveChangesAsync();

        //            Guid extName = Guid.NewGuid() ;
        //            //saving files
        //            long size = files.Sum(f => f.Length);

        //            // full path to file in temp location
        //            var filePath = Path.GetTempFileName();
        //            var uploads = Path.Combine(_env.WebRootPath, "uploads\\img\\driver");
        //            var fileName = "";

        //            foreach (var formFile in files)
        //            {

        //                if (formFile.Length > 0)
        //                {
        //                    var extension = ".jpg";
        //                        if(formFile.FileName.ToLower().EndsWith(".jpg"))
        //                        extension = ".jpg";
        //                    if (formFile.FileName.ToLower().EndsWith(".tif"))
        //                        extension = ".tif";
        //                    if (formFile.FileName.ToLower().EndsWith(".png"))
        //                        extension = ".png";
        //                    if (formFile.FileName.ToLower().EndsWith(".gif"))
        //                        extension = ".gif";




        //                    filePath = string.Format("{0}\\{1}{2}", uploads, extName, extension);
        //                    fileName = string.Format("uploads\\img\\driver\\{0}{1}",  extName, extension);

        //                    using (var stream = new FileStream(filePath, FileMode.Create))
        //                    {
        //                        await formFile.CopyToAsync(stream);
        //                    }
        //                }
        //            }
        //            if (!string.IsNullOrWhiteSpace(fileName))
        //            {
        //                newCustomer.AddPicture(fileName);
        //                _context.Update(newCustomer);
        //                await _context.SaveChangesAsync();
        //            }

        //            return CreatedAtAction(nameof(Result), new { id = newCustomer.Id }, null);
        //        }
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        //Log the error (uncomment ex variable name and write a log.
        //        var error = string.Format("Unable to save changes. " +
        //            "Try again, and if the problem persists " +
        //            "see your system administrator. {0}", ex.Message);

        //        ModelState.AddModelError("", error);
        //    }

        //    PrepareCustomerModel(c);
        //    return View(c);
        //}




        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> SaveNewShipment(NewShipment c) 
        //{
        //    try
        //    {


        //        foreach (var state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
        //        {
        //            var tt = state.Errors.ToString();
        //        }

        //        if (ModelState.IsValid)
        //        {

        //            var sender = _context.Customers.Find(c.CustomerId);


        //            var deliveryAddres = new Address(c.DeliveryStreet, c.DeliveryCity, "WA", "USA", c.DeliveryZipCode, c.DeliveryPhone, c.DeliveryContact, 0, 0);
        //            var pickUpAddres = new Address(c.PickupStreet, c.PickupCity, "WA", "USA", c.PickupZipCode, c.PickupPhone, c.PickupContact, 0, 0);

        //            var tmpUser = Guid.NewGuid().ToString();                     



        //            var shipment = new Shipment(pickUpAddres, deliveryAddres, sender, 0, 0, c.PriorityTypeId, c.PriorityTypeLevel, c.TransportTypeId, c.Note, "", "");
        //            _context.Add(shipment);

        //            _context.SaveChanges();

        //            await _context.SaveChangesAsync();


        //            return RedirectToAction("result", new { id = sender.Id });
        //            //return CreatedAtAction(nameof(Result), new { id = sender.Id }, null);
        //        }
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        //Log the error (uncomment ex variable name and write a log.
        //        var error = string.Format("Unable to save changes. " +
        //            "Try again, and if the problem persists " +
        //            "see your system administrator. {0}", ex.Message);

        //        ModelState.AddModelError("", error);
        //    }
        //    var customer = await _context.Customers.FirstOrDefaultAsync();

        //    PrepareCustomerModel(c);

        //    return ViewComponent("NewShipment", c);

        //    //return View(c);
        //}
        [AllowAnonymous]
        public async Task<JsonResult> ValidateUserName(string UserEmail)
        {
            var validateUri = API.Common.ValidateUserName(_remoteServiceCommonUrl, UserEmail);

            var response = await _apiClient.GetStringAsync(validateUri);

            return Json(!response.Equals("duplicate"));
        }

        public async Task<List<SelectListItem>>  PrepareShippingStatus()
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


            private ActionResult Json(bool v, object allowGet)
        {
            throw new NotImplementedException();
        }

        public async Task<DriverModel> PrepareCustomerModel(DriverModel model)
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
