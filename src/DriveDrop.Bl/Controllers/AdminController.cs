using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities.ClientAgregate;
using AutoMapper;
using DriveDrop.Bl.Data;
using DriveDrop.Bl.Models;
using DriveDrop.Bl.Services;
using DriveDrop.Bl.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DriveDrop.Bl.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IShippingService _shippingService;
        private readonly IPictureService _pictureService;
        private readonly ICustomerService _cService;
        private readonly DriveDropContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IIdentityService _identityService;

        private readonly IEmailSender _emailSender;

        private readonly IOptionsSnapshot<AppSettings> _settings;

        private readonly IMapper _mapper;
        private readonly IIdentityParser<ApplicationUser> _appUserParser;

        public AdminController(ICustomerService cService, IHostingEnvironment env, DriveDropContext context,
            IIdentityService identityService, IEmailSender emailSender, IOptionsSnapshot<AppSettings> settings,
            IMapper mapper, IIdentityParser<ApplicationUser> appUserParser, IShippingService shippingService,
            IPictureService  pictureService)
        {
            _pictureService = pictureService;
            _emailSender = emailSender;
            _cService = cService;
            _context = context;
            _env = env;
            _identityService = identityService;
            _settings = settings;
            _mapper = mapper;
            _appUserParser = appUserParser;
            _shippingService = shippingService;
        }
        [Route("[action]")]
        public async Task<IActionResult> ChangShippingStatus(int shippingId, int statusId)
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var admin = await _cService.Get(user.Email);
            if (!admin.IsAdmin)
            {
                return new JsonResult("somthing wrong");
            }

          var result =   await _shippingService.UpdatePackageStatus(shippingId, statusId);

            return new JsonResult(result);
        }
        [Route("[action]")]
        public async Task<IActionResult> ShippingDetails(int id)
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var admin = await _cService.Get(user.Email);
            if (!admin.IsAdmin)
            {
                return View(new ShipmentModel());
            }
            var shipments =await  _shippingService.GetById(id);

            shipments.DeliveredPictureUri =_pictureService.DisplayImage(shipments.DeliveredPictureUri);
            shipments.PickupPictureUri = _pictureService.DisplayImage(shipments.PickupPictureUri);
            shipments.DeliveredPictureUri = _pictureService.DisplayImage(shipments.DeliveredPictureUri);
            shipments.DropPictureUri = _pictureService.DisplayImage(shipments.DropPictureUri);
            shipments.ShippingStatusList = _context.ShippingStatuses
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name, Selected = x.Id== shipments.ShippingStatusId })
                .ToList();

            return View(shipments);
        }

        [Route("[action]")]
        public async Task<IActionResult> DriverDetails(int? id)
        {
            if (id == null)
            {
                return new JsonResult("invalid option");
            }
            var user =   _appUserParser.Parse(HttpContext.User);
            var currentUser =await  _cService.Get(id??0);

            if (currentUser == null)
            {
                return new JsonResult("driver not found");
            }
            return View(currentUser);
        }
        [Route("[action]")]
        public async Task<IActionResult> SenderDetails(int? id)
        {
            if (id == null)
            {
                return new JsonResult("invalid option");
            }
            var user = _appUserParser.Parse(HttpContext.User);
            var currentUser = await _cService.Get(id ?? 0);

            if (currentUser == null)
            {
                return new JsonResult("driver not found");
            }
            return View(currentUser);
        }

        [Route("[action]")]
        public async Task<IActionResult> Shippings(int? PriorityTypeFilterApplied, int? ShippingStatusFilterAApplied, string IdentityCode, int? page)
        {
            var model = await _shippingService.GetShipping(ShippingStatusFilterAApplied ?? 0, PriorityTypeFilterApplied ?? 0, IdentityCode);

            return View(model);
        }

        [HttpPost]
        [Route("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateInfo(UpdateInfoModel c,  IFormFile  PersonalPhotoUri)
        {
            var result = "Info not updated";
            if (ModelState.IsValid)
            {
                try
                {
                    if (PersonalPhotoUri != null && PersonalPhotoUri.Length > 0)
                    {
                        if (PersonalPhotoUri.Length > 1048576)
                        {

                            return new JsonResult("profile photo file exceeds the file maximum size: 1MB");
                        }
                    }

                  
                    
                    var user = _appUserParser.Parse(HttpContext.User);
                    

                  var customer = await _cService.Get(user.Email);
                    if (customer != null)
                    {
                        var updateCustomer = _context.Customers.OrderBy(i => i.Id)
                          .Include(x => x.CustomerStatus)
                          .Where(x => x.Id == c.Id).FirstOrDefault();

                        var fileName = string.Empty;
                        if (PersonalPhotoUri != null && PersonalPhotoUri.Length > 0)
                              fileName = await SaveFile(PersonalPhotoUri, "driver");

                        updateCustomer.UpdateInfo(c.CustomerStatusId, c.FirstName, c.LastName, c.UserName, c.PrimaryPhone, c.Phone, fileName, c.VerificationId);

                        _context.Update(updateCustomer);
                        await _context.SaveChangesAsync();
                        result = "Info updated";

                    }
                }
                catch (DbUpdateException ex)
                {
                    //Log the error (uncomment ex variable name and write a log.
                    var error = string.Format("Unable to save changes. " +
                        "Try again, and if the problem persists " +
                        "see your system administrator. {0}", ex.Message);

                    ModelState.AddModelError("", error);
                    result = error;
                }
            }


            return new JsonResult(result);
        }

        [Route("[action]")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var user =_appUserParser.Parse(HttpContext.User);


            var currentUser = await _cService.Get(user.Email);
            if (currentUser == null)
            {
                return NotFound();
            }

            if (!currentUser.IsAdmin)
                return NotFound("Invalid entry");

            var customer = await _cService.Get(id??0);
            if (!customer.IsAdmin)
                return NotFound("Invalid entry");



            return View(customer);

        }
        [Route("[action]")]
         public async Task<IActionResult> Index(int? customertype, int? statusId, int? transporTypeId, string LastName = "", int pageIndex = 0, int pageSize = 10)
        {
            var items = await _cService.Get(customertype, statusId, transporTypeId, LastName, pageIndex, pageSize);

            ViewBag.CustomerStatus = items.CustomerStatus;
            return View(items);

        }
        [HttpGet]
       [Route("[action]/type/{customertype}/status/{statusId}/transporType/{transporTypeId}/LastName/{LastName}")]
        public async Task<IActionResult> Items(int? customertype, int? statusId, int? transporTypeId, string LastName="", int pageIndex = 0,int pageSize = 10)
        {
            var index =await _cService.Get(customertype, statusId,   transporTypeId,   LastName ,  pageIndex  ,  pageSize );
              

            return  new JsonResult(index);

        }



        [HttpGet]
        [Route("[action]/adminUser/{adminUser}")]
        public async Task<IActionResult> EndImpersonated(string adminUser)
        {
            var isImpersonated = await _cService.EndImpersonated(adminUser);
            return  new JsonResult(isImpersonated);
        }

        [HttpGet]
        [Route("[action]/adminUser/{adminUser}/userName/{userName}")]
        public async Task<IActionResult> SetImpersonate(string adminUser, string userName)
        {
            var isImpersonated = await _cService.SetImpersonate(adminUser, userName);
            return  new JsonResult(isImpersonated);
        }



        [HttpGet]
        [Route("[action]/CustomerId/{CustomerId:int}/statusId/{statusId:int}")]
        public async Task<IActionResult> ChangeCustomerStatus(int CustomerId, int statusId)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(CustomerId);
                if (customer == null)
                    return  new JsonResult("CustomerNoFound");

                var status = await _context.CustomerStatuses.FindAsync(statusId);

                if (status == null)
                    return  new JsonResult("CustomerstatusNoFound");


                customer.ChangeStatus(status);
                _context.Update(customer);
                await _context.SaveChangesAsync();

                await _emailSender.SendEmailAsync(customer.UserName, "Status updated",
                    $"{customer.FullName}: your status have been updated, access your account by clicking here: <a href='{_settings.Value.MvcClient}'>link</a>");

                return new JsonResult("CustomerstatusChanged");
            }
            catch (Exception exe)
            {
                return BadRequest("DriverNotFound" + exe.Message);
            }
        }


        
        // GET api/values/5
        [HttpGet]
        [Route("[action]/{id:int}")]
        public async Task<IActionResult> GetbyId(int id)
        {


            try
            {
                var model = await _cService.Get(id);

                //   var model = await _context.Customers
                //    .Include(s => s.TransportType)
                //    .Include(t => t.CustomerStatus)
                //    .Include(s => s.CustomerType)
                //    .Include(a => a.Addresses)
                //    .Include("ShipmentDrivers.ShippingStatus")
                //    .Include("ShipmentDrivers.PickupAddress")
                //    .Include("ShipmentDrivers.DeliveryAddress")
                //    .Include("ShipmentSenders.ShippingStatus")
                //        .Include("ShipmentSenders.PickupAddress")
                //    .Include("ShipmentSenders.DeliveryAddress")
                //    .Where(x => x.Id == id)
                //.FirstOrDefaultAsync(); 

                return  new JsonResult(model);

            }
            catch (Exception exe)
            {
                return BadRequest("DriverNotFound" + exe.Message);
            }

        }


        [HttpGet]
        [Route("[action]/{userName}")]
        public async Task<IActionResult> GetbyUserName(string userName)
        {
            var c = await _cService.Get(userName);
            if (c == null)
                return  new JsonResult("CustomerNoFound");


            return  new JsonResult(c);
        }
        [NonAction]
        public async Task<string> SaveFile(IFormFile file, string belong)
        {
            var fileNameGuid = await _pictureService.UploadImage(file, belong);
            return fileNameGuid;


        }
    }
}