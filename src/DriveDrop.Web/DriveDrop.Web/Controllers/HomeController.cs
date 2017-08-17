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
using Microsoft.AspNetCore.Mvc.Rendering;
using DriveDrop.Web.Infrastructure;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;

namespace DriveDrop.Web.Controllers
{
    public class HomeController : Controller
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


        public HomeController(IOptionsSnapshot<AppSettings> settings, IHttpContextAccessor httpContextAccesor,
            IHttpClient httpClient, IIdentityParser<ApplicationUser> appUserParser)
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

        }

        public async Task<IActionResult> myAccount()
        {
            

            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var isAdminUri = API.Common.IsAdmin(_remoteServiceCommonUrl, user.Email);
            var isAdminString = await _apiClient.GetStringAsync(isAdminUri, token);
            var isAdminResponse = JsonConvert.DeserializeObject<bool>(isAdminString);

            if (isAdminResponse)
                return RedirectToAction("index", "admin");
            else
            {
                var getUserUri = API.Common.GetUser(_remoteServiceCommonUrl, user.Email);
                var userString = await _apiClient.GetStringAsync(getUserUri, token);
                var customer = JsonConvert.DeserializeObject<Customer>(userString);
                if (customer == null)
                {
                    ViewBag.UserValid = "false";
                    return RedirectToAction("Index");
                }
                if (customer.CustomerTypeId == 1)
                        return RedirectToAction("index", "admin", new { id = customer.Id });
                    else if (customer.CustomerTypeId==2)
                        return RedirectToAction("result", "sender", new { id = customer.Id});
                    else if (customer.CustomerTypeId == 3)
                        return RedirectToAction("result", "driver", new { id = customer.Id });

            }

            ViewBag.UserValid = "false";
            return RedirectToAction("Index");
        }



        public async Task<IActionResult> Index()
        {
            var model = new HomeQuote();

            var getUri = API.Common.GetAllTransportTypes(_remoteServiceCommonUrl);
            var dataString = await _apiClient.GetStringAsync(getUri);
            var transportTypes = new List<SelectListItem>();
            transportTypes.Add(new SelectListItem() { Value = null, Text = "Fit in...", Selected = true });

           var  gets = JArray.Parse(dataString);

            foreach (var brand in gets.Children<JObject>())
            {
                transportTypes.Add(new SelectListItem()
                {
                    Value = brand.Value<string>("id"),
                    Text = brand.Value<string>("name")
                });
            }
            model.TransportTypeList = transportTypes;


            getUri = API.Common.GetAllPackageSizes(_remoteServiceCommonUrl);
              dataString = await _apiClient.GetStringAsync(getUri);
            var packageSize = new List<SelectListItem>();
            packageSize.Add(new SelectListItem() { Value = null, Text = "Select package Size ...", Selected = true });

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



            getUri = API.Common.GetAllPriorityTypes(_remoteServiceCommonUrl);
            dataString = await _apiClient.GetStringAsync(getUri);
            var priorityType = new List<SelectListItem>();
            priorityType.Add(new SelectListItem() { Value = null, Text = "Select priority ...", Selected = true });

            gets = JArray.Parse(dataString);

            foreach (var brand in gets.Children<JObject>())
            {
                priorityType.Add(new SelectListItem()
                {
                    Value = brand.Value<string>("id"),
                    Text = brand.Value<string>("name")
                });
            }
            model.PriorityTypeList = priorityType;

            return View(model);
        }


        public JsonResult SearchCity(string seach)
        {
            var countries = new List<SelectListItem>
         {
             new   SelectListItem()
                {
                    Value ="United States",
                    Text ="United States"
                } ,
              new   SelectListItem()
                {
                    Value ="Canada",
                    Text ="Canada"
                }  
         };

            return Json(countries);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

    


        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
        [NonAction]
        async Task<string> GetUserTokenAsync()
        {
            var context = _httpContextAccesor.HttpContext;

            return await context.Authentication.GetTokenAsync("access_token");
        }

    }
}
