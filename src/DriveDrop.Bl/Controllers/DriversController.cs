using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc; 
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using DriveDrop.Bl.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using DriveDrop.Bl.Services;
using Microsoft.Extensions.Options;
using ApplicationCore.Entities.Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using DriveDrop.Bl.Data;
using DriveDrop.Bl.ViewModels;
using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;
using ApplicationCore.Entities.ClientAgregate;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Encodings.Web;
using AutoMapper;

namespace DriveDrop.Bl.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class DriverController : Controller
    {

        private readonly IShippingService _shipping;
        private readonly IMapper _mapper;

        private readonly IPictureService _pictureService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IIdentityParser<ApplicationUser> _appUserParser;

        private readonly DriveDropContext _context;
        private readonly IHostingEnvironment _env;

        private readonly ICustomerService _cService;
        private readonly IEmailSender _emailSender;
        private readonly IOptionsSnapshot<AppSettings> _settings;

        public DriverController(ICustomerService cService, IHostingEnvironment env, DriveDropContext context,
            IEmailSender emailSender, IOptionsSnapshot<AppSettings> settings, IIdentityParser<ApplicationUser> appUserParser,
            UserManager<ApplicationUser> userManager, IPictureService pictureService, IMapper mapper, IShippingService shipping)
        {
            _mapper = mapper;
            _appUserParser = appUserParser;
            _pictureService = pictureService;
            _userManager = userManager;
            _context = context;
            _env = env;
            _cService = cService;
            _emailSender = emailSender;
            _settings = settings;
            _shipping = shipping;
        }
        public IActionResult Index()
        {

            return View();
        }
        public async Task<IActionResult> SetDropByInfo(int id)
        {
            var user = _appUserParser.Parse(HttpContext.User);
         
            var customer =  await _cService.Get(user.Email);
            if (customer == null)
                return NotFound();

            var package = customer.ShipmentDrivers.Where(x => x.Id == id).FirstOrDefault();

            return View(package);

        }

        [HttpPost]
        public async Task<IActionResult> SetDropByInfo(int Id, string DropNote, IFormFile photoUrl)

        {

            if (Id == 0)
                return NotFound();
            if (photoUrl != null && photoUrl.Length > 0)
            {
                if (photoUrl.Length > 1048576)
                {

                    return new JsonResult("Photo file exceeds the file maximum size: 1MB");
                }
            }
            try
            {
                var user = _appUserParser.Parse(HttpContext.User);
                var customer = await _cService.Get(user.Email);
                
                if (customer == null)
                    return new JsonResult("driver NotFound");
                @ViewBag.CustomerId = customer.Id;



                var package = await _context.Shipments.FindAsync(Id);
                if (package == null)
                    return NotFound();

                var fileName = await SaveFile(photoUrl, "driver");
                
                package.ChangeStatus(7);
                package.AddStatusHistory(new PackageStatusHistory(7,Id, customer.Id));
                package.SetDropInfo(fileName, DropNote);

                _context.Update(package);
                await _context.SaveChangesAsync();

                return new JsonResult("Info updated");
            }
            catch
            {
                return new JsonResult("PackageNotAccepted");
            }

        }
        public async Task<IActionResult> UpdateDropStatus(int shippingId, int shippingStatusId)
        {
            int customerId = 0;

            try
            {
                var user = _appUserParser.Parse(HttpContext.User);

                var package = await _context.Shipments.FindAsync(shippingId);

                package.ChangeStatus(shippingStatusId);
                package.AddStatusHistory(new PackageStatusHistory(shippingStatusId, shippingId, package.DriverId??0));

                _context.Update(package);
                await _context.SaveChangesAsync();

                customerId = package.DriverId ?? 0;
            }
            catch { }

            return RedirectToAction("PickedUp", new { id = customerId });

        }
        public async Task<IActionResult> AceptPackageFromSender(int id)
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var driver = await _cService.Get(user.Email);
            @ViewBag.CustomerId = driver.Id;

            var package = driver.ShipmentDrivers.Where(x => x.Id == id).FirstOrDefault();

            return View(package);
        }
        [HttpPost]
        public async Task<IActionResult> AceptPackageFromSender(int Id, string SecurityCode, IFormFile photoUrl)
        {
            try
            {
                var user = _appUserParser.Parse(HttpContext.User);
                var driver = await _cService.Get(user.Email);
                @ViewBag.CustomerId = driver.Id;

                if (photoUrl != null && photoUrl.Length > 0)
                {
                    if (photoUrl.Length > 1048576)
                    {

                        return new JsonResult("Photo file exceeds the file maximum size: 1MB");
                    }
                }


                var photo = await SaveFile(photoUrl, "Driver");

                var package = await _context.Shipments.FindAsync(Id);
                package.SetPickupPictureUri(photo);
                package.SetSecyurityCode(SecurityCode);
                package.ChangeStatus(4);  
  
                package.AddStatusHistory(new PackageStatusHistory(4, Id, driver.Id));

                _context.Update(package);
                await _context.SaveChangesAsync();

                return Json("PackageNotAccepted");
            }
            catch
            { }
            return Json("Somthing wrong happend");
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePackageStatus(UpdatePackageStatusModel m)
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var driver = await _cService.Get(user.Email);

           var results = await _shipping.UpdatePackageStatus(m.ShippingId, m.Item.ShippingStatusId);

            return RedirectToAction("PendingPickUp", new { id = driver.Id });
        }
            public async Task<IActionResult> PickedUp(int id)
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var driver = await _cService.Get(user.Email);

            var s = await _shipping.GetByDriverIdAndStatusId(driver.Id, new int[] { 4, 5, 6 });

            var model = new PaginatedShippings
            {
                Count = s.Count,
                CustomerId = id,
                Data = s.Data.ToList(),
                ListOne = s.ListOne,
                ListTwo = s.ListTwo,
                ListThree = s.ListThree,
                PageIndex = s.PageIndex,
                PageSize = s.PageSize,
                ShippingStatusList = await PrepareShippingStatus() 

            };
            ViewBag.CustomerId = driver.Id;
            return View(model); 

        }
        public async Task<IActionResult> PendingPickUp(int id)
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var driver = await _cService.Get(user.Email);

            var s = await _shipping.GetByDriverIdAndStatusId(driver.Id, new int[] { 2, 3 });

            var model = new PaginatedShippings
            {
                Count = s.Count,
                CustomerId = id,
                Data = s.Data.ToList(),
                ListOne = s.ListOne,
                ListTwo = s.ListTwo,
                ListThree = s.ListThree,
                PageIndex = s.PageIndex,
                PageSize = s.PageSize,
                ShippingStatusList = await PrepareShippingStatus() 

            };
            ViewBag.CustomerId = driver.Id;
            return View(model);
             

        }
        [HttpGet]
        public async Task<IActionResult> AssignDriver(int id)
        {
            try
            {

                var user = _appUserParser.Parse(HttpContext.User);
                var driver = await _cService.Get(user.Email);

                var shipping = _context.Shipments.Find(id);

                if (shipping == null)
                    return new JsonResult("Shipping no found");

                if (driver.CustomerTypeId != 3)
                    return new JsonResult(ErrorCode.DriverNotFound.ToString());

                var setDriver = await _context.Customers.FindAsync(driver.Id);
                if (setDriver != null)
                {
                    shipping.ChangeStatus(ShippingStatus.PendingPickUp.Id);
                    shipping.SetDriver(setDriver);

                    shipping.AddStatusHistory(new PackageStatusHistory(ShippingStatus.PendingPickUp.Id, id, driver.Id));

                    _context.Update(shipping);

                    await _context.SaveChangesAsync();
                }
                return new JsonResult("driverAssigned");
            }

            catch (Exception)
            {
            }
            return new JsonResult("something wrong");
        }

            public async Task<IActionResult> ReadyToPickUp(int id)
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var customer =await _cService.Get(user.Email);
            if (customer == null)
                return View(new PaginatedShippings());
            @ViewBag.CustomerId = customer.Id;

            var s = await _shipping.GetPackagesReadyForDriver(id);

            var model = new PaginatedShippings
            {
                Count = s.Count,
                CustomerId = id,
                Data = s.Data.ToList(),
                ListOne = s.ListOne,
                ListTwo = s.ListTwo,
                ListThree = s.ListThree,
                 PageIndex = s.PageIndex,
                 PageSize=s.PageSize,
                  ShippingStatusList = await PrepareShippingStatus()


            };
            ViewBag.DriverId = customer.Id;
            return View(model);
        }

        public async Task<IActionResult> Result(int? id)
        {
            if (id == null)
            {
                return NotFound("Invalid entry");
            }

            var user = _appUserParser.Parse(HttpContext.User);

            var currentUser = await _cService.Get(user.Email);

            if (currentUser == null)
            {
                return NotFound();
            }
            

            if (string.IsNullOrWhiteSpace(currentUser.PersonalPhotoUri))
                currentUser.PersonalPhotoUri = "profile-icon.png";

            //currentUser.PersonalPhotoUri = _settings.Value.PicBaseUrl.Replace("[0]", System.Net.WebUtility.UrlDecode(currentUser.PersonalPhotoUri)).Replace("//","/");


            var model = _mapper.Map<DriveDrop.Bl.ViewModels.CurrentCustomerModel, DriverInfoModel>(currentUser);
           
            return View(model);
        }

        [HttpGet]
        [Route("{fullName}")]
        public async Task<IActionResult> AutoComplete(string fullName)
        {
            try
            {

                if (fullName.Length < 3)
                    return NotFound("null");


                    var drivers = await _context.Customers
                 .Include(x => x.CustomerStatus)
                 .Where(x => x.CustomerTypeId == 3  && x.CustomerStatus.Id ==2 && x.Isdeleted==false
                 && ( x.FirstName.StartsWith(fullName) || x.LastName.StartsWith(fullName)))
                 .Select(x=> new CustomerInfoModel
                 {
                    CustomerStatus = x.CustomerStatus.Name,
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Id=x.Id,
                    Phone = x.Phone,
                    PersonalPhotoUri=x.PersonalPhotoUri,
                    PrimaryPhone=x.PrimaryPhone,
                    StatusId= x.CustomerStatus.Id,
                    VerificationId = x.VerificationId
                 })
                 .ToListAsync();

                
                return Ok(drivers);


            }
            catch (Exception exe)
            {
                return BadRequest("DriverNotFound" + exe.Message);
            }

        }

        [Route("[action]/driver/{id:int}/comission/{comission}")]
        [HttpPost]
        public async Task<IActionResult> UpdateComission(int id, decimal comission)
        {
            var updateCustomer = _context.Customers.OrderBy(i => i.Id)
                .Include(x => x.CustomerStatus)
                .Where(x => x.Id == id  ).FirstOrDefault();
            if (updateCustomer != null)
            {

                updateCustomer.UpdateCommission(comission);

                _context.Update(updateCustomer);
                await _context.SaveChangesAsync();
            }
            return CreatedAtAction(nameof(GetbyId), new { id = updateCustomer.Id }, null);

        }

         
        [HttpPost]
        public async Task<IActionResult> UpdateInfo(DriverInfoModel c)
        {
            if (ModelState.IsValid)
            {
                if (c.PhotoUrl == null && c.PhotoUrl.Length > 0)
                {
                    return new JsonResult("profile photo file exceeds the file maximum size: 1MB");
                }
                    if (c.PhotoUrl != null && c.PhotoUrl.Length > 0)
                {
                    if (c.PhotoUrl.Length > 1048576)
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

                    var photoUrl = string.Empty;

                    if (c.PhotoUrl == null && c.PhotoUrl.Length > 0)
                         photoUrl = await SaveFile(c.PhotoUrl, "Driver");

                    updateCustomer.UpdateInfo(c.StatusId, c.FirstName, c.LastName, c.Email, c.PrimaryPhone, c.Phone, photoUrl, c.VerificationId);

                    _context.Update(updateCustomer);
                    await _context.SaveChangesAsync();
                    return new JsonResult("info updated");


                }
                }
            return new JsonResult("something wrong");



             
        }

        // GET api/values/5
        [HttpGet]
        [Route("[action]/{userName}")]
        public async Task<IActionResult> GetByUserName(string userName )
        {
            try
            {
                var customer = await _cService.Get(userName);


                if (customer == null || !customer.IsValid)
                    return StatusCode(StatusCodes.Status409Conflict, "DriverNotFound");


                if (customer.CustomerTypeId != 3)
                    return StatusCode(StatusCodes.Status409Conflict, "DriverNotFound");

                return Ok(customer);


            }
            catch (Exception exe)
            {
                return BadRequest("DriverNotFound" + exe.Message);
            }

        }
        // GET api/values/5
        [HttpGet]
        [Route("[action]/userName/{userName}/customerId/{customerId:int}")]
        public async Task<IActionResult> GetDriver(string userName, int customerId)
        {
            try
            {
                var customer = await _cService.Get(userName, customerId);


                if (customer == null || !customer.IsValid)
                    return StatusCode(StatusCodes.Status409Conflict, "DriverNotFound");


                if (customer.CustomerTypeId != 3)
                    return StatusCode(StatusCodes.Status409Conflict, "DriverNotFound");

                return Ok(customer);


            }
            catch (Exception exe)
            {
                return BadRequest("DriverNotFound" + exe.Message);
            }

        }
        [HttpGet]
        [Route("[action]/Customer/{id:int}/shipping/{shippingId:int}")]
        public async Task<IActionResult> ssAssignDriver(int id, int shippingId)
        {
            try
            {
                var shipping = _context.Shipments.Find(shippingId);

                if (shipping == null)
                    return StatusCode(StatusCodes.Status409Conflict, ErrorCode.DriverNotFound.ToString());

                var driver = _context.Customers.Find(id);
                if (driver == null)
                    return StatusCode(StatusCodes.Status409Conflict, ErrorCode.DriverNotFound.ToString());

                shipping.SetDriver(driver);
                shipping.ChangeStatus(ShippingStatus.PendingPickUp.Id);

                shipping.AddStatusHistory(new PackageStatusHistory(ShippingStatus.PendingPickUp.Id, shippingId, driver.Id));

                _context.Update(driver);

                await _context.SaveChangesAsync();

                return Ok("driverAssigned");
            }

            catch (Exception)
            {
                return BadRequest("CouldNotAssignDriver");
            }
        }
        [HttpPost]
        [Route("[action]/customer/{customerId:int}/shipping/{shippingId:int}/shippingStatus/{shippingStatusId:int}")]
        public async Task<IActionResult> ChangeShippingStatus(int customerId, int shippingId, int shippingStatusId)
        {
            try
            {
                var shipping = _context.Shipments.Find(shippingId);

            if (shipping == null)
                return RedirectToAction("Result", new { id = customerId });

            var driver = _context.Customers.Find(customerId);
            if (driver == null)
                return RedirectToAction("Result", new { id = customerId });

            shipping.ChangeStatus(shippingStatusId);
            _context.Update(shipping);
            await _context.SaveChangesAsync();

                return Ok("ShippingStatusChanged");
            }

            catch (Exception)
            {
                return BadRequest("cannotChangeShippingStatus");
    }
            
        }
     
        // GET api/values/5
        [HttpGet]
        [Route("[action]/{id:int}")]
        public async Task<IActionResult> GetbyId(int id)
        {

            try
            {

                //var customer = await _context.Customers.FindAsync(id);

                var customer = await _context.Customers
                    .OrderBy(i => i.Id)
                .Include(s => s.TransportType)
                .Include(t => t.CustomerStatus)
                .Include(s => s.CustomerType) 
                .Include(A=>A.Addresses)
                .Include(d=>d.DefaultAddress)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

                if (customer == null|| customer.Isdeleted)
                    return StatusCode(StatusCodes.Status409Conflict, "DriverNotFound");
                return Ok(customer);

                 
            }
            catch (Exception exe)
            {
                return BadRequest("DriverNotFound" + exe.Message);
            }

        }
        [Route("[action]/{customerId:int}/{addressId:int}")]
        [HttpPost]
        public async Task<IActionResult> DeleteAddress(int customerId, int addressId)
        {
            var updateCustomer = _context.Customers.OrderBy(i => i.Id)
                .Include(a => a.Addresses)
                .Where(x => x.Id == customerId).FirstOrDefault();
            if (updateCustomer != null)
            {
                foreach (var a in updateCustomer.Addresses)
                    if (a.Id == addressId)
                    {
                        updateCustomer.DeleteAddress(a);
                        _context.Update(updateCustomer);
                        await _context.SaveChangesAsync();

                        break;
                    }

            }


            return CreatedAtAction(nameof(GetbyId), new { id = updateCustomer.Id }, null);

        }
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> AddAddress([FromBody]AddressModel c)
        {
            var updateCustomer = _context.Customers.OrderBy(i => i.Id).Where(x => x.Id == c.CustomerId).FirstOrDefault();
            if (updateCustomer != null)
            {
                var addres = new Address(c.Street, c.City, c.State, c.Country, c.ZipCode, c.Phone, c.Contact, c.Longitude, c.Longitude, c.TypeAddress);

                updateCustomer.AddAddress(addres);

                _context.Update(updateCustomer);
                await _context.SaveChangesAsync();
            }
            return CreatedAtAction(nameof(GetbyId), new { id = updateCustomer.Id }, null);

        }
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> AddVehicleInfo([FromBody]VehicleInfoModel c)
        {
            var updateCustomer = _context.Customers.OrderBy(i => i.Id).Where(x => x.Id == c.DriverId).FirstOrDefault();
            if (updateCustomer != null)
            {
                updateCustomer.UpdateVehicleInfo(c.lincensePictureUri, c.vehiclePhotoUri, c.insurancePhotoUri, c.vehicleTypeId, c.VehicleMake, c.VehicleModel, c.VehicleColor, c.VehicleYear);                
                 
                _context.Update(updateCustomer);
                await _context.SaveChangesAsync();
            }
            return CreatedAtAction(nameof(GetbyId), new { id = updateCustomer.Id }, null);

        }
        [AllowAnonymous]
        [Route("[action]")]
        public async Task<IActionResult> NewDriver()
        {
            var model = new DriverModel();
            await PrepareCustomerModel(model);
            return View(model);
        }
        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> NewDriver(int id,  DriverModel c)
        {
            var result = await AddDriver(id,c);
            if(result == "new driver created")
                return RedirectToAction("NewDriverResults", "Driver", new { user = c.UserEmail });

            await PrepareCustomerModel(c);
            ModelState.AddModelError("", "Unable to register Login infomation user: " + c.UserEmail);
            return View(c);


        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> NewDriverFromBody([FromBody]DriverModel c)
        {

            c.UserEmail = System.Net.WebUtility.UrlDecode(c.UserEmail);
            c.Password = System.Net.WebUtility.UrlDecode(c.Password);

            var result = await AddDriver(0,c);
            return Ok(c);
        }


        
        private async Task<string> AddDriver(int id, DriverModel c )
        {
            try
            {
                if (c.FromXamarin == false)
                    if (c == null || !ModelState.IsValid)
                {
                    var msgError = string.Empty;
                    IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                    foreach (var er in allErrors)
                        msgError += " " + er.ErrorMessage;
                    return msgError;
                }
                if (c.FromXamarin == false)
                {
                    if (c.Personalfiles == null || c.Personalfiles[0].Length <= 0)
                    {
                        return "Upload profile photo";
                    }
                    if (c.Personalfiles[0].Length > 1048576)
                    {

                        return "profile photo file exceeds the file maximum size: 1MB";
                    }
                    if (c.Licensefiles == null || c.Licensefiles[0].Length <= 0)
                    {
                        return "Upload DriverLincensePicture photo";
                    }
                    if (c.Licensefiles[0].Length > 1048576)
                    {

                        return "Driver Lincense Picture file exceeds the file maximum size: 1MB";
                    }
                    if (c.Vehiclefiles == null || c.Vehiclefiles[0].Length <= 0)
                    {
                        return "Upload vehicle photo";
                    }
                    if (c.Vehiclefiles[0].Length > 1048576)
                    {

                        return "vehicle photo file exceeds the file maximum size: 1MB";
                    }
                    if (c.Insurancefiles == null || c.Insurancefiles[0].Length <= 0)
                    {
                        return "Upload Insurance photo";
                    }
                    if (c.Insurancefiles[0].Length > 1048576)
                    {

                        return "vehicle Insurance file exceeds the file maximum size: 1MB";

                    }
                }
                var user = new ApplicationUser { UserName = c.UserEmail, Email = c.UserEmail };
                var result = await _userManager.CreateAsync(user, c.Password);
                if (result.Succeeded)
                {

                    if (c.FromXamarin == false)
                    {
                        c.PersonalPhotoUri = await SaveFile(c.Personalfiles[0], "driver");
                        c.DriverLincensePictureUri = await SaveFile(c.Licensefiles[0], "driver");
                        c.VehiclePhotoUri = await SaveFile(c.Vehiclefiles[0], "driver");
                        c.InsurancePhotoUri = await SaveFile(c.Insurancefiles[0], "driver");
                    }

                    var defaultAddres = new Address(c.DeliveryStreet, c.DeliveryCity, c.DeliveryState, c.DeliveryCountry, c.DeliveryZipCode, "", "", c.DeliveryLatitude, c.DeliveryLongitude);

                    var tmpUser = Guid.NewGuid().ToString();

                    var newCustomer = new Customer(tmpUser, c.FirstName, c.LastName, transportTypeId: c.TransportTypeId, statusId: CustomerStatus.WaitingApproval.Id, email: c.UserEmail, phone: c.Phone,
                            customerTypeId: CustomerType.Driver.Id, maxPackage: c.MaxPackage, pickupRadius: c.PickupRadius,
                           deliverRadius: c.DeliverRadius, commission: 0, userName: c.UserEmail,
                           primaryPhone: c.PrimaryPhone, driverLincensePictureUri: c.DriverLincensePictureUri, personalPhotoUri: c.PersonalPhotoUri,
                           vehiclePhotoUri: c.VehiclePhotoUri, insurancePhotoUri: c.InsurancePhotoUri,
                           vehicleMake: c.VehicleMake, vehicleModel: c.VehicleModel, vehicleColor: c.VehicleColor, vehicleYear: c.VehicleYear);

                    _context.Add(newCustomer);
                    _context.SaveChanges();

                    newCustomer.AddAddress(defaultAddres);
                    newCustomer.AddDefaultAddress(defaultAddres);
                    newCustomer.AddPicture(c.InsurancePhotoUri, "insurance");

                    _context.Update(newCustomer);

                    await _context.SaveChangesAsync();


                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    Task.Factory
                   .StartNew(async () =>
                            //await _emailSender.SendEmailConfirmationAsync(newCustomer.UserName, callbackUrl)

                            await _emailSender.SendEmailAsync(newCustomer.UserName, "Confirm Email Address for New Account",
                            $"Hi {newCustomer.FullName} !You have been sent this email because you created an account on our website. " +
                            $"Please click on <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>this link</a> to confirm your email address is correct")


                    //await _emailSender.SendEmailAsync(newCustomer.UserName, "DriveDrop account created",
                    //    $"{newCustomer.FullName}: your account have been create and your status is {CustomerStatus.Active.Name},<br /> <br />  " +
                    //    $"Your login infomation:<br /> Email: {newCustomer.UserName}<br /> Password:{c.Password} <br /><br />  " +
                    //    $" you can access your account by clicking here: <a href='{_settings.Value.MvcClient}'>link</a>")
                    )
                    .Wait();


                    //Task.Factory
                    //  .StartNew(async () =>
                    //     await _emailSender.SendEmailAsync(newCustomer.UserName, "DriveDrop account created",
                    //          $"{newCustomer.FullName}: your account have been create and your status is {newCustomer.CustomerStatus.Name}, " +
                    //          $"we'll review you application ASAP, you can access your account by clicking here: <a href='{_settings.Value.MvcClient}'>link</a>")

                    //   )
                    //    .Wait();

                    return "new driver created";

                }

            }
            catch (Exception)
            {
                 return ErrorCode.CouldNotCreateDriver.ToString();
            }
            return "new driver not created";
        }

        [AllowAnonymous]
        [Route("[action]")]
        public IActionResult NewDriverResults(string user)
        {
            return View("NewDriverResults", user);
        }

        public async Task<DriverModel> PrepareCustomerModel(DriverModel model)
        {
            var types = await _context.CustomerTypes.Select(x => new { Id = x.Id.ToString(),  x.Name }).ToListAsync();

            var CustomerTypes = new List<SelectListItem>
            {
                new SelectListItem() { Value = null, Text = "All", Selected = true }
            };


            foreach (var brand in types)
            {
                CustomerTypes.Add(new SelectListItem()
                {
                    Value = brand.Id,
                    Text = brand.Name
                });
            }
            model.CustomerTypeList = CustomerTypes;
 

            var status = await _context.CustomerStatuses.Select(x => new { Id = x.Id.ToString(), Name = x.Name }).ToListAsync();

            var customerStatus = new List<SelectListItem>
            {
                new SelectListItem() { Value = null, Text = "All", Selected = true }
            };


            foreach (var brand in types)
            {
                customerStatus.Add(new SelectListItem()
                {
                    Value = brand.Id,
                    Text = brand.Name
                });
            }
            model.CustomerStatusList = customerStatus;



            
            var transport = await _context.TransportTypes.Select(x => new { Id = x.Id.ToString(),  x.Name }).ToListAsync();
            var transportTypes = new List<SelectListItem>
            {
                new SelectListItem() { Value = null, Text = "All Vehicle Types", Selected = true }
            };


            foreach (var brand in transport)
            {
                transportTypes.Add(new SelectListItem()
                {
                    Value = brand.Id,
                    Text = brand.Name
                });
            }
            model.TransportTypeList = transportTypes; 

             

            return model;

        }
        private async Task<string> SaveFile(IFormFile file, string belong)
        {
            var fileNameGuid = await _pictureService.UploadImage(file, belong);
            return fileNameGuid;


        }
        public async Task<List<SelectListItem>> PrepareShippingStatus()
        {
            var shippin = await _context.ShippingStatuses.Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name }).ToListAsync();

            shippin.Insert(0,new SelectListItem() { Value = null, Text = "All", Selected = true });

            return shippin;
        }
        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        public enum ErrorCode
        {
            DriverInformationRequired,
            DriverIDInUse,
            DriverNotFound,
            CouldNotCreateDriver,
            CouldNotUpdateDriver,
            CouldNotDeleteDriver,
            ShipmentNotFound,
            ShipmentAlreadyAssigned
        }
    }
}
