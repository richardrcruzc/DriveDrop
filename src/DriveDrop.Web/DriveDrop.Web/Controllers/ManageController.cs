using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DriveDrop.Web.ViewModels;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using DriveDrop.Web.Services;
using Microsoft.eShopOnContainers.BuildingBlocks.Resilience.Http;
using Microsoft.AspNetCore.Hosting;
using DriveDrop.Web.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;

namespace DriveDrop.Web.Controllers
{
    public class ManageController : Controller
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

        public ManageController(IOptionsSnapshot<AppSettings> settings, IHttpContextAccessor httpContextAccesor,
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


            //
            // GET: /Manage/ChangePassword
            [HttpGet]
        public async Task<IActionResult> ChangePassword (int id)
        {

            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var getUserUri = API.Common.GetUser(_remoteServiceCommonUrl, user.Email);
            var userString = await _apiClient.GetStringAsync(getUserUri, token);
            var customer = JsonConvert.DeserializeObject<Customer>(userString);
            if (customer == null)
                return RedirectToAction("index", "home");

            var model = new ChangePasswordViewModel();
            model.Id = id;
            if (customer.CustomerTypeId == 3)
                model.IsDriver = true;
            else
                model.IsDriver = false;

            if (customer.CustomerTypeId == 1)
                model.IsAdmin =true;
            return View(model);
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var getUserUri = API.Common.GetUser(_remoteServiceCommonUrl, user.Email);
            var userString = await _apiClient.GetStringAsync(getUserUri, token);
            var customer = JsonConvert.DeserializeObject<Customer>(userString);
            if (customer == null)
                return RedirectToAction("index", "home");
            else if (customer.CustomerTypeId == 2)
                return RedirectToAction("index", "home");
            
            if (user != null)
            { 

               var changePassword=  API.Identity.ChangePassword(_remoteServiceIdentityUrl, user.Email, model.OldPassword, model.NewPassword, model.ConfirmPassword);

                var dataString = await _apiClient.GetStringAsync(changePassword);
                if (dataString == null)
                {
                    ModelState.AddModelError("", "Unable to password password");                    
                    return View(model);
                }else
                if (!dataString.Contains("User changed their password successfully"))
                {
                    ModelState.AddModelError("", "User changed their password successfully");
                    return View(model);
                }

                ModelState.AddModelError("", dataString);
                return View(model);
            }
            ModelState.AddModelError("", "Somthing wrong!");
            return View(model);
        }
        async Task<string> GetUserTokenAsync()
        {
            var context = _httpContextAccesor.HttpContext;

            return await context.Authentication.GetTokenAsync("access_token");
        }
    }
}