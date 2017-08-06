using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; 
using Microsoft.AspNetCore.Http.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication; 
using DriveDrop.Web.Services;
using DriveDrop.Web.ViewModels;

namespace DriveDrop.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IIdentityParser<ApplicationUser> _identityParser;
        public AccountController(IIdentityParser<ApplicationUser> identityParser) =>
            _identityParser = identityParser;

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

        public IActionResult Signout()
        {
            HttpContext.Authentication.SignOutAsync("Cookies");
            HttpContext.Authentication.SignOutAsync("oidc");

            // "Catalog" because UrlHelper doesn't support nameof() for controllers
            // https://github.com/aspnet/Mvc/issues/5853
            var homeUrl = Url.Action(nameof(AccountController.SignIn), "Account");
            return new SignOutResult("oidc", new AuthenticationProperties { RedirectUri = homeUrl });
        }
    }
}
