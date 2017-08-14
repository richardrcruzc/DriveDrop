using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; 
using Microsoft.AspNetCore.Http.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication; 
using DriveDrop.Web.Services;
using DriveDrop.Web.ViewModels;
using Microsoft.Extensions.Options;

namespace DriveDrop.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {

        private readonly string _remoteServiceIdentityUrl;
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly IIdentityParser<ApplicationUser> _identityParser;
        public AccountController(IIdentityParser<ApplicationUser> identityParser, IOptionsSnapshot<AppSettings> settings) {
            _identityParser = identityParser;
            _settings = settings;
            _remoteServiceIdentityUrl = $"{settings.Value.IdentityUrl}/account/";
        }

        public ActionResult Index() => View();

        [Authorize]
        public async Task<IActionResult> SignIn(string returnUrl)
        {
            var user = User as ClaimsPrincipal;
            var token = await HttpContext.Authentication.GetTokenAsync("access_token");

            if (token != null)
            {
                ViewData["access_token"] = token;
            }

            // "Catalog" because UrlHelper doesn't support nameof() for controllers
            // https://github.com/aspnet/Mvc/issues/5853
            return RedirectToAction(nameof(AdminController.Index), "Home");
             

        }
        [Authorize]
        public async Task<IActionResult> Register(string returnUrl)
        {
            var user = User as ClaimsPrincipal;
            var token = await HttpContext.Authentication.GetTokenAsync("access_token");

            if (token != null)
            {
                ViewData["access_token"] = token;
            }

            // "Catalog" because UrlHelper doesn't support nameof() for controllers
            // https://github.com/aspnet/Mvc/issues/5853
            return RedirectToAction(nameof(AdminController.Index), "Home");


        }

       

        public IActionResult Signout()
        {
            HttpContext.Authentication.SignOutAsync("Cookies");
            HttpContext.Authentication.SignOutAsync("oidc");

            // "Catalog" because UrlHelper doesn't support nameof() for controllers
            // https://github.com/aspnet/Mvc/issues/5853
            var homeUrl = Url.Action(nameof(AccountController.SignIn), "Account");
            return new SignOutResult("oidc", new AuthenticationProperties { RedirectUri = homeUrl });
        }

        

        //
        // GET /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
