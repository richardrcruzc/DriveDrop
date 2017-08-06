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
        public async Task<IActionResult> Create()
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            RateModel model = new RateModel { Active = true, EndDate = DateTime.Today, StartDate=DateTime.Today, Tax=0 };

            var allRatesUri = API.Rate.NewRate(_remoteServiceRatessUrl);

            var response = await _apiClient.PostAsync(allRatesUri, model, token); 

            var allRatesUri1 = API.Rate.Get(_remoteServiceRatessUrl);
            var dataString = await _apiClient.GetStringAsync(allRatesUri1, token);
            var rates = JsonConvert.DeserializeObject<List<RateModel>>(dataString);
            var last = rates.OrderByDescending(x => x.Id).FirstOrDefault();

            return RedirectToAction("Edit", new { id = last.Id});

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
                if (item.WeightOrDistance!=null && item.WeightOrDistance.ToLower() == "distance")
                    response.MileRateDetails.Add(item);
                else
                    response.WeightRateDetails.Add(item);
             
            await PrepareRate(response);


          

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

            //if (ModelState.IsValid)
            //{
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

                    foreach (var item in model.MileRateDetails)
                    {
                        if (item.Charge == 0 || item.From == 0 || item.To == 0)
                            continue;
                        item.WeightOrDistance = "distance";
                        item.MileOrLbs = "miles";
                        model.RateDetails.Add(item);
                    }
                    foreach (var item in model.WeightRateDetails)
                    {
                        if (item.Charge == 0 || item.From == 0 || item.To == 0)
                            continue;

                        item.WeightOrDistance = "weight";
                        item.MileOrLbs = "lbs";
                        model.RateDetails.Add(item);
                    }
                        
                        var allRatesUri = API.Rate.SaveRate(_remoteServiceRatessUrl);

                        var response = await _apiClient.PostAsync(allRatesUri, model, token);
                    }
                    //return RedirectToAction("Index");
                }
                catch 
                {
                }
           // }

            await PrepareRate(model);
            return View(model);
        }
        public async Task<IActionResult> DeleteRate(int id)
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();
            var deleteRateUri = API.Rate.DeleteRate(_remoteServiceRatessUrl, id);

            var dataString = await _apiClient.GetStringAsync(deleteRateUri, token);



            return RedirectToAction("Index");
        }
            public async Task<IActionResult> Delete(int id, int rateId, int type)
        {
            var del = new RateDeleteDetailModel();
            del.RateId = rateId;
            if(type==1)
            del.RateDetails.Add(new RateDetailModel { RateId = rateId, Id=id });
            if (type == 2)
                del.RatePackageSizes.Add(new RatePackageSizeModel { RateId = rateId, Id = id });
            if (type == 3)
                del.RatePriorities.Add(new RatePriorityModel { RateId = rateId, Id = id });

            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();
            var deleteRateUri = API.Rate.DeleteDetail(_remoteServiceRatessUrl);

            var response = await _apiClient.PostAsync(deleteRateUri, del, token);

            return RedirectToAction("edit", new { id = rateId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MileRateDetail(List<RateDetailModel> model)
        {

            return ViewComponent("PerMiles", new { model = model });
        }
        async Task<string> GetUserTokenAsync()
        {
            var context = _httpContextAccesor.HttpContext;

            return await context.Authentication.GetTokenAsync("access_token");
        }

        public async Task<RateModel>  PrepareRate(RateModel model)
        { 
            var CustomerTypes = new List<SelectListItem>();
            CustomerTypes.Add(new SelectListItem() { Value = null, Text = "All", Selected = true });


            var getUri = API.Common.GetAllPriorityTypes(_remoteServiceCommonUrl);
            var dataString = await _apiClient.GetStringAsync(getUri);
            var priority = new List<SelectListItem>();
            priority.Add(new SelectListItem() { Value = null, Text = "Select a priority", Selected = true });
            
            var gets = JArray.Parse(dataString);

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

            ViewBag.PackageSizeList = model.PackageSizeList;
            ViewBag.PriorityTypeList = model.PriorityTypeList;


            var ps = model.PackageSizes.FirstOrDefault(x => x.Charge == 0 && x.PackageSizeId == 0);
            if (ps==null || model.PackageSizes.Count()==0)
                model.PackageSizes.Add(new RatePackageSizeModel { RateId = model.Id });

            var rp = model.RatePriorities.FirstOrDefault(x => x.Charge == 0 && x.PriorityId == 0);
            if (rp == null || model.RatePriorities.Count() == 0)
                model.RatePriorities.Add(new RatePriorityModel { RateId = model.Id });


            var md = model.MileRateDetails.FirstOrDefault(x=>x.Charge==0 && x.From==0 && x.To==0 );
            if(md == null || model.MileRateDetails.Count()==0)
                model.MileRateDetails.Add(new RateDetailModel { RateId = model.Id });


            var wd = model.WeightRateDetails.FirstOrDefault(x => x.Charge == 0 && x.From == 0 && x.To == 0);
            if (wd == null || model.WeightRateDetails.Count() == 0)
                model.WeightRateDetails.Add(new RateDetailModel { RateId = model.Id });

 

            return model;
        }


    }
}
