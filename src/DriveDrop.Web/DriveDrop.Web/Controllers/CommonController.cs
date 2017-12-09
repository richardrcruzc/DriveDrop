using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.BuildingBlocks.Resilience.Http;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using DriveDrop.Web.Services;
using DriveDrop.Web.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using DriveDrop.Web.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using System.IO;
using static DriveDrop.Web.Services.PasswordAdvisor;

namespace DriveDrop.Web.Controllers
{
    [Authorize]
    public class CommonController : Controller
    {

       // private readonly IRatingRepository _redisRepository;

        private IHttpClient _apiClient;
        private readonly string _remoteServiceCommonUrl; 

        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly IHttpContextAccessor _httpContextAccesor;
        private readonly IIdentityParser<ApplicationUser> _appUserParser;
        private readonly string _remoteServiceRatingUrl;

        private readonly IHostingEnvironment _env;
        private IMemoryCache _cache;

        public CommonController(  IMemoryCache memoryCache, IOptionsSnapshot<AppSettings> settings, IHttpContextAccessor httpContextAccesor,
            IHttpClient httpClient, IIdentityParser<ApplicationUser> appUserParser,
            IHostingEnvironment env)
        {
//_redisRepository = redisRepository;
            _cache = memoryCache;
            _remoteServiceCommonUrl = $"{settings.Value.DriveDropUrl}/api/v1/common/"; 
            _remoteServiceRatingUrl = $"{settings.Value.DriveDropUrl}/api/v1/review/";

            _settings = settings;
            _httpContextAccesor = httpContextAccesor;
            _apiClient = httpClient;
            _appUserParser = appUserParser;

            _env = env;
        }

        [AllowAnonymous]
        [AcceptVerbs("Get", "Post")]
        public async Task<JsonResult> ValidateUserName(string UserEmail)
        {
            var validateUri = API.Common.ValidateUserName(_remoteServiceCommonUrl, UserEmail);

            var response = await _apiClient.GetStringAsync(validateUri);
            if(!response.Equals("duplicate"))
                return Json(data: true);
            else
                return Json(data: $"Email {UserEmail} is already in use.");

             
        }

        [AllowAnonymous]
        [AcceptVerbs("Get", "Post")]
        public   JsonResult ValidatePassword(string Password)
        {
            var message = string.Empty;
            PasswordScore score;
            var valid = PasswordAdvisor.ValidatePassword(Password, out message, out score);

            if (valid)
                return Json(data: true);

            return Json(data: $"{message} this is {score} password");

           


        }

        public FileResult GetFileFromDisk(string fileName, string name)
        {
            return File(fileName, "multipart/form-data", name);
        }
        //public FileResult Download()
        //{
        //    byte[] fileBytes = System.IO.File.ReadAllBytes(@"c:\folder\myfile.ext");
        //    string fileName = "myfile.ext";
        //    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        //}
        public FileResult DisplayPDF(string file)
        {
            return File( "/uploads/img/driver/cdd440a5-ca7a-4509-ab84-7038ce0f4f97.jpg", "application /jpg");            
        } 

        [HttpPost]
        public ActionResult ViewPDF(string file)
        {
            string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"500px\" height=\"300px\">";
            embed += "If you are unable to view file, you can download from <a href = \"{0}\">here</a>";
            embed += " or download <a target = \"_blank\" href = \"http://get.adobe.com/reader/\">Adobe PDF Reader</a> to view the file.";
            embed += "</object>";
            TempData["Embed"] = string.Format(embed, file);

            return Json("Index");
        }
        public async Task<IActionResult> WelcomeEmail(string UserName)
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var getUser = API.Common.GetUser(_remoteServiceCommonUrl, user.Email);
            var dataString = await _apiClient.GetStringAsync(getUser, token);
            var response = JsonConvert.DeserializeObject<CurrentCustomerModel>((dataString));
            if (response == null || !response.IsAdmin)
                return Json("Something wrong happened");
            
            var sendEmail = API.Common.WelcomeEmail(_remoteServiceCommonUrl, UserName);
            var sendString = await _apiClient.GetStringAsync(sendEmail, token);
            //var sendEmailResponse = JsonConvert.DeserializeObject<string>((sendString));


            return Json(sendString);
            //var allRatesUri = API.Rating.GetAllReviews
        }




        public async Task<IActionResult> SaveReview(int shippingId, string questionIdValues, string reviewed)
        {

 

            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var getUser = API.Common.GetUser(_remoteServiceCommonUrl, user.Email);
            var dataString = await _apiClient.GetStringAsync(getUser, token);
            var response = JsonConvert.DeserializeObject<CurrentCustomerModel>((dataString));
            if (response == null)
                return Json("Something wrong happened");



           var  model = new ReviewModel
            {
                Comment = "",
                // DriverId = shipping.Driver.Id,
                // SenderId = shipping.Sender.Id,
                Reviewed = reviewed,
                ShippingId = shippingId,
            };
            model.Details = new List<ReviewDetail>();

           var questionTmp = 0;
            var question = 0;
            var value = 0;

            var tmp = questionIdValues.Split(',');

            var tmp1 = tmp[0].Split('|');

            int.TryParse(tmp1[0].ToString(), out question);
            int.TryParse(tmp1[1].ToString(), out value);

            foreach (var questions in questionIdValues.Split(','))
            {
                var q = questions.Split('|');

                int.TryParse(q[0].ToString(), out questionTmp);

                if (question != questionTmp)
                    model.Details.Add(new ReviewDetail { ReviewQuestion = new ReviewQuestion { Id = question }, Values = value }); 

                int.TryParse(q[0].ToString(), out question);
                int.TryParse(q[1].ToString(), out value); 
                
            }

            if (question >0) 
                model.Details.Add(new ReviewDetail { ReviewQuestion = new ReviewQuestion { Id = question }, Values = value });


            var ratingUri = API.Rating.AddReviews(_remoteServiceRatingUrl);

            var ratinResponse = await _apiClient.PostAsync(ratingUri, model, token);
            if (ratinResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return Json("SomethingBadHappend");
            }


                return Json("RatingSaved");

        }
            public async Task<IActionResult> SaveReviewCache(int shippingId,int reviewed, int questionId, int value)
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var getUser = API.Common.GetUser(_remoteServiceCommonUrl, user.Email);
            var dataString = await _apiClient.GetStringAsync(getUser, token);
            var response = JsonConvert.DeserializeObject<CurrentCustomerModel>((dataString));
            if (response == null)
                return Json("Something wrong happened");

            //var shipping = response.ShipmentSenders.Where(x => x.Id == shippingId).FirstOrDefault();
            //if (shipping == null)
            //    shipping = response.ShipmentDrivers.Where(x => x.Id == shippingId).FirstOrDefault();
            //if (shipping == null)
            //    return "Something wrong happened";

            var model = new ReviewModel();

            string cache = string.Empty;
            string key = string.Format("rating_{0}_{1}", shippingId, reviewed );


              model = _cache.Get<ReviewModel>(key);
            if (model != null)
            {
                var index = model.Details.FindIndex(x => x.ReviewQuestion.Id == questionId);
                if (index < 0)
                    model.Details.Add(new ReviewDetail { ReviewQuestion = new ReviewQuestion { Id = questionId }, Values = value });
                else
                {
                    model.Details[index].Values = value;
                  
                }
                _cache.Remove(key);
            }



            model = await
        _cache.GetOrCreateAsync(key, entry =>
        {

            entry.SlidingExpiration = TimeSpan.FromMinutes(30);
            //return Task.FromResult(DateTime.Now);

            model = new ReviewModel
            {
                Comment = "",
                // DriverId = shipping.Driver.Id,
                // SenderId = shipping.Sender.Id,
                Reviewed = reviewed == 1 ? "driver" : "sender",
                ShippingId = shippingId,
            };
            model.Details.Add(new ReviewDetail { ReviewQuestion = new ReviewQuestion { Id = questionId }, Values = value });
            return Task.FromResult(model);

        });



            //// Look for cache key.
            //if (!_cache.TryGetValue(key, out cache))
            //{
            //    cache = string.Format("rating_{0}_{1}_{2}_{3}", shippingId, reviewed, questionId, value);

            //    // Key not in cache, so get data.
            //    //model = new ReviewModel
            //    //{
            //    //    Comment = "",
            //    //    // DriverId = shipping.Driver.Id,
            //    //    // SenderId = shipping.Sender.Id,
            //    //    Reviewed = reviewed == 1 ? "driver" : "sender",
            //    //    ShippingId = shippingId,
            //    //};
            //    //model.Details.Add(new ReviewDetail { ReviewQuestion = new ReviewQuestion { Id = questionId }, Values = value });

            //    // Set cache options.
            //    var cacheEntryOptions = new MemoryCacheEntryOptions()
            //        // Keep in cache for this time, reset time if accessed.
            //        .SetSlidingExpiration(TimeSpan.FromMinutes(15));

            //    // Save data in cache.
            //    _cache.Set(key, key, cacheEntryOptions);
            //}


            //var pp = cache;

            //try
            //{
            //    var exist = await _redisRepository.GetAsync(model.Id);
            //    if (exist != null)
            //    {
            //        await _redisRepository.DeleteAsync(model.Id);
            //    }

            //    model = await _redisRepository.UpdateAsync(model);
            //}
            //catch (Exception ex)
            //{
            //    var mm = ex;
            //    var m = mm.Message;

            //}
            return Json("Created");
            //var allRatesUri = API.Rating.GetAllReviews
        }
        public async Task<string> GetReviewCache()
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var getUser = API.Common.GetUser(_remoteServiceCommonUrl, user.Email);
            var dataString = await _apiClient.GetStringAsync(getUser, token);
            var response = JsonConvert.DeserializeObject<CurrentCustomerModel>((dataString));
            if(response==null)
                return "Something wrong happened";


            var model = new ReviewModel
            {
                 Comment="",
                  

            };



            return "Created";
            //var allRatesUri = API.Rating.GetAllReviews
        }

        public async Task<IActionResult> AddressAdd(int id)
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var getUser = API.Common.GetUser(_remoteServiceCommonUrl, user.Email);

            var dataString = await _apiClient.GetStringAsync(getUser, token);

            var response = JsonConvert.DeserializeObject<CurrentCustomerModel>((dataString)); 

            ViewBag.Id = response.Id;
            var model = new AddressModel { CustomerId = response.Id };


            ViewBag.CustomerType = response.CustomerTypeId;

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

            var updateInfo = API.Common.AddAddress(_remoteServiceCommonUrl);

            var response = await _apiClient.PostAsync(updateInfo, model, token);
            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                //throw new Exception("Error creating Shipping, try later.");

                ModelState.AddModelError("", "Error creating Shipping, try later.");
                result = "Error creating Shipping, try later.";
            }


            return result;
        }

        public async Task<IActionResult> Addresses(int id)
        {
            ViewBag.Id = id;
            //call shipping api service
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();
            
            var getUser = API.Common.GetUser(_remoteServiceCommonUrl, user.Email);

            var dataString = await _apiClient.GetStringAsync(getUser, token);

            var response = JsonConvert.DeserializeObject<CurrentCustomerModel>((dataString));

             

            var listAddresses = new List<AddressModel>();
            foreach (var a in response.Addresses)
            {
                //if (response.DefaultAddress!=null && response.DefaultAddress.Id == a.Id)
                //    a.TypeAddress = "default";
                listAddresses.Add(a);
            }

            ViewBag.CustomerType = response.CustomerTypeId;


            return View(listAddresses);

        }



        //[HttpPost]
        // [ValidateAntiForgeryToken]
        public async Task<string> DefaultAddress(int id)
        {
            var result = "Address updated";

            //call shipping api service
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var getUser = API.Common.GetUser(_remoteServiceCommonUrl, user.Email);

            var dataString = await _apiClient.GetStringAsync(getUser, token);

            var customer = JsonConvert.DeserializeObject<CurrentCustomerModel>((dataString));
            if (customer == null)
                return "Address Not Found";
            ViewBag.Id = customer.Id;

            var defaultAddress = customer.Addresses.Where(x => x.Id == id).FirstOrDefault();

            if (defaultAddress == null)
                return "Address Not Found";


            var updateInfo = API.Common.DefaultAddress(_remoteServiceCommonUrl, customer.Id, defaultAddress.Id);

            var dataString1 = await _apiClient.GetStringAsync(updateInfo,  token);
              result = JsonConvert.DeserializeObject<string>((dataString1)); 

            return result;
        }
         [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<string> AddressDelete(int id)
        {
            var result = "Address deleted";

            //call shipping api service
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var getById = API.Common.GetUser(_remoteServiceCommonUrl, user.Email);

            var dataString = await _apiClient.GetStringAsync(getById, token);

            var customer = JsonConvert.DeserializeObject<Customer>((dataString));
            if (customer == null)
                return "Address Not Found";
            ViewBag.Id = customer.Id;

            var addressToDelete = customer.Addresses.Where(x => x.Id == id).FirstOrDefault();

            if (addressToDelete == null)
                return "Address Not Found";

            if (addressToDelete.TypeAddress.ToLower() == "home" || addressToDelete.Id == customer.DefaultAddress.Id)
                return "Cannot delete default address";


            var updateInfo = API.Common.DeleteAddress(_remoteServiceCommonUrl, customer.Id,addressToDelete.Id);

            var response = await _apiClient.PostAsync(updateInfo, addressToDelete, token);
            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                //throw new Exception("Error creating Shipping, try later.");

                ModelState.AddModelError("", "Error deleting address, try later.");
                result = "Error deleting address, try later.";
            }

            return result;
        }
        async Task<string> GetUserTokenAsync()
        {
            var context = _httpContextAccesor.HttpContext;

             return await context.GetTokenAsync("access_token");
        }
    }
}