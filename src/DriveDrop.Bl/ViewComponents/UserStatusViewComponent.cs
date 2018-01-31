using DriveDrop.Bl.Infrastructure;
using DriveDrop.Bl.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; 
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using DriveDrop.Bl.Services;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication;
using DriveDrop.Bl.Data;

namespace DriveDrop.Bl.ViewComponents
{
    public class UserStatusViewComponent : ViewComponent
    {
        private readonly ICustomerService _cService;
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly IHttpContextAccessor _httpContextAccesor;
        private readonly IIdentityParser<Models.ApplicationUser> _appUserParser;
        private readonly DriveDropContext _context;

        public UserStatusViewComponent(IOptionsSnapshot<AppSettings> settings, IHttpContextAccessor httpContextAccesor,
             IIdentityParser<Models.ApplicationUser> appUserParser, DriveDropContext context,
              ICustomerService cService)
        {
           
            _settings = settings;
            _httpContextAccesor = httpContextAccesor;
           
            _appUserParser = appUserParser;

            _context = context;
            _cService = cService;
        }
        public async Task<IViewComponentResult> InvokeAsync(int id)

        {
            var user = _appUserParser.Parse(HttpContext.User);

            var model = new UserStatusModel();

            if (string.IsNullOrEmpty(user.Email))
                return View(new UserStatusModel());

            var c = await _cService.Get(user.Email);
            if (c == null)
                return View(model);

              
            if (c == null )
            {
                model.Status = "There is a error with this user, please try again later...";
                return View(model);
            }
            if (c.UserName == null)
            {
                model.Status = "There is a error with this user, please try again later...";
                return View(model);
            }
            if (c.CustomerStatusId!=2)
                model.Status = c.CustomerStatus;

            if (c.CanBeUnImpersonate)
                model.Impersonated = c.UserName;
            model.CustomerType = c.CustomerType;

            return View(model);
        }
         

    }
}
