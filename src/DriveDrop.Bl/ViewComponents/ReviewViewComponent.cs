using DriveDrop.Bl.Services;
using DriveDrop.Bl.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
 
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DriveDrop.Bl.Data;

namespace DriveDrop.Bl.ViewComponents
{
    public class ReviewViewComponent : ViewComponent
    {
      
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly IHttpContextAccessor _httpContextAccesor;
        private readonly IIdentityParser<Models.ApplicationUser> _appUserParser;

        private readonly DriveDropContext _context;
        public ReviewViewComponent(IOptionsSnapshot<AppSettings> settings, IHttpContextAccessor httpContextAccesor,
            IIdentityParser<Models.ApplicationUser> appUserParser,
             DriveDropContext context)
        {
            _context = context;
            _settings = settings;
            _httpContextAccesor = httpContextAccesor;
           
            _appUserParser = appUserParser;
             
        }


        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var model =new ReviewModel();

            return View(model);
        }
        async Task<string> GetUserTokenAsync()
        {
            var context = _httpContextAccesor.HttpContext;

             return await context.GetTokenAsync("access_token");
        }
    }
}
