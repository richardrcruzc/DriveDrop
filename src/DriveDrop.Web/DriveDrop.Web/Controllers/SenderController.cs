
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

            var getById = API.Sender.GetbyId(_remoteServiceBaseUrl, id ?? 0);


            var dataString = await _apiClient.GetStringAsync(getById, token);


            var response = JsonConvert.DeserializeObject<Customer>((dataString));




            return View(response);
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
        public async Task<IActionResult> NewSender()
        {
            CustomerModel c = new CustomerModel();
            await PrepareCustomerModel(c);
            c.CustomerTypeId = 2;
            return View(c);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewSender(CustomerModel c, List<IFormFile> files)
        {
            try
            {
                foreach (var state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
                {
                    var tt = state.Errors.ToString();
                }

                if (ModelState.IsValid)
                {
                    var token = await GetUserTokenAsync();

                    //try register new user

                    var addNewUserUri = API.Identity.RegisterUser(_remoteServiceIdentityUrl,c.UserEmail, c.Password);

                    var dataString = await _apiClient.GetStringAsync(addNewUserUri, token);

                    //var isCreated = JsonConvert.DeserializeObject<string>((dataString));

                    if (dataString == null)
                    {
                        await PrepareCustomerModel(c);
                        return View(c);
                    }

                    if (!dataString.Contains("IsAuthenticated") && !dataString.Contains("IsNotAuthenticated"))
                    {
                        await PrepareCustomerModel(c);
                        return View(c);
                    }

                    var user = _appUserParser.Parse(HttpContext.User);

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

                        c.FilePath = fileName;
                        
                    }

                    var addNewSenderUri = API.Sender.NewSender(_remoteServiceBaseUrl);

                    var response = await _apiClient.PostAsync(addNewSenderUri, c, token);

                    if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        throw new Exception("Error creating Shipping, try later.");
                    }

                    // return RedirectToAction("result", new { id = c.CustomerId });
                    return RedirectToAction("index","admin");
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
      
        public JsonResult ValidateUserName(string UserEmail)
        {
            return Json(!UserEmail.Equals("duplicate"));
        }

        private ActionResult Json(bool v, object allowGet)
        {
            throw new NotImplementedException();
        }

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

            return model;
        }
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


            return model;
             
        }

    }
}
