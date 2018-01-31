using DriveDrop.Bl.Infrastructure;
using DriveDrop.Bl.Services;
using DriveDrop.Bl.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc; 
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DriveDrop.Bl.Data;

namespace DriveDrop.Bl.ViewComponents
{
  
    public class UserInfoViewComponent : ViewComponent
    {
     
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly IHttpContextAccessor _httpContextAccesor;
        private readonly IIdentityParser<Models.ApplicationUser> _appUserParser; 
        private readonly ICustomerService _cService;

        public UserInfoViewComponent(IOptionsSnapshot<AppSettings> settings, IHttpContextAccessor httpContextAccesor,
            IIdentityParser<Models.ApplicationUser> appUserParser,
            ICustomerService cService)
        {
            
            _settings = settings;
            _httpContextAccesor = httpContextAccesor;
           
            _appUserParser = appUserParser;
            _cService = cService;

        }
        public async Task<IViewComponentResult> InvokeAsync(int id) 
        {
            var user = _appUserParser.Parse(HttpContext.User); 

            if (user.Email == "")
                return View(new UserStatusModel());
            var currentUser = await _cService.Get(user.Email);
            if (currentUser == null)
                return View(new UserStatusModel());


            //if (string.IsNullOrEmpty(currentUser.PersonalPhotoUri))
            //    currentUser.PersonalPhotoUri = "profile-icon.png";
            //currentUser.PersonalPhotoUri = _settings.Value.PicBaseUrl.Replace("[0]", System.Net.WebUtility.UrlDecode(currentUser.PersonalPhotoUri));
            
               

            if (currentUser == null)
            { 
                return View(new CurrentCustomerModel());
            }
            if (currentUser.UserName == null)
            {

                return View(new CurrentCustomerModel());
            }
            return View(currentUser);
        }
         

    }
}
