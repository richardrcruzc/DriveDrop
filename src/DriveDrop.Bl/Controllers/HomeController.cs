using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DriveDrop.Bl.Models;
using DriveDrop.Bl.ViewModels.ManageViewModels;
using DriveDrop.Bl.Services;
using DriveDrop.Bl.Data;
using DriveDrop.Bl.ViewModels; 
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DriveDrop.Bl.Controllers
{
    public class HomeController : Controller
    { 
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DriveDropContext _context;
        private readonly IIdentityParser<ApplicationUser> _appUserParser;
        private readonly ICustomerService _cService;
        public HomeController(ICustomerService cService, 
            IIdentityParser<ApplicationUser> appUserParser,
            DriveDropContext context, SignInManager<ApplicationUser> signInManager,
             UserManager<ApplicationUser> userManager )
        {
            _appUserParser = appUserParser;
            _cService = cService;
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager; 
        }

        [Route("identity")]
        [Authorize]
            [HttpGet]
            public IActionResult Get()
            {
                return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
            }
        

        //[Route("ProvideData")]
        //[HttpGet]
        public ActionResult ProvideData(string data = "test msq", string connectionName = "default")
        {

            //byte[] buffer = Encoding.UTF8.GetBytes(data);
            //ArraySegment<byte> segment = new ArraySegment<byte>(buffer);

            //WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync(); // _httpContextAccesor.HttpContext.WebSockets.AcceptWebSocketAsync();
            //await webSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);

            if (!string.IsNullOrEmpty(data) && !string.IsNullOrEmpty(connectionName))
            {


                //_httpContextAccesor.HttpContext.Items.Add("ConnectionName", connectionName);
                //_httpContextAccesor.HttpContext.Items.Add("Data", data);

                // WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                HttpContext.Request.Path = "/ws";
                HttpContext.Items.Add("ConnectionName", connectionName);
                HttpContext.Items.Add("Data", data);
            }
            return Ok("OK");
        }


        public async Task<IActionResult> MyAccount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var currentUser = await  _cService.Get(user.Email); 

            if (currentUser != null)
            {
                if (currentUser.IsAdmin)
                    return RedirectToAction("index", "admin");
                else
                {

                    if (currentUser.CustomerTypeId == 1)
                        return RedirectToAction("index", "admin", new { id = currentUser.Id });
                    else if (currentUser.CustomerTypeId == 2)
                        return RedirectToAction("result", "sender", new { id = currentUser.Id });
                    else if (currentUser.CustomerTypeId == 3)
                        return RedirectToAction("result", "driver", new { id = currentUser.Id });

                }
            }

            ViewBag.UserValid = "false";
            return RedirectToAction("Index");
        }



        public async Task<IActionResult> Index()
        { 

            if (_signInManager.IsSignedIn(User))
            //if (User.Identity.IsAuthenticated)
            {
                var user = _userManager.GetUserName(User);
                //var user = _appUserParser.Parse(HttpContext.User);
                var currentUser = await _cService.Get(user);
                
                if (currentUser == null)
                    return RedirectToAction("Signout", "account");
                else
                    if (currentUser.IsAdmin)
                    return RedirectToAction("index", "admin");
                else
                {

                    if (currentUser.CustomerTypeId == 1)
                        return RedirectToAction("index", "admin", new { id = currentUser.Id });
                    else if (currentUser.CustomerTypeId == 2)
                        return RedirectToAction("result", "sender", new { id = currentUser.Id });
                    else if (currentUser.CustomerTypeId == 3)
                        return RedirectToAction("result", "driver", new { id = currentUser.Id });

                }

            }

            var model = new HomeQuote();

            var tTypes = await _context.TransportTypes.Select(x => new { Id = x.Id.ToString(),  x.Name }).ToListAsync();

            var transportTypes = new List<SelectListItem>
            {
                new SelectListItem() { Value = null, Text = "Fit in...", Selected = true }
            };


            foreach (var brand in tTypes)
            {
                transportTypes.Add(new SelectListItem()
                {
                    Value = brand.Id.ToString(),
                    Text = brand.Name
                });
            }
            model.TransportTypeList = transportTypes;

            var packageSize = new List<SelectListItem>
            {
                new SelectListItem() { Value = null, Text = "Select Size", Selected = true }
            };


            var allSices = await _context.PackageSizes.Select(x => new { Id = x.Id.ToString(),  x.Name }).ToListAsync();


            foreach (var brand in allSices)
            {
                packageSize.Add(new SelectListItem()
                {
                    Value = brand.Id.ToString(),
                    Text = brand.Name
                });
            }
            model.PackageSizeList = packageSize;



            var priorityType = new List<SelectListItem>
            {
                new SelectListItem() { Value = null, Text = "Select Priority", Selected = true }
            };


            var allPriorities = await _context.PriorityTypes.Select(x => new { Id = x.Id.ToString(), x.Name }).ToListAsync();


            foreach (var brand in allPriorities)
            {
                priorityType.Add(new SelectListItem()
                {
                    Value = brand.Id.ToString(),
                    Text = brand.Name
                });
            }
            model.PriorityTypeList = priorityType;

            return View(model);
        }


        public JsonResult SearchCity(string seach)
        {
            var countries = new List<SelectListItem>
         {
             new   SelectListItem()
                {
                    Value ="United States",
                    Text ="United States"
                } ,
              new   SelectListItem()
                {
                    Value ="Canada",
                    Text ="Canada"
                }
         };

            return Json(countries);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Test()
        {


            return View();
        }
        public IActionResult Register()
        {


            return View();
        }
        [HttpGet]
        public IActionResult Testchat()
        {
            return View("InsertUserName");
        }

        [HttpPost]
        public IActionResult Testchat(string username)
        {
            return View("Testchat", username);
        }




        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
        [NonAction]
        async Task<string> GetUserTokenAsync()
        {
            await Task.Delay(100);
            return string.Empty;

            //var context = _httpContextAccesor.HttpContext;

            // return await context.GetTokenAsync("access_token");
        }

    }
}
