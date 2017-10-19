// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4.Quickstart.UI.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Identity.Api.Services;
using Identity.Api.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Identity.Api.Models.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Identity.Api;
using Identity.Api.Models.ManageViewModels;

namespace IdentityServer4.Quickstart.UI.Controllers
{
    /// <summary>
    /// This sample controller implements a typical login/logout/provision workflow for local and external accounts.
    /// The login service encapsulates the interactions with the user data store. This data store is in-memory only and cannot be used for production!
    /// The interaction service provides a way for the UI to communicate with identityserver for validation and context retrieval
    /// </summary>
    public class AccountController : Controller
    {
        //private readonly InMemoryUserLoginService _loginService;
        private readonly ILoginService<ApplicationUser> _loginService;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly ILogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IOptionsSnapshot<AppSettings> _settings;

        private readonly IEmailSender _emailSender;

        public AccountController(

            IOptionsSnapshot<AppSettings> settings,
        //InMemoryUserLoginService loginService,
        ILoginService<ApplicationUser> loginService,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            ILoggerFactory loggerFactory, 
            UserManager<ApplicationUser> userManager,
              IEmailSender emailSender)
        {
            _settings = settings;
            _loginService = loginService;
            _interaction = interaction;
            _clientStore = clientStore;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _userManager = userManager;
            _emailSender = emailSender;
        } 
         
        //
        // GET: /Account/RegisterUser
        [HttpGet]
        [AllowAnonymous] 
        public async Task<IActionResult> RegisterUser(string userName, string password)
        {
            string callbackUrl = string.Empty;


            if (ModelState.IsValid)
            {

                // delete authentication cookie
                await HttpContext.Authentication.SignOutAsync();

                // set this so UI rendering sees an anonymous user
                HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());


                var user = new ApplicationUser
                {
                    UserName = userName,
                    Email = userName, 

                }; 

                var result = await _userManager.CreateAsync(user, password);
               
                if (result.Errors.Count() > 0)
                {
                   
                    // If we got this far, something failed, redisplay form
                    return Ok(result);
                }
                if (result.Succeeded)
                {
                    // Send an email with this link
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = System.Net.WebUtility.UrlEncode(code);
                      callbackUrl = string.Format("Account/ConfirmEmail?userId={0}&code={1}", user.Id, code);


                }
                var actualUser = await _loginService.FindByUsername(userName);
                if (await _loginService.ValidateCredentials(user, password))
                {
                    AuthenticationProperties props = null;
                    //if (model.RememberMe)
                    //{
                    props = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddYears(10)                      
                    };
                    //};

                     // await _loginService.SignIn(actualUser);
                    // make sure the returnUrl is still valid, and if yes - redirect back to authorize endpoint



                  
                }

                
                
                return Ok("IsAuthenticated " + callbackUrl);


                //await _loginService.FindByUsername(userName);

            }
 
         
                return Ok("IsNotAuthenticated");
        }
        // GET: /Account/RegisterUser
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateEmailConfirmationTokenAsync(string userName)
        {
            try
            {
                var user = await _loginService.FindByUsername(userName);
                if (user == null)
                    return NotFound();

                // Send an email with this link
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                code = System.Net.WebUtility.UrlEncode(code);
                // Comment out following line to prevent a new user automatically logged on.
                // await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation(3, "User GenerateEmailConfirmationTokenAsync.");

                var callbackUrl = string.Format("Account/ConfirmEmail?userId={0}&code={1}", user.Id, code);


                return Ok(callbackUrl);
            }
            catch( Exception ex)
            {
                return Ok("Cannot Generate Token");
            }
        }
        // GET: /Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);

            ViewData["ReturnHomeUrl"] = _settings.Value.MvcClient;
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        /// <summary>
        /// Show login page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null)
            {
                // if IdP is passed, then bypass showing the login screen
                return ExternalLogin(context.IdP, returnUrl);
            }

            var vm = await BuildLoginViewModelAsync(returnUrl, context);
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["ReturnHomeUrl"] = _settings.Value.MvcClient;

            return View(vm);
        }

        /// <summary>
        /// Handle postback from username/password login
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _loginService.FindByUsername(model.Email);
                
                if (await _loginService.ValidateCredentials(user, model.Password))
                {
                    
                        // Require the user to have a confirmed email before they can log on.
                        if (!await _userManager.IsEmailConfirmedAsync(user))
                        {
                            ModelState.AddModelError(string.Empty,
                                          "You must have a confirmed email to log in.");
                        var vm1 = await BuildLoginViewModelAsync(model);
                        ViewData["ReturnUrl"] = model.ReturnUrl;
                        return View(vm1);
                    }
                    

                    AuthenticationProperties props = null;
                    if (model.RememberMe)
                    {
                        props = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddYears(10)
                        };
                    };

                    await _loginService.SignIn(user);
                    // make sure the returnUrl is still valid, and if yes - redirect back to authorize endpoint
                    if (_interaction.IsValidReturnUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return Redirect("~/");
                }

                ModelState.AddModelError("", "Invalid username or password.");
            }

            // something went wrong, show form with error
            ViewData["ReturnHomeUrl"] = _settings.Value.MvcClient;
            var vm = await BuildLoginViewModelAsync(model);
            ViewData["ReturnUrl"] = model.ReturnUrl;
            return View(vm);
        }

        async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl, AuthorizationRequest context)
        {
            var allowLocal = true;
            if (context?.ClientId != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(context.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;
                }
            }

            return new LoginViewModel
            {
                ReturnUrl = returnUrl,
                Email = context?.LoginHint,
            };
        }

        async Task<LoginViewModel> BuildLoginViewModelAsync(LoginViewModel model)
        {
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
            var vm = await BuildLoginViewModelAsync(model.ReturnUrl, context);
            vm.Email = model.Email;
            vm.RememberMe = model.RememberMe;
            return vm;
        }

        /// <summary>
        /// Show logout page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            if (User.Identity.IsAuthenticated == false)
            {
                // if the user is not authenticated, then just show logged out page
                return await Logout(new LogoutViewModel { LogoutId = logoutId });
            }

            //Test for Xamarin. 
            var context = await _interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                //it's safe to automatically sign-out
                return await Logout(new LogoutViewModel { LogoutId = logoutId });
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            var vm = new LogoutViewModel
            {
                LogoutId = logoutId
            };
            return View(vm);
        }

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutViewModel model)
        {
            var idp = User?.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
            if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider)
            {
                if (model.LogoutId == null)
                {
                    // if there's no current logout context, we need to create one
                    // this captures necessary info from the current logged in user
                    // before we signout and redirect away to the external IdP for signout
                    model.LogoutId = await _interaction.CreateLogoutContextAsync();
                }

                string url = "/Account/Logout?logoutId=" + model.LogoutId;
                try
                {
                    // hack: try/catch to handle social providers that throw
                    await HttpContext.Authentication.SignOutAsync(idp, new AuthenticationProperties { RedirectUri = url });
                }
                catch(Exception ex)
                {
                    _logger.LogCritical(ex.Message);
                }
            }

            // delete authentication cookie
            await HttpContext.Authentication.SignOutAsync();

            // set this so UI rendering sees an anonymous user
            HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interaction.GetLogoutContextAsync(model.LogoutId);

            return Redirect(logout?.PostLogoutRedirectUri);
        }

        public async Task<IActionResult> DeviceLogOut(string redirectUrl)
        {
            // delete authentication cookie
            await HttpContext.Authentication.SignOutAsync();

            // set this so UI rendering sees an anonymous user
            HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            return Redirect(redirectUrl);
        }

        /// <summary>
        /// initiate roundtrip to external authentication provider
        /// </summary>
        [HttpGet]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            if (returnUrl != null)
            {
                returnUrl = UrlEncoder.Default.Encode(returnUrl);
            }
            returnUrl = "/account/externallogincallback?returnUrl=" + returnUrl;

            // start challenge and roundtrip the return URL
            var props = new AuthenticationProperties
            {
                RedirectUri = returnUrl, 
                Items = { { "scheme", provider } }
            };
            return new ChallengeResult(provider, props);
        }


        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                  
                    //CardHolderName = model.User.CardHolderName,
                    //CardNumber = model.User.CardNumber,
                    //CardType = model.User.CardType,
                    //City = model.User.City,
                    //Country = model.User.Country,
                    //Expiration = model.User.Expiration,
                    //LastName = model.User.LastName,
                    //Name = model.User.Name,
                    //Street = model.User.Street,
                    //State = model.User.State,
                    //ZipCode = model.User.ZipCode,
                    //PhoneNumber = model.User.PhoneNumber,
                    //SecurityNumber = model.User.SecurityNumber
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Errors.Count() > 0)
                {
                    AddErrors(result);
                    // If we got this far, something failed, redisplay form
                    return View(model);
                }
            }

            if (returnUrl != null) {
                if (HttpContext.User.Identity.IsAuthenticated)
                    return Redirect(returnUrl);
                else
                    if (ModelState.IsValid)
                    return RedirectToAction("login", "account", new { returnUrl = returnUrl });
                else
                    return View(model);
            }

            return RedirectToAction("index", "home");
        }

        [HttpGet]
        public IActionResult Redirecting()
        {
            return View();
        }
        //
        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }
        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {

                await _emailSender.SendEmailAsync(model.Email, "Reset password",
                    $" Hi {model.Email},<br />"+
                    $"Your DriveDrop password has been reset.<br /> Please click on this <a href='{_settings.Value.MvcClient}'>link</a> to sign in.");


                //return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
                return Redirect(_settings.Value.MvcClient);
            }
            AddErrors(result);
            return View();
        }
        //
        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            ViewBag.returnUrl = _settings.Value.MvcClient  ;
            ViewData["ReturnHomeUrl"] = _settings.Value.MvcClient;
            return View();
        }
        //
        // POST: /Account/ForgotPassword
        [HttpPost] 
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            ViewBag.returnUrl = _settings.Value.MvcClient;
            ViewData["ReturnHomeUrl"] = _settings.Value.MvcClient;

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    ModelState.AddModelError("", "Somethign Wrong !" );
                    // Don't reveal that the user does not exist or is not confirmed
                    //return Ok("SomethignWrong");
                   // return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
                // Send an email with this link
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                await _emailSender.SendEmailAsync(model.Email, "Reset password",
                    $"Hi {model.Email},<br />"+
                   $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
                return View("ForgotPasswordConfirmation");
                //return Ok("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
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
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return Ok("SomethignWrong");
            }
             
            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {

                    return Ok("User changed their password successfully");

                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    //_logger.LogInformation(3, "User changed their password successfully.");
                    //return RedirectToAction(nameof(Index), new { Message = ManageMessageId.ChangePasswordSuccess });
                }
                AddErrors(result);
                return Ok(result);

                //return View(model);
            }
            // return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
            return Ok("SomethignWrong");
        }


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}