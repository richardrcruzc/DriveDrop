using DriveDrop.Web.Infrastructure;
using DriveDrop.Web.Services;
using DriveDrop.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.BuildingBlocks.Resilience.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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


        public async Task<IActionResult> CalculateAmount(decimal distance, decimal weight, int priority, int packageSizeId, string promoCode)
        {
            var token = await GetUserTokenAsync();

            var allRatesUri = API.Rate.Amount(_remoteServiceRatessUrl, distance,  weight,   priority, packageSizeId, promoCode);

            var dataString = await _apiClient.GetStringAsync(allRatesUri, token);

             var response = JsonConvert.DeserializeObject<CalculatedChargeModel>(dataString);

            return Json(response);

            //  return Content(response);
            // return Content(dataString, "application/json");
        }



        public async Task<IActionResult> Index()
        {

            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var allRatesUri = API.Rate.Get(_remoteServiceRatessUrl);

            var dataString = await _apiClient.GetStringAsync(allRatesUri, token);



          var response = JsonConvert.DeserializeObject<List<RateModel>>(dataString);
             

            return View(response);
        }

        // GET: rates/Details/5
        public async Task<IActionResult> Details(int id)
        {       

            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var allRatesUri = API.Rate.GetbyId(_remoteServiceRatessUrl, id);

            var dataString = await _apiClient.GetStringAsync(allRatesUri, token);

            var response = JsonConvert.DeserializeObject<RateModel>(dataString);

            return View(response);
        }

        // GET: Rates/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var allRatesUri = API.Rate.GetbyId(_remoteServiceRatessUrl, id);

            var dataString = await _apiClient.GetStringAsync(allRatesUri, token);

            var response = JsonConvert.DeserializeObject<RateModel>(dataString);

            response.WeightRateDetails = new List<RateDetailModel>();
            foreach (var item in response.RateDetails)
                if (item.WeightOrDistance.ToLower() == "distance")
                    response.MileRateDetails.Add(item);
                else
                    response.WeightRateDetails.Add(item);

            response.RateDetail.WeightOrDistance = "";
            response.RateDetail.MileOrLbs = "miles";

            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
       public async Task<IActionResult> Edit(RateModel model)
        {

            ModelState.Remove("WeightOrDistance");

            foreach (var state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
                var tt = state.Errors.ToString();
            }
            if (model.RateDetail.WeightOrDistance != null)
                if (model.RateDetail.WeightOrDistance == "weight")
                    model.RateDetail.MileOrLbs = "lbs";
            else
                    model.RateDetail.MileOrLbs = "miles";

            if (ModelState.IsValid)
            {
                try
                {
                    var user = _appUserParser.Parse(HttpContext.User);
                    var token = await GetUserTokenAsync();

                    var ratesUri = API.Rate.GetbyId(_remoteServiceRatessUrl, model.Id);

                    var dataString = await _apiClient.GetStringAsync(ratesUri, token);

                    var responseR = JsonConvert.DeserializeObject<RateModel>(dataString);

                    if (responseR != null)
                    {
                        model.RateDetails = new List<RateDetailModel>();

                        if (model.RateDetail.From > 0 && model.RateDetail.To > 0 && model.RateDetail.Charge > 0 
                            && model.RateDetail.WeightOrDistance != null 
                            && (model.RateDetail.WeightOrDistance.Contains("weight") || model.RateDetail.WeightOrDistance.Contains("distance")))
                        {
                            model.RateDetails.Add(model.RateDetail);
                             
                        }

                        foreach (var item in model.MileRateDetails)
                            model.RateDetails.Add(item);

                        foreach (var item in model.WeightRateDetails)
                            model.RateDetails.Add(item);

                        var allRatesUri = API.Rate.SaveRate(_remoteServiceRatessUrl);

                        var response = await _apiClient.PostAsync(allRatesUri, model, token);
                    }
                    return RedirectToAction("Index");
                }
                catch 
                {
                }
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
