using ApplicationCore.Entities.ClientAgregate;
using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;
using ApplicationCore.Interfaces;
using DriveDrop.Bl.Infrastructure;
using DriveDrop.Bl.Services;
using DriveDrop.Bl.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DriveDrop.Bl.Data;
using DriveDrop.Bl.ViewModels;
using Microsoft.AspNetCore.Identity;
using DriveDrop.Bl.Extensions;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DriveDrop.Bl.Controllers
{
    //[Authorize]
    [Route("[controller]/[action]")]
    public class SenderController : Controller
    {

        private readonly IShippingService _shippingService;
        private readonly IPictureService _pictureService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IIdentityParser<ApplicationUser> _appUserParser;
        private readonly DriveDropContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IIdentityService _identityService;
        private readonly IRateService _rateService;
        private readonly ICustomerService _cService;

        private readonly IEmailSender _emailSender;
        private readonly IOptionsSnapshot<AppSettings> _settings;


        public SenderController(ICustomerService cService, IHostingEnvironment env, DriveDropContext context, IIdentityService identityService, 
            IRateService rateService, IEmailSender emailSender, IOptionsSnapshot<AppSettings> settings, IIdentityParser<ApplicationUser> appUserParser,
            UserManager<ApplicationUser> userManager,   IPictureService pictureService, IShippingService shippingService)
        {
            _shippingService = shippingService;
            _pictureService = pictureService;
            _userManager = userManager;
            _context = context;
            _env = env;
            _identityService = identityService;
            _rateService = rateService;
            _cService = cService;
            _emailSender = emailSender;
            _settings = settings;
            _appUserParser = appUserParser;
        }


        [HttpGet]
      
        public async Task<IActionResult> ShippingDetails(int id)
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var currentUser = await _cService.Get(user.Email);
            if (currentUser == null)
            {
                return NotFound();
            }
            var shipping = currentUser.ShipmentSenders.Where(x => x.Id == id).FirstOrDefault();

            if (string.IsNullOrWhiteSpace(shipping.PickupPictureUri))
                shipping.PickupPictureUri = "profile-icon.png";
            if (string.IsNullOrWhiteSpace(shipping.DeliveredPictureUri))
                shipping.DeliveredPictureUri = "profile-icon.png";
            
            shipping.PickupPictureUri =_pictureService.DisplayImage(shipping.PickupPictureUri);
            shipping.DeliveredPictureUri = _pictureService.DisplayImage(shipping.DeliveredPictureUri);

            shipping.ShippingStatusList =  _context.ShippingStatuses.OrderBy(x => x.Name)
                .Select(x=> new SelectListItem {Value = x.Id.ToString(), Text = x.Name, Selected = shipping.ShippingStatusId == x.Id })
                .ToList();
            return View(shipping);
        }

        [HttpGet]
      
        public async Task<IActionResult> NewShipping (int? id)
        {  
            var model = new NewShipment { CustomerId = id??0};
            await PrepareCustomerAddresses(model, id??0);
           await PrepareCustomerModel(model);

            return View(model);

        }
        [HttpPost]
      
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewShipping(NewShipment c, IFormFile photoUrl)
        {
            var results = await AddShipping(c, photoUrl);

            if (string.IsNullOrEmpty(results))
            {

                var shipments = await _shippingService.GetShippingByCustomerId(c.CustomerId);
                var lastShipment = shipments.OrderByDescending(x => x.Id).FirstOrDefault();
                if(lastShipment.Discount<=lastShipment.AmountPay)
                         return RedirectToAction("PostToPayPalAsync", new { item = "Charge per Shipping Service", amount = c.TotalCharge, customerId = c.CustomerId });
            }
            ModelState.AddModelError("", results);
            await PrepareCustomerAddresses(c, c.CustomerId);
            await PrepareCustomerModel(c);
            return View(c);

        }
        //        [HttpPost]
        //      
        //        [ValidateAntiForgeryToken]
        //        public async Task<IActionResult> NewShippingFromBody([FromBody]NewShipment c, [FromBody]IFormFile photoUrl)
        //        {

        //        }

        public async Task<string> AddShipping(NewShipment c, IFormFile photoUrl)
        {
            var result = string.Empty;
            if (c.PickupAddressId == 0)
            {
                if (string.IsNullOrEmpty(c.PickupStreet))
                    result+= "Select a Pickup Address ";
                if (string.IsNullOrEmpty(c.PickupPhone))
                    result += "Select a Pickup Phone ";
                if (string.IsNullOrEmpty(c.PickupContact))
                    result += "Select a Pickup Contact "; 
            }

            if (c.DropAddressId == 0)
            {

                if (string.IsNullOrEmpty(c.DeliveryStreet))
                    result += "Select a Drop Address " ;
                if (string.IsNullOrEmpty(c.DeliveryPhone))
                    result += "Select a Drop Phone " ;
                if (string.IsNullOrEmpty(c.DeliveryContact))
                    result += "Select a Drop Contact " ; 
            }
            if (c.PackageSizeId == 0)
            {
                result += "Select Package Size" ;
            }

            if (c.PriorityTypeId == 0)
            { result += "Select Package Priority" ; }

            if (c.Amount == 0)
            {
                result += "Select package value" ;
            }
            if (c.ShippingWeight == 0)
            {
                result += "Select Package Weight" ;
            }
            if (photoUrl == null || photoUrl.Length <= 0)
            {
                return "Upload package photo";
            }
            if (!string.IsNullOrEmpty(result))
                return result;

            var user = _appUserParser.Parse(HttpContext.User);

            c.PickupPictureUri = await SaveFile(photoUrl, "Shipment");

            var positive = await _shippingService.SaveNewShipment(c);
            if (positive <= 0)
                return "Something went wrong";

            return result;
        }

        [HttpGet]
      
        public async Task<IActionResult> Shippings(int id)
        {
            //call shipping api service
            var user = _appUserParser.Parse(HttpContext.User);
            var currentUser = await _cService.Get(user.Email);
            if (currentUser == null)
            {
                return NotFound();
            }
            ViewBag.Id = currentUser.Id;
            var shippings = currentUser.ShipmentSenders;

            return View(shippings);
        }

        // GET api/values/5
        [HttpGet]
        [Route("[action]/userName/{userName}/customerId/{customerId:int}")]
        public async Task<IActionResult> GetSender(string userName, int customerId)
        {
             
            try
            {
                var customer = await _cService.Get(userName, customerId);
                
                
                if (customer == null || !customer.IsValid)
                    return StatusCode(StatusCodes.Status409Conflict, "SenderNotFound");


                if(customer.CustomerTypeId !=2)
                    return StatusCode(StatusCodes.Status409Conflict, "SenderNotFound");

                return  new JsonResult(customer);


            }
            catch (Exception exe)
            {
                return BadRequest("DriverNotFound" + exe.Message);
            }

        }

        // GET api/values/5
        [HttpGet]
        [Route("[action]/{userName}")]
        public async Task<IActionResult> GetByUserName(string userName)
        {
            try
            {
                var customer = await _cService.Get(userName);
 

                if (customer == null)
                    return StatusCode(StatusCodes.Status409Conflict, "SenderNotFound");

                if (customer.CustomerTypeId != 2)
                    return StatusCode(StatusCodes.Status409Conflict, "SenderNotFound");


                return  new JsonResult(customer);


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

                var customer = await _cService.Get(id);


                if (customer == null)
                    return StatusCode(StatusCodes.Status409Conflict, "SenderNotFound");

                return  new JsonResult(customer);
 

            }
            catch (Exception exe)
            {
                return BadRequest("DriverNotFound" + exe.Message);
            }

        }

      
        [HttpPost]
        public async Task<IActionResult> UpdateInfo(CustomerInfoModel c)
        {
            
            return await GoUpdateInfo(c);

        }
        [HttpPost]
        public async Task<IActionResult> UpdateInfoFromBody([FromBody]CustomerInfoModel c)
        {
            return await GoUpdateInfo(c);

        }
        private async Task<IActionResult> GoUpdateInfo(CustomerInfoModel c)
        {
            if (ModelState.IsValid)
            {
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

                    var updateCustomer = _context.Customers.OrderBy(i => i.Id).Where(x => x.Id == c.Id).FirstOrDefault();
                    if (updateCustomer != null)
                    {
                        string photoUrl = string.Empty;
                        if (c.PhotoUrl != null)
                        {
                            photoUrl = await SaveFile(c.PhotoUrl, "Sender");
                        }
                        updateCustomer.UpdateInfo(c.StatusId, c.FirstName, c.LastName, c.Email, c.PrimaryPhone, c.Phone, photoUrl, c.VerificationId);

                        _context.Update(updateCustomer);
                        await _context.SaveChangesAsync();
                        return new JsonResult("Info updated");
                    }
                }
            }
            var errorMsg = string.Empty;
            foreach (var state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
                errorMsg += state.Errors.ToString();
            }
            return new JsonResult(errorMsg);

        }

        [Route("[action]/{customerId:int}/{addressId:int}")]
        [HttpPost]
        public async Task<IActionResult> DeleteAddress(int customerId, int addressId)
        {            
            var updateCustomer = _context.Customers.OrderBy(i => i.Id)
                .Include(a=>a.Addresses)
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
        [AllowAnonymous]      
        public IActionResult NewSender()
        {
            //await HttpContext.Authentication.SignOutAsync("Cookies");
            //await  HttpContext.Authentication.SignOutAsync("oidc");

            SenderRegisterModel c = new SenderRegisterModel
            {
                CustomerTypeId = 2
            };

            return View(c);
        }
        private async Task<string> AddSender(SenderRegisterModel c) //, [FromBody]List<IFormFile> files)
        {   
            try
            {
                if (c.FromXamarin == false)
                {
                    foreach (var state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
                    {
                        var tt = state.Errors.ToString();
                    }
                    if (c.ImgeFoto == null || c.ImgeFoto.Length <= 0)
                    {
                        return "Upload profile photo";
                    }
                    if (c.ImgeFoto.Length > 1048576)
                    {

                        return "profile photo file exceeds the file maximum size: 1MB";
                    }
                }
                if (c.FromXamarin == false)
                    if (c == null || !ModelState.IsValid)
                    {
                        var msgError = string.Empty;
                        IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                        foreach (var er in allErrors)
                            msgError += " " + er.ErrorMessage;
                        return msgError;
                    }
                
                    var user = new ApplicationUser { UserName = c.UserEmail, Email = c.UserEmail };
                    var result = await _userManager.CreateAsync(user, c.Password);
                    if (result.Succeeded)
                    {
                        if (c.FromXamarin == false)
                            c.PersonalPhotoUri = await SaveFile(c.ImgeFoto, "sender");

                        //   var deliveryAddres = new Address(c.DeliveryStreet, c.DeliveryCity, c.DeliveryState, c.DeliveryCountry, c.DeliveryZipCode, c.DeliveryPhone, c.DeliveryContact, 0, 0);
                        var pickUpAddres = new Address(c.PickupStreet, c.PickupCity, c.PickupState, c.PickupCountry, c.PickupZipCode, "", "", c.PickupLatitude, c.PickupLongitude);

                        var tmpUser = Guid.NewGuid().ToString();

                        var newCustomer = new Customer(tmpUser, c.FirstName, c.LastName, null, CustomerStatus.Active.Id, email: c.UserEmail, phone: c.Phone,
                            customerTypeId: CustomerType.Sender.Id, maxPackage: 0, pickupRadius: 0,
                           deliverRadius: 0, commission: 0, userName: c.UserEmail,
                           primaryPhone: c.PrimaryPhone, driverLincensePictureUri: "", personalPhotoUri: c.PersonalPhotoUri,
                           vehiclePhotoUri: "", insurancePhotoUri: "",
                            vehicleMake: "", vehicleModel: "", vehicleColor: "", vehicleYear: "");

                        _context.Add(newCustomer);
                        _context.SaveChanges();
                         
                        newCustomer.AddAddress(pickUpAddres);
                        newCustomer.AddDefaultAddress(pickUpAddres);

                        _context.Update(newCustomer);
                        await _context.SaveChangesAsync();



                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                        Task.Factory
                       .StartNew(async () =>                     
                                //await _emailSender.SendEmailConfirmationAsync(newCustomer.UserName, callbackUrl)

                                await _emailSender.SendEmailAsync(newCustomer.UserName, "Confirm Email Address for New Account",
                                $"Hi {newCustomer.FullName} !You have been sent this email because you created an account on our website. "+
                                $"Please click on <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>this link</a> to confirm your email address is correct")

                     
                            //await _emailSender.SendEmailAsync(newCustomer.UserName, "DriveDrop account created",
                            //    $"{newCustomer.FullName}: your account have been create and your status is {CustomerStatus.Active.Name},<br /> <br />  " +
                            //    $"Your login infomation:<br /> Email: {newCustomer.UserName}<br /> Password:{c.Password} <br /><br />  " +
                            //    $" you can access your account by clicking here: <a href='{_settings.Value.MvcClient}'>link</a>")
                        )
                        .Wait();         

                        return   "new sender created" ;
                    }
                
            }
            catch (DbUpdateException ex)
            {
                //Log the error (uncomment ex variable name and write a log.
                var error = string.Format("Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator. {0}", ex.Message);

                return error;
            }
            return  "UnableToSaveChanges"  ;
        }

      
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> NewSender(SenderRegisterModel c) //, [FromBody]List<IFormFile> files)
        {
            var result = await AddSender(c);
            if(result=="new sender created")
            return RedirectToAction("NewSenderResults","Sender", new { user = c.UserEmail });

            ModelState.AddModelError("", "Unable to register Login infomation user: " + c.UserEmail);
            return View(c);
             
        }
        //PUT [controller]/New
      
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> NewSenderFromBody([FromBody]SenderRegisterModel c) //, [FromBody]List<IFormFile> files)
        {

            c.UserEmail = System.Net.WebUtility.UrlDecode(c.UserEmail);
            c.Password = System.Net.WebUtility.UrlDecode(c.Password);

            var results = await AddSender(c);
            return Ok(c);
        }

        //PUT [controller]/New
      
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SaveNewShipment([FromBody]NewShipment c) //, [FromBody]List<IFormFile> files) 
        {

            try
            {
                foreach (var state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
                {
                    var tt = state.Errors.ToString();
                }

                if (ModelState.IsValid)
                {

                    var sender = _context.Customers.Find(c.CustomerId);


                    var deliveryAddres = new Address(c.DeliveryStreet, c.DeliveryCity, c.DeliveryState, c.DeliveryCountry, c.DeliveryZipCode, c.DeliveryPhone, c.DeliveryContact,c.DeliveryLatitude,c.DeliveryLongitude);
                    var pickUpAddres = new Address(c.PickupStreet, c.PickupCity, c.PickupState, c.PickupCountry, c.PickupZipCode, c.PickupPhone, c.PickupContact,c.PickupLatitude, c.PickupLongitude);

                    foreach (var s in sender.Addresses)
                    {
                        if (s.Equals(deliveryAddres))
                        {
                            deliveryAddres = s;
                             
                        }

                        if (s.Equals(pickUpAddres))
                        {

                            pickUpAddres = s;
                            
                        }
                    }



                    

                    var tmpUser = Guid.NewGuid().ToString();

                    //var rate = await _rateService.CalculateAmount(int.Parse(c.PickupZipCode), int.Parse(c.DeliveryZipCode), c.ShippingWeight, 1, c.PriorityTypeId, c.TransportTypeId  , c.PromoCode);
                    var rate = await _rateService.CalculateAmount(c.Distance, c.ShippingWeight??0, c.PriorityTypeId, c.PromoCode, c.PackageSizeId, c.ExtraCharge ?? 0, c.ExtraChargeNote, c.PickupState, c.PickupCity);

                    var shipment = new Shipment(pickup: pickUpAddres, delivery: deliveryAddres, sender: sender, amount: c.Amount??0, discount: rate.Discount,
                     shippingWeight: c.ShippingWeight??0, priorityTypeId: c.PriorityTypeId, transportTypeId: c.TransportTypeId  , note: c.Note, pickupPictureUri: c.PickupPictureUri, deliveredPictureUri: "",
                     distance: rate.Distance, chargeAmount: rate.AmountToCharge, promoCode: c.PromoCode, tax: rate.TaxAmount,packageSizeId: c.PackageSizeId,
                     extraCharge: c.ExtraCharge ?? 0, extraChargeNote:c.ExtraChargeNote, needaVanOrPickup: c.NeedaVanOrPickup);
                     
                    _context.Add(shipment);

                    _context.SaveChanges();

                    await _context.SaveChangesAsync();

                     

                    return  new JsonResult(sender.Id);

                    //return RedirectToAction("result", new { id = sender.Id });
                    //return CreatedAtAction(nameof(Result), new { id = sender.Id }, null);
                }
            }
            catch (DbUpdateException ex)
            {
                //Log the error (uncomment ex variable name and write a log.
                var error = string.Format("Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator. {0}", ex.Message);
                return NotFound(error);
                //ModelState.AddModelError("", error);
            }
            var customer = await _context.Customers.FirstOrDefaultAsync();

            //await PrepareCustomerModel(c);

            //    return ViewComponent("NewShipment", c);

            return  new JsonResult(c);
        }
      
        public async Task<IActionResult> Result(int? id)
        {

            var user = _appUserParser.Parse(HttpContext.User);
            var currentUser = await _cService.Get(user.Email);
          
            if (currentUser == null)
            {
                return NotFound();
            }

           // currentUser.CustomerStatus = currentUser.CustomerStatus.ToTitleCase();
            return View(currentUser);
        }
        public async Task<string> SaveFile(IFormFile file, string belong)
        {
            var fileNameGuid = await _pictureService.UploadImage(file, belong);
            return fileNameGuid;


        }
        [AllowAnonymous]
      
        public IActionResult NewSenderResults(string user)
        {
            return View("NewSenderResults", user);
        }

        [NonAction]
        public async Task<NewShipment> PrepareCustomerAddresses(NewShipment model, int id)
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var currentUser = await _cService.Get(user.Email);

            var addressesP = new List<SelectListItem>();
            var addressesD = new List<SelectListItem>();

            foreach (var a in currentUser.Addresses)
            {
                var stringAddress = string.Format("{0}, {1}, {2}, {3}, {4} ",
                     a.Street, a.City, a.State, a.ZipCode, a.Country);

                if (a.TypeAddress.ToLower() == "pickup" || a.TypeAddress.ToLower() == "home")
                    addressesP.Add(new SelectListItem()
                    {
                        Value = a.Id.ToString(),
                        Text = stringAddress
                    });

                else
                    addressesD.Add(new SelectListItem()
                    {
                        Value = a.Id.ToString(),
                        Text = stringAddress
                    });

            }
            addressesP.Add(new SelectListItem() { Value = null, Text = "Add new address", Selected = true });
            addressesD.Add(new SelectListItem() { Value = null, Text = "Add new address", Selected = true });

            model.PickupAddresses = addressesP;
            model.DropAddresses = addressesD;

            if (currentUser.Addresses.Where(pa => pa.TypeAddress.ToLower() == "pickup" || pa.TypeAddress.ToLower() == "home").Any())
            {
                var a = currentUser.Addresses.Where(p => p.TypeAddress.ToLower() == "pickup" || p.TypeAddress.ToLower() == "home").FirstOrDefault();
                model.PickupAddressId = a.Id;
                model.PickupStreet = a.Street;
                model.PickupCity = a.City;
                model.PickupState = a.State;
                model.PickupCountry = a.Country;
                model.PickupZipCode = a.ZipCode;
                model.PickupPhone = a.Phone;
                model.PickupContact = a.Contact;
            }
            if (currentUser.Addresses.Where(a => a.TypeAddress.ToLower() == "drop").Any())
            {
                var d = currentUser.Addresses.Where(a => a.TypeAddress.ToLower() == "drop").FirstOrDefault();
                model.DropAddressId = d.Id;
                model.DeliveryStreet = d.Street;
                model.DeliveryCity = d.City;
                model.DeliveryState = d.State;
                model.DeliveryCountry = d.Country;
                model.DeliveryZipCode = d.ZipCode;
                model.DeliveryPhone = d.Phone;
                model.DeliveryContact = d.Contact;
            }


            return model;
        }


        [NonAction]
        public async Task<NewShipment> PrepareCustomerModel(NewShipment model)
        {
            var types = await _context.CustomerTypes.Select(x => new { Id = x.Id.ToString(), x.Name }).ToListAsync();

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
             
            var transport = await _context.TransportTypes.Select(x => new { Id = x.Id.ToString(), x.Name }).ToListAsync();
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


            var PriorityTypes = await _context.PriorityTypes.Select(x => new { Id = x.Id.ToString(), x.Name }).ToListAsync();
            var PriorityTypesList = new List<SelectListItem>
            {
                new SelectListItem() { Value = null, Text = "Select Priority Shipping", Selected = true }
            };


            foreach (var brand in PriorityTypes)
            {
                PriorityTypesList.Add(new SelectListItem()
                {
                    Value = brand.Id,
                    Text = brand.Name
                });
            }
            model.PriorityTypeList = PriorityTypesList;


            var getAllPackageSizes = await _context.PackageSizes.Select(x => new { Id = x.Id.ToString(), x.Name }).ToListAsync();
            var PackageSizesList = new List<SelectListItem>
            {
                new SelectListItem() { Value = null, Text = "Select a Package Size", Selected = true }
            };


            foreach (var brand in getAllPackageSizes)
            {
                PackageSizesList.Add(new SelectListItem()
                {
                    Value = brand.Id,
                    Text = brand.Name
                });
            }
            model.PackageSizeList = PackageSizesList;

            return model;
        }
      
        public async Task<IActionResult> PostToPayPalAsync(string item, string amount, int customerId)
        {
            if (customerId > 0)
                return RedirectToAction("Shippings", "sender", new { id= customerId});


           var user = _appUserParser.Parse(HttpContext.User);
            var customer =await  _cService.Get(user.Email);
            if (customer != null)
            {
            }

            var lastShipment = customer.ShipmentSenders.OrderByDescending(x => x.Id).LastOrDefault();
            if (lastShipment == null)
            { }

            ViewBag.CustomerId = customerId;

            var paypal = new Paypal
            {
                cmd = "_xclick",
                //paypal.cmd = "_cart";
                business = _settings.Value.BusinessAccountKey
            };
            bool useSandBox = _settings.Value.UseSandbox;
           // if (useSandBox)
                ViewBag.actionURL = "https://www.sandbox.paypal.com/cgi-bin/webscr?";
            //else
            //    ViewBag.actionURL = "https://www.paypal.com/cgi-bin/webscr?";

            paypal.cancel_return = string.Format(_settings.Value.CancelURL, customerId);
            paypal.returN = string.Format(_settings.Value.ReturnURL, customerId);
            paypal.notify_url = _settings.Value.NotifyURL;
            paypal.currency_code = _settings.Value.CurrencyCode;
            paypal.item_name = item;
            paypal.item_number = lastShipment.IdentityCode;
            paypal.invoice = lastShipment.IdentityCode;
            paypal.amount = lastShipment.ChargeAmount.ToString();
            paypal.price_per_item = lastShipment.ChargeAmount.ToString();
            paypal.discount = lastShipment.Discount.ToString();
            paypal.custom = lastShipment.DriverId.ToString();
            paypal.invoice = lastShipment.IdentityCode;
            paypal.tax = lastShipment.Tax.ToString();
            paypal.no_shipping = "2";
            paypal.rm = "2";
            paypal.no_note = "1";
            paypal.charset = "utf-8";

            return View(paypal);
             


        }
    }
}
