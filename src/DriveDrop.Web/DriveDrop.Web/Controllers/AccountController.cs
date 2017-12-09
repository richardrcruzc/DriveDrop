using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;



namespace DriveDrop.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        //private readonly IHttpContextAccessor _httpContextAccesor;
        //private readonly string _remoteServiceIdentityUrl;
        //private readonly IOptionsSnapshot<AppSettings> _settings;
        //private readonly IIdentityParser<ApplicationUser> _identityParser;
        //public AccountController(IIdentityParser<ApplicationUser> identityParser, IOptionsSnapshot<AppSettings> settings,
        //     IHttpContextAccessor httpContextAccesor) {
        //    _identityParser = identityParser;
        //    _settings = settings;
        //    _remoteServiceIdentityUrl = $"{settings.Value.IdentityUrl}/account/";
        //    _httpContextAccesor= httpContextAccesor;
        //}

       
            [Authorize]
            public async Task<IActionResult> SignIn(string returnUrl)
            {
                var user = User as ClaimsPrincipal;

                var token =await GetUserTokenAsync();

                if (token != null)
                {
                    ViewData["access_token"] = token;
                }

                // "Catalog" because UrlHelper doesn't support nameof() for controllers
                // https://github.com/aspnet/Mvc/issues/5853
                return RedirectToAction(nameof(AdminController.Index), "Home");
            }

            public async Task<IActionResult> Signout()
            {
            // delete authentication cookie
            await HttpContext.SignOutAsync();
            // set this so UI rendering sees an anonymous user
            HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

                // "Catalog" because UrlHelper doesn't support nameof() for controllers
                // https://github.com/aspnet/Mvc/issues/5853
                var homeUrl = Url.Action(nameof(AdminController.Index), "Home");
                return new SignOutResult(OpenIdConnectDefaults.AuthenticationScheme,
                    new Microsoft.AspNetCore.Authentication.AuthenticationProperties { RedirectUri = homeUrl });
            }

        async Task<string> GetUserTokenAsync()
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            return token;
        }
        //
        // GET /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }

        //    public ActionResult Index() => View();

        //    [Authorize]
        //    public async Task<IActionResult> SignIn(string returnUrl)
        //    {
        //        var user = User as ClaimsPrincipal;
        //        var token = await GetUserTokenAsync();

        //        if (token != null)
        //        {
        //            ViewData["access_token"] = token;
        //        }

        //        // "Catalog" because UrlHelper doesn't support nameof() for controllers
        //        // https://github.com/aspnet/Mvc/issues/5853
        //        return RedirectToAction(nameof(AdminController.Index), "Home");


        //    }
        //    [Authorize]
        //    public async Task<IActionResult> Register(string returnUrl)
        //    {
        //        var user = User as ClaimsPrincipal;
        //        var token = await GetUserTokenAsync();

        //        if (token != null)
        //        {
        //            ViewData["access_token"] = token;
        //        }

        //        // "Catalog" because UrlHelper doesn't support nameof() for controllers
        //        // https://github.com/aspnet/Mvc/issues/5853
        //        return RedirectToAction(nameof(AdminController.Index), "Home");


        //    }



        //    public async Task<IActionResult> Signout()
        //    {
        //        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //        await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

        //        // "Catalog" because UrlHelper doesn't support nameof() for controllers
        //        // https://github.com/aspnet/Mvc/issues/5853
        //        var homeUrl = Url.Action(nameof(AccountController.SignIn), "Account");
        //        return new SignOutResult("oidc", new AuthenticationProperties { RedirectUri = homeUrl });




        //        //// "Catalog" because UrlHelper doesn't support nameof() for controllers
        //        //// https://github.com/aspnet/Mvc/issues/5853
        //        //var homeUrl = Url.Action(nameof(CatalogController.Index), "Catalog");
        //        //return new SignOutResult(OpenIdConnectDefaults.AuthenticationScheme,
        //        //    new AspNetCore.Authentication.AuthenticationProperties { RedirectUri = homeUrl });



        //    }


        //    async Task<string> GetUserTokenAsync()
        //    {
        //        var context = _httpContextAccesor.HttpContext;

        //        return await context.GetTokenAsync("access_token");
        //    }
        //    //
        //    // GET /Account/AccessDenied
        //    [HttpGet]
        //    public IActionResult AccessDenied()
        //    {
        //        return View();
        //    }
        //}
    }
