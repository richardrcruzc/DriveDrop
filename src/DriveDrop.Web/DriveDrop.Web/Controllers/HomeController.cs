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
using Microsoft.AspNetCore.Authorization;
using System.Net.WebSockets;
using System.Threading;
using System.Text;

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
       // private readonly IHttpContextAccessor _httpContextAccesor;
        private readonly IIdentityParser<ApplicationUser> _appUserParser;

        private readonly string _remoteServiceAdminUrl;

        public HomeController(IOptionsSnapshot<AppSettings> settings,
            //, IHttpContextAccessor httpContextAccesor,
            IHttpClient httpClient, IIdentityParser<ApplicationUser> appUserParser)
        {
            _remoteServiceCommonUrl = $"{settings.Value.DriveDropUrl}/api/v1/common/";
            _remoteServiceBaseUrl = $"{settings.Value.DriveDropUrl}/api/v1/sender";
            _remoteServiceShippingsUrl = $"{settings.Value.DriveDropUrl}/api/v1/shippings";
            _remoteServiceDriversUrl = $"{settings.Value.DriveDropUrl}/api/v1/drivers";
            _remoteServiceIdentityUrl = $"{settings.Value.IdentityUrl}/account/";
            _remoteServiceAdminUrl = $"{settings.Value.DriveDropUrl}/api/v1/admin";
            _settings = settings;
          //  _httpContextAccesor = httpContextAccesor;
            _apiClient = httpClient;
            _appUserParser = appUserParser;

        }

        //[Route("ProvideData")]
        //[HttpGet]
        public async Task<ActionResult> ProvideDataAsync(string data="test msq", string connectionName="default")
        {
            
            //byte[] buffer = Encoding.UTF8.GetBytes(data);
            //ArraySegment<byte> segment = new ArraySegment<byte>(buffer);

            //WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync(); // _httpContextAccesor.HttpContext.WebSockets.AcceptWebSocketAsync();
            //await webSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);

            if (!string.IsNullOrEmpty(data) && !string.IsNullOrEmpty(connectionName))
            {
               

                //_httpContextAccesor.HttpContext.Items.Add("ConnectionName", connectionName);
                //_httpContextAccesor.HttpContext.Items.Add("Data", data);

                // WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                HttpContext.Request.Path = "/ws";
                HttpContext.Items.Add("ConnectionName", connectionName);
                HttpContext.Items.Add("Data", data);
            }
            return Ok("OK");
        }


        public async Task<IActionResult> myAccount()
        { 

            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync(); 


            var getcurrent = API.Admin.GetbyUserName(_remoteServiceAdminUrl, user.Email);
            var currentDataString = await _apiClient.GetStringAsync(getcurrent, token);
            var currentUser = JsonConvert.DeserializeObject<CurrentCustomerModel>((currentDataString));

            if (currentUser != null)
            {
                if (currentUser.IsAdmin)
                    return RedirectToAction("index", "admin");
                else
                {

                    if (currentUser.CustomerTypeId == 1)
                        return RedirectToAction("index", "admin", new { id = currentUser.Id });
                    else if (currentUser.CustomerTypeId == 2)
                        return RedirectToAction("result", "sender", new { id = currentUser.Id });
                    else if (currentUser.CustomerTypeId == 3)
                        return RedirectToAction("result", "driver", new { id = currentUser.Id });

                }
            }

            ViewBag.UserValid = "false";
            return RedirectToAction("Index");
        }



        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = _appUserParser.Parse(HttpContext.User);
                var token = await GetUserTokenAsync();


                var getcurrent = API.Admin.GetbyUserName(_remoteServiceAdminUrl, user.Email);
                var currentDataString = await _apiClient.GetStringAsync(getcurrent, token);
                var currentUser = JsonConvert.DeserializeObject<CurrentCustomerModel>((currentDataString));

                if (currentUser == null)
                    return RedirectToAction("Signout", "account");
                else
                    if (currentUser.IsAdmin)
                    return RedirectToAction("index", "admin");
                else
                {

                    if (currentUser.CustomerTypeId == 1)
                        return RedirectToAction("index", "admin", new { id = currentUser.Id });
                    else if (currentUser.CustomerTypeId == 2)
                        return RedirectToAction("result", "sender", new { id = currentUser.Id });
                    else if (currentUser.CustomerTypeId == 3)
                        return RedirectToAction("result", "driver", new { id = currentUser.Id });

                }

            }

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
            packageSize.Add(new SelectListItem() { Value = null, Text = "Select Size", Selected = true });

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
            priorityType.Add(new SelectListItem() { Value = null, Text = "Select Priority", Selected = true });

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

        public IActionResult Test()
        {
            

                return View();
        }

        [HttpGet]
        public IActionResult Testchat()
        {
            return View("InsertUserName");
        }

        [HttpPost]
        public IActionResult Testchat(string username)
        {
            return View("Testchat", username);
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
            await Task.Delay(100);
            return string.Empty;

            //var context = _httpContextAccesor.HttpContext;

            // return await context.GetTokenAsync("access_token");
        }

    }
}
