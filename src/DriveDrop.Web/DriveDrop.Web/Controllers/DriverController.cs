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
        [AllowAnonymous]
        public async Task<IActionResult> NewDriver()
        {

            var model = new DriverModel();

            await PrepareCustomerModel(model);


            return View(model);
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> NewDriver(DriverModel c, List<IFormFile> personalfiles, List<IFormFile> licensefiles, List<IFormFile> Vehiclefiles, List<IFormFile> insurancefiles)
        {
            try
            { 

                foreach (var state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
                {
                    var tt = state.Errors.ToString();
                }

                if (ModelState.IsValid)
                { 

                    //try register new user

                    var addNewUserUri = API.Identity.RegisterUser(_remoteServiceIdentityUrl, c.UserEmail, c.Password);

                    var dataString = await _apiClient.GetStringAsync(addNewUserUri);

                    // var isCreated = JsonConvert.DeserializeObject<string>((dataString));

                    if (dataString == null)
                    {
                        await PrepareCustomerModel(c);
                        return View(c);
                    }
                        if(!dataString.Contains("IsAuthenticated") && !dataString.Contains("IsNotAuthenticated"))
                    {
                        await PrepareCustomerModel(c);
                        return View(c);
                    }

                   c.PersonalPhotoUri =await SaveFile(personalfiles, "driver");
                    c.DriverLincensePictureUri = await SaveFile(licensefiles, "driver");
                   c.VehiclePhotoUri = await SaveFile(Vehiclefiles, "driver");
                   c.InsurancePhotoUri = await SaveFile(insurancefiles, "driver");
                    
                    var user = _appUserParser.Parse(HttpContext.User);
                    var token = await GetUserTokenAsync();

                    var addNewDriverUri = API.Driver.NewDriver(_remoteServiceDriversUrl);

                    var response = await _apiClient.PostAsync(addNewDriverUri, c, token);

                    if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        throw new Exception("Error creating Shipping, try later.");
                    }

                    return RedirectToAction("index", "admin");

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


        public async Task<IActionResult> Result(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
             

            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

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
         


        public async Task<IActionResult> AssignDriver(int id, int shipingId)
        {
             
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var assign = API.Driver.AssignDriver(_remoteServiceDriversUrl, id, shipingId);


            var dataString = await _apiClient.GetStringAsync(assign, token);


            //var response = JsonConvert.DeserializeObject<CustomerViewModel>((dataString));



            return RedirectToAction("Result", new { id = id });

        }

        public async Task<IActionResult> UpdatePackageStatus(int id,int customerId )
        {

            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var assign = API.Shipping.UpdatePackageStatus(_remoteServiceShippingsUrl, id);


            var dataString = await _apiClient.GetStringAsync(assign, token);


            //var response = JsonConvert.DeserializeObject<CustomerViewModel>((dataString));



            return RedirectToAction("Result", new { id = customerId });

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

        public IActionResult ValidateUserName(string UserEmail)
        {
            return Json(!UserEmail.Equals("duplicate") );
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
