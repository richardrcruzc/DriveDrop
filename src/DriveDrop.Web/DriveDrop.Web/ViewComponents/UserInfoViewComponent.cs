using DriveDrop.Web.Infrastructure;
using DriveDrop.Web.Services;
using DriveDrop.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.BuildingBlocks.Resilience.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Web.ViewComponents
{
  
    public class UserInfoViewComponent : ViewComponent
    {
        private IHttpClient _apiClient;
        private readonly string _remoteServiceBaseUrl;
        private readonly string _remoteServiceCommonUrl;
        private readonly string _remoteServiceRatesUrl;
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly IHttpContextAccessor _httpContextAccesor;
        private readonly IIdentityParser<ApplicationUser> _appUserParser;


        public UserInfoViewComponent(IOptionsSnapshot<AppSettings> settings, IHttpContextAccessor httpContextAccesor,
            IHttpClient httpClient, IIdentityParser<ApplicationUser> appUserParser)
        {
            _remoteServiceCommonUrl = $"{settings.Value.DriveDropUrl}/api/v1/common/";
            _remoteServiceBaseUrl = $"{settings.Value.DriveDropUrl}/api/v1/admin";
            _remoteServiceRatesUrl = $"{settings.Value.DriveDropUrl}/api/v1/rates";
            _settings = settings;
            _httpContextAccesor = httpContextAccesor;
            _apiClient = httpClient;
            _appUserParser = appUserParser;



        }
        public async Task<IViewComponentResult> InvokeAsync(int id)

        {
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            if (user.Email == "")
                return View(new UserStatusModel());

            var getcurrent = API.Admin.GetbyUserName(_remoteServiceBaseUrl, user.Email);
            var currentDataString = await _apiClient.GetStringAsync(getcurrent, token);
            var currentUser = JsonConvert.DeserializeObject<CurrentCustomerModel>((currentDataString));

            
            if (currentUser == null)
            { 
                return View(currentUser);
            }
            if (currentUser.UserName == null)
            {
              
                return View(currentUser);
            }
            

            return View(currentUser);
        }

        async Task<string> GetUserTokenAsync()
        {
            var context = _httpContextAccesor.HttpContext;

            return await context.Authentication.GetTokenAsync("access_token");
        }

    }
}
