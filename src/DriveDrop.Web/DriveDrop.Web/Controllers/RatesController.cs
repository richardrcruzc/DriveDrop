using DriveDrop.Web.Infrastructure;
using DriveDrop.Web.Services;
using DriveDrop.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.eShopOnContainers.BuildingBlocks.Resilience.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Web.Controllers
{ 
    [Authorize]
    public class RatesController : Controller
    { 
        private IHttpClient _apiClient;
        private readonly string _remoteServiceBaseUrl;
        private readonly string _remoteServiceCommonUrl;
        private readonly string _remoteServiceShippingsUrl;
        private readonly string _remoteServiceRatessUrl;
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly IHttpContextAccessor _httpContextAccesor;
        private readonly IIdentityParser<ApplicationUser> _appUserParser;

        private readonly IHostingEnvironment _env;


        public RatesController(IOptionsSnapshot<AppSettings> settings, IHttpContextAccessor httpContextAccesor,
            IHttpClient httpClient, IIdentityParser<ApplicationUser> appUserParser,
            IHostingEnvironment env)
        {
            _remoteServiceCommonUrl = $"{settings.Value.DriveDropUrl}/api/v1/common/";
            _remoteServiceBaseUrl = $"{settings.Value.DriveDropUrl}/api/v1/sender";
            _remoteServiceShippingsUrl = $"{settings.Value.DriveDropUrl}/api/v1/shippings";
            _remoteServiceRatessUrl = $"{settings.Value.DriveDropUrl}/api/v1/rates/";
            _settings = settings;
            _httpContextAccesor = httpContextAccesor;
            _apiClient = httpClient;
            _appUserParser = appUserParser;

            _env = env;

        }

        public async Task<IActionResult> ListTaxes()
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var isAdminUri = API.Common.IsAdmin(_remoteServiceCommonUrl, user.Email);
            var isAdminString = await _apiClient.GetStringAsync(isAdminUri, token);
            var isAdminResponse = JsonConvert.DeserializeObject<bool>(isAdminString);

            if (!isAdminResponse)
                return RedirectToAction("index", "home");

            var taxUri = API.Tax.Get(_remoteServiceRatessUrl );
            var taxString = await _apiClient.GetStringAsync(taxUri, token);
            var response = JsonConvert.DeserializeObject<List<TaxModel>>(taxString);

            return View(response);
        }

        public async Task<IActionResult> AddTax()
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var isAdminUri = API.Common.IsAdmin(_remoteServiceCommonUrl, user.Email);
            var isAdminString = await _apiClient.GetStringAsync(isAdminUri, token);
            var isAdminResponse = JsonConvert.DeserializeObject<bool>(isAdminString);

            if (!isAdminResponse)
                return RedirectToAction("index", "home"); 
           

            return View(new TaxModel());

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTax(TaxModel model)
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();
            var isAdminUri = API.Common.IsAdmin(_remoteServiceCommonUrl, user.Email);
            var isAdminString = await _apiClient.GetStringAsync(isAdminUri, token);
            var isAdminResponse = JsonConvert.DeserializeObject<bool>(isAdminString);

            if (!isAdminResponse)
                return NotFound("Invalid entry");

            //if (ModelState.IsValid)
            //{
            try
            {

                var taxUri = API.Tax.SaveTax(_remoteServiceRatessUrl);

                var response = await _apiClient.PostAsync(taxUri, model, token);

                if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    //throw new Exception("Error creating Shipping, try later.");

                    ModelState.AddModelError("", "Error updating tax table, try later.");

                }
                return RedirectToAction("ListTaxes");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }


            return View(model);
        }



        // GET: Rates/Edit/5
        public async Task<IActionResult> EditTax(int id)
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var isAdminUri = API.Common.IsAdmin(_remoteServiceCommonUrl, user.Email);
            var isAdminString = await _apiClient.GetStringAsync(isAdminUri, token);
            var isAdminResponse = JsonConvert.DeserializeObject<bool>(isAdminString);

            if (!isAdminResponse)
                return RedirectToAction("index", "home");

            var taxUri = API.Tax.GetTax(_remoteServiceRatessUrl, id);
            var taxString = await _apiClient.GetStringAsync(taxUri, token);
            var response = JsonConvert.DeserializeObject<TaxModel>(taxString);

            return View(response);
             
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTax(TaxModel model)
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();
            var isAdminUri = API.Common.IsAdmin(_remoteServiceCommonUrl, user.Email);
            var isAdminString = await _apiClient.GetStringAsync(isAdminUri, token);
            var isAdminResponse = JsonConvert.DeserializeObject<bool>(isAdminString);

            if (!isAdminResponse)
                return NotFound("Invalid entry");

            //if (ModelState.IsValid)
            //{
            try
            {

                var taxUri = API.Tax.SaveTax(_remoteServiceRatessUrl);

                var response = await _apiClient.PostAsync(taxUri, model, token);

                if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    //throw new Exception("Error creating Shipping, try later.");

                    ModelState.AddModelError("", "Error updating tax table, try later.");

                }
                return RedirectToAction("ListTaxes");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }


            return View(model);
        }





        [AllowAnonymous]
        public async Task<IActionResult> CalculateAmount(decimal distance, decimal weight, int priority, int packageSizeId, string promoCode)
        {
           // var token = await GetUserTokenAsync();

            var allRatesUri = API.Rate.Amount(_remoteServiceRatessUrl, distance,  weight,   priority, packageSizeId, promoCode);

            var dataString = await _apiClient.GetStringAsync(allRatesUri);

             var response = JsonConvert.DeserializeObject<CalculatedChargeModel>(dataString);

            return Json(response);

            //  return Content(response);
            // return Content(dataString, "application/json");
        }



        public async Task<IActionResult> Index()
        {

            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var isAdminUri = API.Common.IsAdmin(_remoteServiceCommonUrl, user.Email);
            var isAdminString = await _apiClient.GetStringAsync(isAdminUri, token);
            var isAdminResponse = JsonConvert.DeserializeObject<bool>(isAdminString);

            if (!isAdminResponse)
                return RedirectToAction("index", "home");


            var allRatesUri = API.Rate.Get(_remoteServiceRatessUrl);

            var dataString = await _apiClient.GetStringAsync(allRatesUri, token);

            var response = JsonConvert.DeserializeObject<IEnumerable<RateModel>>(dataString);
            
            return View(response);
        }

        // GET: rates/Details/5
        public async Task<IActionResult> DistanceAndWeight(int id)
        {       

            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();
            var isAdminUri = API.Common.IsAdmin(_remoteServiceCommonUrl, user.Email);
            var isAdminString = await _apiClient.GetStringAsync(isAdminUri, token);
            var isAdminResponse = JsonConvert.DeserializeObject<bool>(isAdminString);

            if ( !isAdminResponse)
                return RedirectToAction("index", "home");

            var allRatesUri = API.Rate.Details(_remoteServiceRatessUrl);

            var dataString = await _apiClient.GetStringAsync(allRatesUri, token);

            var response = JsonConvert.DeserializeObject<List<RateDetailModel>>(dataString);

            var model = new WeightAndDistance
            {
                RateWeightSizeModel = response.Where(x=>x.WeightOrDistance== "weight" && x.Charge>0).OrderBy(f=>f.From).ToList(),
                RateDistanceModel = response.Where(x => x.WeightOrDistance == "distance" &&  x.Charge > 0).OrderBy(f => f.From).ToList(),
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DistanceAndWeight(WeightAndDistance model)
        {
            try
            {
                var user = _appUserParser.Parse(HttpContext.User);
                var token = await GetUserTokenAsync();
                var isAdminUri = API.Common.IsAdmin(_remoteServiceCommonUrl, user.Email);
                var isAdminString = await _apiClient.GetStringAsync(isAdminUri, token);
                var isAdminResponse = JsonConvert.DeserializeObject<bool>(isAdminString);

                if (!isAdminResponse)
                    return RedirectToAction("index", "home");


                var save = new List<RateDetailModel>();

                save.AddRange(model.RateDistanceModel);
                save.AddRange(model.RateWeightSizeModel);

                var allRatesUri = API.Rate.DetailSave(_remoteServiceRatessUrl);

                var response = await _apiClient.PostAsync(allRatesUri, save, token);

                ModelState.AddModelError("", "Distance and Weight Rate Updated");

                // return RedirectToRoute("DistanceAndWeight", new { id = 1 });


            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            
            return View(model);
        }
             

            // GET: Rates/Edit/5
            public async Task<IActionResult> Edit(int id)
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var isAdminUri = API.Common.IsAdmin(_remoteServiceCommonUrl, user.Email);
            var isAdminString = await _apiClient.GetStringAsync(isAdminUri, token);
            var isAdminResponse = JsonConvert.DeserializeObject<bool>(isAdminString);

            if ( !isAdminResponse)
                return RedirectToAction("index", "home");


            var allRatesUri = API.Rate.GetbyId(_remoteServiceRatessUrl, id);

            var dataString = await _apiClient.GetStringAsync(allRatesUri, token);

            var response = JsonConvert.DeserializeObject<RateModel>(dataString);

            var query = response.RatePriorities.OrderBy(x => x.PriorityTypeId).ToList();

            var model = new RateModel {Id= response.Id, OverHead= response.OverHead, PackageSize = response.PackageSize, RatePriorities = query };
             
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RateModel model)
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();
            var isAdminUri = API.Common.IsAdmin(_remoteServiceCommonUrl, user.Email);
            var isAdminString = await _apiClient.GetStringAsync(isAdminUri, token);
            var isAdminResponse = JsonConvert.DeserializeObject<bool>(isAdminString);

            if (!isAdminResponse)
                return NotFound("Invalid entry");

            //if (ModelState.IsValid)
            //{
            try
            {


                var getRatesUri = API.Rate.GetbyId(_remoteServiceRatessUrl, model.Id);

                var dataString = await _apiClient.GetStringAsync(getRatesUri, token);

                var rateToUpdate = JsonConvert.DeserializeObject<RateModel>(dataString);
                if (rateToUpdate == null)
                    ModelState.AddModelError("", "Error updating rate");
                else
                {
                    rateToUpdate.OverHead = model.OverHead;
                    rateToUpdate.RatePriorities = model.RatePriorities;
                     
                    var allRatesUri = API.Rate.SaveRate(_remoteServiceRatessUrl);

                    var response = await _apiClient.PostAsync(allRatesUri, rateToUpdate, token);

                    //return RedirectToAction("Index");
                    ModelState.AddModelError("", "Rate Updated");
                    model = rateToUpdate;

                      allRatesUri = API.Rate.GetbyId(_remoteServiceRatessUrl, model.Id);

                      dataString = await _apiClient.GetStringAsync(allRatesUri, token);

                    rateToUpdate = JsonConvert.DeserializeObject<RateModel>(dataString);

                    var query = rateToUpdate.RatePriorities.OrderBy(x => x.PriorityTypeId).ToList();

                      model = new RateModel { Id = rateToUpdate.Id, OverHead = rateToUpdate.OverHead, PackageSize = rateToUpdate.PackageSize, RatePriorities = query };


                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
           

            return View(model);
        }
       


        async Task<string> GetUserTokenAsync()
        {
            var context = _httpContextAccesor.HttpContext;

            return await context.Authentication.GetTokenAsync("access_token");
        }

    }
}
