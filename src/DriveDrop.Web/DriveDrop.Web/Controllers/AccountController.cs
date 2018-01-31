using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using DriveDrop.Web.ViewModels;
using Microsoft.eShopOnContainers.BuildingBlocks.Resilience.Http;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using DriveDrop.Web.Services;
using Microsoft.AspNetCore.Hosting;
using DriveDrop.Web.Infrastructure;
using System;

namespace DriveDrop.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
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

        public AccountController( 
            IOptionsSnapshot<AppSettings> settings, IHttpContextAccessor httpContextAccesor,
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


        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            //await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {

                model.Email = System.Net.WebUtility.UrlEncode(model.Email);
                model.Password = System.Net.WebUtility.UrlEncode(model.Password);
                var addNewUserUri = API.Identity.LoginExt(_remoteServiceIdentityUrl, model.Email, model.Password);
                try
                {
                    var response = await _apiClient.GetStringAsync(addNewUserUri);
                    //if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    //{
                    //    ModelState.AddModelError("", "Unable to login user : " + model.Email);
                    //    return View(model);
                    //}

                }
                catch(Exception ex)
                {
                    var test = ex.Message;
                }


                    //if (dataString == null)
                    //{
                    //    ModelState.AddModelError("", "Unable to register user : " + c.UserEmail);

                    //    return View(model);
                    //}
                    //if (!dataString.Contains("IsAuthenticated") && !dataString.Contains("IsNotAuthenticated"))
                    //{
                    //    ModelState.AddModelError("", "Unable to register user : " + c.UserEmail);

                    //    return View(model);
                    //}




                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, model.Email));

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                return RedirectToAction(nameof(AdminController.Index), "Home");
          
                
                
                
                //// This doesn't count login failures towards account lockout
                //// To enable password failures to trigger account lockout, set lockoutOnFailure: true
                //var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                //if (result.Succeeded)
                //{
                //    _logger.LogInformation("User logged in.");
                //    return RedirectToLocal(returnUrl);
                //}
                //if (result.RequiresTwoFactor)
                //{
                //    return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });
                //}
                //if (result.IsLockedOut)
                //{
                //    _logger.LogWarning("User account locked out.");
                //    return RedirectToAction(nameof(Lockout));
                //}
                //else
                //{
                //    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                //    return View(model);
                //}
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        //[Authorize]
        //public async Task<IActionResult> SignIn(string returnUrl = null)
        //{
        //    var user = User as ClaimsPrincipal;

        //    var token =await GetUserTokenAsync();

        //    if (token != null)
        //    {
        //        ViewData["access_token"] = token;
        //    }

        //    // "Catalog" because UrlHelper doesn't support nameof() for controllers
        //    // https://github.com/aspnet/Mvc/issues/5853
        //    return RedirectToAction(nameof(AdminController.Index), "Home");
        //}

        public async Task<IActionResult> Signout()
            {

            await HttpContext.SignOutAsync(
    CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction(nameof(AdminController.Index), "Home");
            //// delete authentication cookie
            //await HttpContext.SignOutAsync();
            //// set this so UI rendering sees an anonymous user
            //HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //    await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

           // return View();

                //// "Catalog" because UrlHelper doesn't support nameof() for controllers
                //// https://github.com/aspnet/Mvc/issues/5853
                //var homeUrl = Url.Action(nameof(AdminController.Index), "Home");
                //return new SignOutResult(OpenIdConnectDefaults.AuthenticationScheme,
                //    new Microsoft.AspNetCore.Authentication.AuthenticationProperties { RedirectUri = homeUrl });
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
