
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
using static DriveDrop.Web.Services.PasswordAdvisor;

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

        
            public async Task<IActionResult> InitializeReview(int id)
        {

            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var currenUserUri = API.Sender.GetByUserName(_remoteServiceBaseUrl, user.Email);
            var currentUserString = await _apiClient.GetStringAsync(currenUserUri, token);
            var currentUser = JsonConvert.DeserializeObject<CurrentCustomerModel>((currentUserString));
            if (currentUser == null)
            {
                return NotFound();
            }
            var package = currentUser.ShipmentSenders.Where(x => x.Id == id).FirstOrDefault();
            if (package == null)
            {
                return NotFound();
            }
            


                 var iUri = API.Rating.InitializeReview(_remoteServiceBaseUrl,id);
            var iString = await _apiClient.GetStringAsync(currenUserUri, token);
            var review = JsonConvert.DeserializeObject<ReviewModel>((iString));


            return RedirectToAction("ShippingDetails", new { id = package.Id }) ;
        }


        public async Task<IActionResult> ShippingDetails(int id)
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var currenUserUri = API.Sender.GetByUserName(_remoteServiceBaseUrl, user.Email);
            var currentUserString = await _apiClient.GetStringAsync(currenUserUri, token);
            var currentUser = JsonConvert.DeserializeObject<CurrentCustomerModel>((currentUserString));

            if (currentUser == null)
            {
                return NotFound();
            }
            var shipping = currentUser.ShipmentSenders.Where(x=>x.Id==id).FirstOrDefault();

            //var allnotassignedshipings = API.Shipping.GetById(_remoteServiceShippingsUrl, id);

            //var dataString = await _apiClient.GetStringAsync(allnotassignedshipings, token);

            //var shippings = JsonConvert.DeserializeObject<Shipment>((dataString));
            //if (shippings == null)
            //    return View(new Shipment());
            return View(shipping);

        }

        public async Task<IActionResult> Shippings(int id)
        {
            //call shipping api service
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var currenUserUri = API.Sender.GetByUserName(_remoteServiceBaseUrl, user.Email);
            var currentUserString = await _apiClient.GetStringAsync(currenUserUri, token);
            var currentUser = JsonConvert.DeserializeObject<CurrentCustomerModel>((currentUserString));

            if (currentUser == null)
            {
                return NotFound();
            }
            ViewBag.Id = currentUser.Id;
            var shippings = currentUser.ShipmentSenders;
            
            return View(shippings);
            
        }

     

        public async Task<IActionResult> Result(int? id)
        {

            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();
             
            var currenUserUri = API.Sender.GetByUserName(_remoteServiceBaseUrl, user.Email );
            var currentUserString = await _apiClient.GetStringAsync(currenUserUri, token);
            var currentUser = JsonConvert.DeserializeObject<CurrentCustomerModel>((currentUserString)); 

            if (currentUser == null)
            {
                return NotFound();
            }

              
            if (string.IsNullOrWhiteSpace(currentUser.PersonalPhotoUri))
                currentUser.PersonalPhotoUri = _settings.Value.CallBackUrl + "/images/profile-icon.png";


            currentUser.CustomerStatus = currentUser.CustomerStatus.ToTitleCase();
            return View(currentUser);
        }
 
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateInfo(CustomerInfoModel model, IFormFile photoUrl)
        {
            var result = "Info updated";
            if (ModelState.IsValid)
            {
                try
                {

                var fileName = await SaveFile(photoUrl, "sender");

                    if (!string.IsNullOrWhiteSpace(fileName))
                        model.PhotoUrl = fileName;
                    else
                        model.PhotoUrl = model.PersonalPhotoUri;

                var user = _appUserParser.Parse(HttpContext.User);
                var token = await GetUserTokenAsync();
                    var getUserUri = API.Sender.GetByUserName(_remoteServiceBaseUrl, user.Email);
                    var userString = await _apiClient.GetStringAsync(getUserUri, token);
                    var customer = JsonConvert.DeserializeObject<CurrentCustomerModel>(userString);
                    if (customer != null)
                    {
                        var updateInfo = API.Sender.UpdateInfo(_remoteServiceBaseUrl);

                        var response = await _apiClient.PostAsync(updateInfo, model, token);
                        if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                        {
                            //throw new Exception("Error creating Shipping, try later.");

                            ModelState.AddModelError("", "Error creating Shipping, try later.");

                        }
                        else
                            ModelState.AddModelError("", "Info Updated!");
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

            return RedirectToAction("result", new { id = model.Id });
        }

        public async Task<IActionResult> NewShipping(int id)
        {  
           

            var model = new NewShipment();
            await PrepareCustomerModel(model);
            model.CustomerId = id;

            await PrepareCustomerAddresses(model, id);


            ViewBag.PhotoUrl = _settings.Value.CallBackUrl + "/images/profile-icon.png";

            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewShipping(NewShipment c, IFormFile photoUrl)
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
                        ModelState.AddModelError("", "Select a Pickup Address");
                    if (string.IsNullOrEmpty(c.PickupPhone))
                        ModelState.AddModelError("", "Select a Pickup Phone");
                    if (string.IsNullOrEmpty(c.PickupContact))
                        ModelState.AddModelError("", "Select a Pickup Contact");
                     
                }
                 
                if (c.DropAddressId == 0)
                {

                    if (string.IsNullOrEmpty(c.DeliveryStreet))
                        ModelState.AddModelError("", "Select a Drop Address");
                    if (string.IsNullOrEmpty(c.DeliveryPhone))
                        ModelState.AddModelError("", "Select a Drop Phone");
                    if (string.IsNullOrEmpty(c.DeliveryContact))
                        ModelState.AddModelError("", "Select a Drop Contact");
                     
                }

                //if (photoUrl.Count() == 0)
                //{
                //    ModelState.AddModelError("", "Select package picture");
                //}
                if (c.PackageSizeId == 0)
                {
                    ModelState.AddModelError("", "Select Package Size");
                }

                if (c.PriorityTypeId == 0)
                { ModelState.AddModelError("", "Select Package Priority"); }
                 
                if (c.Amount == 0)
                {
                    ModelState.AddModelError("", "Select package value");
                }
                if (c.ShippingWeight == 0)
                {
                    ModelState.AddModelError("", "Select Package Weight");
                }
                var errors = ViewData.ModelState.Values.Count();
                if (errors == 0)
                {
                    var user = _appUserParser.Parse(HttpContext.User);
                    var token = await GetUserTokenAsync();

                    c.PickupPictureUri = await SaveFile(photoUrl, "Shipment");


                    var addNewShippingUri = API.Sender.SaveNewShipment(_remoteServiceShippingsUrl);

                    var response = await _apiClient.PostAsync(addNewShippingUri, c, token);
                    if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError || response.StatusCode==System.Net.HttpStatusCode.Unauthorized)
                    {
                        //throw new Exception("Error creating Shipping, try later.");
                        await PrepareCustomerModel(c);

                        ModelState.AddModelError("", "Error creating Shipping, try later.");
                        return View(c);
                    }

                    // try to process payment with  paypal
                    if (response.StatusCode == System.Net.HttpStatusCode.OK) 
                     return RedirectToAction("PostToPayPalAsync", new { item = "Charge per Shipping Service", amount =c.TotalCharge, customerId = c.CustomerId });
                    else
                        ModelState.AddModelError("", "Error creating Shipping, try later.");
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

             return await context.GetTokenAsync("access_token");
        }
        [AllowAnonymous]
        public IActionResult NewSender()
        {
            //await HttpContext.Authentication.SignOutAsync("Cookies");
            //await  HttpContext.Authentication.SignOutAsync("oidc");

            SenderRegisterModel c = new SenderRegisterModel();
            c.CustomerTypeId = 2;
            
            return View(c);
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewSender(SenderRegisterModel c) //,  List<IFormFile> imgeFoto)
        {
            try
            {
                foreach (var state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
                {
                    var tt = state.Errors.ToString();
                }
                if (c.ImgeFoto==null || c.ImgeFoto.Length <= 0)
                {
                    ModelState.AddModelError("", "Upload profile photo");
                    return View(c);
                }
                if (c.ImgeFoto.Length > 1048576)
                {
                    ModelState.AddModelError("", "profile photo file exceeds the file maximum size: 1MB");
                    return View(c);
                }              

                if (ModelState.IsValid)
                {
                    var user = _appUserParser.Parse(HttpContext.User);
                    var token = await GetUserTokenAsync();

                    //try register new user

                    var registerModel = new RegisterUserViewModel{userName = c.UserEmail , Password = c.Password };

                    var addNewUserUri = API.Identity.RegisterUser(_remoteServiceIdentityUrl, System.Net.WebUtility.UrlEncode(c.UserEmail), System.Net.WebUtility.UrlEncode(c.Password));

                    var dataString = await _apiClient.GetStringAsync(addNewUserUri, token);


                    //var userStatus = JsonConvert.DeserializeObject<object>((dataString));

                    if (string.IsNullOrWhiteSpace(dataString) || string.IsNullOrEmpty(dataString))
                    {
                        ModelState.AddModelError("", "Unable to register Login infomation user: " + c.UserEmail);
                        return View(c);
                    }
                    if (!dataString.Contains("IsAuthenticated") && !dataString.Contains("IsNotAuthenticated"))
                    {

                        ModelState.AddModelError("", "Unable to register Login infomation user: " + c.UserEmail);
                        return View(c);
                    }

                    var ppersonalUri = await SaveFile(c.ImgeFoto, "Sender");
                     
                    c.PersonalPhotoUri = ppersonalUri;


                    var addNewSenderUri = API.Sender.NewSender(_remoteServiceBaseUrl);

                    c.CustomerTypeId = 2;
                    c.Email = c.UserEmail;
                    var response = await _apiClient.PostAsync(addNewSenderUri, c, token);

                    if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        throw new Exception("Error creating sender, try later.");
                    }

                    // return RedirectToAction("result", new { id = c.CustomerId });
                    //return RedirectToAction("SignIn", "Account");
                    // var results = new NewSenderResult { Amount = "", Message="", UserName =c.UserEmail };

                    var callbackUrl = string.Empty;
                    var modfyMsg = string.Empty;
                    if (dataString.Contains("IsAuthenticated") || dataString.Contains("IsNotAuthenticated"))
                    {

                        dataString = dataString.Replace("IsAuthenticated", "");
                        callbackUrl = string.Format("{0}/{1}", _settings.Value.IdentityUrl.Trim(), dataString.Trim());
                        modfyMsg = string.Format("Hi {0} ! You have been sent this email because you created an account on our website. Please click on <a href =\"{1}\">this link</a> to confirm your email address is correct. ", c.FirstName, callbackUrl);



                        var message = new SendEmailModel
                        {
                            Subject = "Confirm Email Address for New Account",
                            UserName = c.UserEmail,
                            Message = modfyMsg
                        };

                        var sendEmailUri = API.Common.SendEmail(_remoteServiceCommonUrl);
                        var emailResponse = await _apiClient.PostAsync(sendEmailUri, message, token);

                    }


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


           // await PrepareCustomerModel(c);
            return View(c);

        }
        [AllowAnonymous]
        public async Task<IActionResult> NewSenderComplete()
        {
            //await HttpContext.Authentication.SignOutAsync("Cookies");
            //await  HttpContext.Authentication.SignOutAsync("oidc");

            CustomerModelComplete c = new CustomerModelComplete();
            await PrepareCustomerModel(c);
            c.CustomerTypeId = 2;
            c.Distance = 0;
            return View(c);
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewSenderComplete(CustomerModelComplete c, IFormFile packageImage, IFormFile imgeFoto)
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

        
 

        private ActionResult Json(bool v, object allowGet)
        {
            throw new NotImplementedException();
        }

         
        public async Task<IActionResult> PostToPayPalAsync(string item, string amount, int customerId)
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();
             
            var getUserUri = API.Sender.GetByUserName(_remoteServiceBaseUrl, user.Email);
            var userString = await _apiClient.GetStringAsync(getUserUri, token);
            var customer = JsonConvert.DeserializeObject<CurrentCustomerModel>(userString);
            if (customer != null)
            {
            }

            var lastShipment = customer.ShipmentSenders.OrderBy(x=>x.Id).LastOrDefault();
            if (lastShipment == null)
            { }

            ViewBag.CustomerId = customerId;

            var paypal = new Paypal();
            paypal.cmd = "_xclick";
            //paypal.cmd = "_cart";
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
            paypal.item_number = lastShipment.IdentityCode;
            paypal.invoice = lastShipment.IdentityCode;
            paypal.amount = lastShipment.ChargeAmount.ToString();
            paypal.price_per_item = lastShipment.ChargeAmount.ToString();
            paypal.discount = lastShipment.Discount.ToString();
            paypal.custom = lastShipment.DriverId.ToString();
            paypal.invoice = lastShipment.IdentityCode;
            paypal.tax = lastShipment.Tax.ToString();
            paypal.no_shipping = "2";
            paypal.rm = "2";
            paypal.no_note = "1";
            paypal.charset = "utf-8";

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
        [HttpPost]
        public IActionResult NotifyFromPaypal(string txn_id, string payment_date,
                                string payer_email, string payment_status,
                                string first_name, string last_name,
                                string item_number, string item_name,
                                string payer_id, string verify_sign)
        {
            var paypaltypes = item_name.Split('-');


            var result = item_number.Split('-');
            var userid = int.Parse(result[1]);
            var TransPaymentString = result[1].ToString() + result[0].ToString();
            var TransPayment = int.Parse(TransPaymentString);
            //var user = _context.Person.Include(p => p.Payments).Where(p => p.UserID == userid).Single();
            //var payment = user.Payments.Where(p => p.TransPaymentID == TransPayment).Single();

            //if (paypaltypes[0] == "Event")
            //{
            //    var eventid = int.Parse(result[0]);

            //    payment.PaymentReceipt = txn_id;
            //    payment.PaymentReceived = true;
            //    payment.PaymentReceivedDate = DateTime.Now;
            //    payment.PaymentNotes = payer_email + " " + first_name + " " + last_name + " " + item_number + " " + payer_id + " " + verify_sign + " " + item_name;

            //    _context.Payments.Update(payment);
            //    _context.SaveChanges();

            //    var userevent = _context.Person.Include(p => p.EventRegistry).Where(p => p.UserID == userid).Single();
            //    var eventreg = userevent.EventRegistry.Where(er => er.EventID == eventid).Single();
            //    eventreg.EventPaid = true;

            //    _context.EventRegistry.Update(eventreg);
            //    _context.SaveChanges();
            //    Response.StatusCode = (int)HttpStatusCode.OK;
            //    return Json("Json Result");

            //}

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

            var getById = API.Sender.GetByUserName(_remoteServiceBaseUrl, user.Email);

            var dataString = await _apiClient.GetStringAsync(getById, token);

            var response = JsonConvert.DeserializeObject<CurrentCustomerModel>((dataString));
            var addressesP = new List<SelectListItem>();
            var addressesD = new List<SelectListItem>();

            foreach (var a in response.Addresses)
            {
                var stringAddress = string.Format("{0}, {1}, {2}, {3}, {4} ",
                     a.Street, a.City, a.State, a.ZipCode, a.Country);

                if (a.TypeAddress.ToLower() == "pickup" || a.TypeAddress.ToLower() == "home")
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

            if (response.Addresses.Where(pa=>pa.TypeAddress.ToLower() == "pickup" || pa.TypeAddress.ToLower() == "home").Any()) {
              var a= response.Addresses.Where(p => p.TypeAddress.ToLower() == "pickup" || p.TypeAddress.ToLower() == "home").FirstOrDefault();
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
            priority.Add(new SelectListItem() { Value = null, Text = "Select Priority Shipping", Selected = true });

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
            packageSize.Add(new SelectListItem() { Value = null, Text = "Select a Package Size", Selected = true });

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
        public async Task<CustomerModelComplete> PrepareCustomerModel(CustomerModelComplete model)
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
        public async Task<string> SaveFile(IFormFile files, string belong)
        {

            Guid extName = Guid.NewGuid();
            //saving files
           // long size = files.Sum(f => f.Length);

            // full path to file in temp location
            var filePath = Path.GetTempFileName();
            var uploads = Path.Combine(_env.WebRootPath, string.Format("uploads\\img\\{0}", belong));
            var fileName = "";
            if (files == null)
                return "";
            var formFile = files;
           

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
            return fileName;

        }
       
    }
}
