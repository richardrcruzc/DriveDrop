using ApplicationCore.Entities.ClientAgregate;
using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;
using ApplicationCore.Interfaces;
using DriveDrop.Api.Infrastructure;
using DriveDrop.Api.Infrastructure.Services;
using DriveDrop.Api.Services;
using DriveDrop.Api.ViewModels;
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

namespace DriveDrop.Api.Controllers
{
    //[Authorize]
    [Route("api/v1/[controller]")]
    public class SenderController : Controller
    {
        private readonly DriveDropContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IIdentityService _identityService;
        private readonly IRateService _rateService;
        private readonly ICustomerService _cService;

        private readonly IEmailSender _emailSender;
        private readonly IOptionsSnapshot<AppSettings> _settings;


        public SenderController(ICustomerService cService, IHostingEnvironment env, DriveDropContext context, IIdentityService identityService, 
            IRateService rateService, IEmailSender emailSender, IOptionsSnapshot<AppSettings> settings)
        {
            _context = context;
            _env = env;
            _identityService = identityService;
            _rateService = rateService;
            _cService = cService;
            _emailSender = emailSender;
            _settings = settings;
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

                return Ok(customer);


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


                return Ok(customer);


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

                return Ok(customer);
 

            }
            catch (Exception exe)
            {
                return BadRequest("DriverNotFound" + exe.Message);
            }

        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> UpdateInfo([FromBody]CustomerInfoModel c)
        {
            var updateCustomer = _context.Customers.Where(x => x.Id == c.Id).FirstOrDefault();
            if (updateCustomer != null)
            {

                updateCustomer.UpdateInfo(c.StatusId, c.FirstName, c.LastName, c.Email, c.PrimaryPhone, c.Phone, c.PhotoUrl, c.VerificationId);

                _context.Update(updateCustomer);
                await _context.SaveChangesAsync();
            }
            return CreatedAtAction(nameof(GetbyId), new { id = updateCustomer.Id }, null);

        }



        [Route("[action]/{customerId:int}/{addressId:int}")]
        [HttpPost]
        public async Task<IActionResult> DeleteAddress(int customerId, int addressId)
        {            
            var updateCustomer = _context.Customers
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

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> AddAddress([FromBody]AddressModel c)
        {
            var updateCustomer = _context.Customers.Where(x => x.Id == c.CustomerId).FirstOrDefault();
            if (updateCustomer != null)
            {
                var addres = new Address(c.Street, c.City, c.State, c.Country, c.ZipCode, c.Phone, c.Contact, c.Longitude, c.Longitude, c.TypeAddress);

                updateCustomer.AddAddress(addres);

                _context.Update(updateCustomer);
                await _context.SaveChangesAsync();
            }
            return CreatedAtAction(nameof(GetbyId), new { id = updateCustomer.Id }, null);

        }
        //PUT api/v1/[controller]/New
        [Route("[action]")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> NewSender([FromBody]SenderRegisterModel c) //, [FromBody]List<IFormFile> files)
        {
            try
            {
                if (ModelState.IsValid)
                {
                 //   var deliveryAddres = new Address(c.DeliveryStreet, c.DeliveryCity, c.DeliveryState, c.DeliveryCountry, c.DeliveryZipCode, c.DeliveryPhone, c.DeliveryContact, 0, 0);
                    var pickUpAddres = new Address(c.PickupStreet, c.PickupCity, c.PickupState, c.PickupCountry, c.PickupZipCode,"","",c.PickupLatitude,c.PickupLongitude);

                    var tmpUser = Guid.NewGuid().ToString();

                    var newCustomer = new Customer(tmpUser, c.FirstName, c.LastName, null, CustomerStatus.Active.Id, email:c.Email,phone: c.Phone,
                        customerTypeId:CustomerType.Sender.Id, maxPackage:  0,pickupRadius:  0, 
                       deliverRadius:  0,commission: 0,userName: c.UserEmail,  
                       primaryPhone: c.PrimaryPhone, driverLincensePictureUri:  "",personalPhotoUri: c.PersonalPhotoUri,
                       vehiclePhotoUri: "", insurancePhotoUri: "",
                        vehicleMake: "", vehicleModel: "", vehicleColor: "", vehicleYear: "");
                    
                    _context.Add(newCustomer);
                    _context.SaveChanges();


                    //newCustomer.AddAddress(deliveryAddres);
                    newCustomer.AddAddress(pickUpAddres);
                    newCustomer.AddDefaultAddress(pickUpAddres);

                    _context.Update(newCustomer);
                    await _context.SaveChangesAsync();


                    //var rate = await _rateService.CalculateAmount(int.Parse(c.PickupZipCode), int.Parse(c.DeliveryZipCode), c.ShippingWeight, 1, c.PriorityTypeId, c.TransportTypeId??0, c.PromoCode);
                    // var rate = await _rateService.CalculateAmount(c.Distance, c.ShippingWeight, c.PriorityTypeId, c.PromoCode,  c.PackageSizeId );


                    //var shipment = new Shipment(pickup: pickUpAddres, delivery: deliveryAddres, sender: newCustomer, amount: c.Amount, discount: rate.Discount,
                    //   shippingWeight: c.ShippingWeight, priorityTypeId: c.PriorityTypeId, transportTypeId: c.TransportTypeId??0, note: c.Note, pickupPictureUri: c.FilePath, 
                    //   deliveredPictureUri: "", distance: c.Distance, chargeAmount: rate.AmountToCharge, promoCode: c.PromoCode, tax: rate.TaxAmount, packageSizeId: c.PackageSizeId);


                    //_context.Add(shipment);

                    //_context.SaveChanges();

                    await _context.SaveChangesAsync();



                    //await _emailSender.SendEmailAsync(newCustomer.UserName, "DriveDrop account created",
                    //    $"{newCustomer.FullName}: your account have been create and your status is {CustomerStatus.Active.Name},<br /> <br />  " +
                    //    $"Your login infomation:<br /> Email: {newCustomer.UserName}<br /> Password:{c.Password} <br /><br />  " +
                    //    $" you can access your account by clicking here: <a href='{_settings.Value.MvcClient}'>link</a>");




                    return CreatedAtAction(nameof(GetbyId), new { id = newCustomer.Id }, null);
                     
                }
            }
            catch (DbUpdateException ex)
            {
                //Log the error (uncomment ex variable name and write a log.
                var error = string.Format("Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator. {0}", ex.Message);

                return NotFound("UnableToSaveChanges");
            }

            //PrepareCustomerModel(c);
            return Ok(c);
        }

        //PUT api/v1/[controller]/New
        [Route("[action]")]
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

                    var tmpUser = Guid.NewGuid().ToString();

                    //var rate = await _rateService.CalculateAmount(int.Parse(c.PickupZipCode), int.Parse(c.DeliveryZipCode), c.ShippingWeight, 1, c.PriorityTypeId, c.TransportTypeId  , c.PromoCode);
                    var rate = await _rateService.CalculateAmount(c.Distance, c.ShippingWeight, c.PriorityTypeId, c.PromoCode, c.PackageSizeId, c.ExtraCharge, c.ExtraChargeNote, c.PickupState, c.PickupCity);

                    var shipment = new Shipment(pickup: pickUpAddres, delivery: deliveryAddres, sender: sender, amount: c.Amount, discount: rate.Discount,
                     shippingWeight: c.ShippingWeight, priorityTypeId: c.PriorityTypeId, transportTypeId: c.TransportTypeId  , note: c.Note, pickupPictureUri: c.PickupPictureUri, deliveredPictureUri: "",
                     distance: rate.Distance, chargeAmount: rate.AmountToCharge, promoCode: c.PromoCode, tax: rate.TaxAmount,packageSizeId: c.PackageSizeId,
                     extraCharge: c.ExtraCharge, extraChargeNote:c.ExtraChargeNote);
                     
                    _context.Add(shipment);

                    _context.SaveChanges();

                    await _context.SaveChangesAsync();

                     

                    return Ok(sender.Id);

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

            return Ok(c);
        }



       

    }
}
