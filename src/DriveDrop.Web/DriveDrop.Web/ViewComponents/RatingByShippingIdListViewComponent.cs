using DriveDrop.Web.Infrastructure;
using DriveDrop.Web.Services;
using DriveDrop.Web.ViewModels;
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
  
    public class RatingByShippingIdListViewComponent : ViewComponent
    {
        private IHttpClient _apiClient;
        private readonly string _remoteServiceBaseUrl;
        private readonly string _remoteServiceCommonUrl;
        private readonly string _remoteServiceRatingUrl;
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly IHttpContextAccessor _httpContextAccesor;
        private readonly IIdentityParser<ApplicationUser> _appUserParser;


        public RatingByShippingIdListViewComponent(IOptionsSnapshot<AppSettings> settings, IHttpContextAccessor httpContextAccesor,
            IHttpClient httpClient, IIdentityParser<ApplicationUser> appUserParser)
        {
            _remoteServiceCommonUrl = $"{settings.Value.DriveDropUrl}/api/v1/common/";
            _remoteServiceBaseUrl = $"{settings.Value.DriveDropUrl}/api/v1/admin";
            _remoteServiceRatingUrl = $"{settings.Value.DriveDropUrl}/api/v1/review/";
            _settings = settings;
            _httpContextAccesor = httpContextAccesor;
            _apiClient = httpClient;
            _appUserParser = appUserParser;



        } 
        public async Task<IViewComponentResult> InvokeAsync(int indexPage, int pageSize,int? senderId, int? driverId,int? published, int? reviwApplyTo, int? shippingId , string hidden =null)        
        {
            var allRatesUri = API.Rating.GetAllReviews(_remoteServiceRatingUrl, indexPage,  pageSize,  senderId,  driverId,  published,  reviwApplyTo,  shippingId);
            var dataString = await _apiClient.GetStringAsync(allRatesUri);
            var response = JsonConvert.DeserializeObject<ReviewIndex>(dataString);

            if (hidden!=null)
                response.HiddenType = hidden;               


            return View(response);

             
        }

    }
}
