using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DriveDrop.Api.Infrastructure;
using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;
using ApplicationCore.Entities.ClientAgregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using DriveDrop.Api.ViewModels;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using DriveDrop.Api.Services;
using Microsoft.Extensions.Options;

namespace DriveDrop.Api.Controllers
{
    [Route("api/v1/[controller]")]
   // [Authorize]
    public class DriversController : Controller
    {
        private readonly DriveDropContext _context;
        private readonly IHostingEnvironment _env;

        private readonly ICustomerService _cService;
        private readonly IEmailSender _emailSender;
        private readonly IOptionsSnapshot<AppSettings> _settings;

        public DriversController(ICustomerService cService, IHostingEnvironment env, DriveDropContext context,
            IEmailSender emailSender, IOptionsSnapshot<AppSettings> settings)
        {
            _context = context;
            _env = env;
            _cService = cService;
            _emailSender = emailSender;
            _settings = settings;
        }

        
        [HttpGet]
        [Route("[action]/{fullName}")]
        public async Task<IActionResult> AutoComplete(string fullName)
        {
            try
            {

                if (fullName.Length < 3)
                    return NotFound("null");


                    var drivers = await _context.Customers
                 .Include(x => x.CustomerStatus)
                 .Where(x => x.CustomerTypeId == 3  && x.CustomerStatus.Id ==2
                 && ( x.FirstName.StartsWith(fullName) || x.LastName.StartsWith(fullName)))
                 .Select(x=> new CustomerInfoModel
                 {
                    CustomerStatus = x.CustomerStatus.Name,
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Id=x.Id,
                    Phone = x.Phone,
                    PhotoUrl=x.PersonalPhotoUri,
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

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> UpdateInfo([FromBody]CustomerInfoModel c)
        {
            var updateCustomer = _context.Customers
                .Include(x=>x.CustomerStatus)
                .Where(x => x.Id == c.Id).FirstOrDefault();
            if (updateCustomer != null)
            {

                updateCustomer.UpdateInfo(c.StatusId, c.FirstName, c.LastName, c.Email, c.PrimaryPhone, c.Phone, c.PhotoUrl, c.VerificationId);

                _context.Update(updateCustomer);
                await _context.SaveChangesAsync();
            }
            return CreatedAtAction(nameof(GetbyId), new { id = updateCustomer.Id }, null);

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
        public async Task<IActionResult> AssignDriver(int id, int shippingId)
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
                .Include(s => s.TransportType)
                .Include(t => t.CustomerStatus)
                .Include(s => s.CustomerType) 
                .Include(A=>A.Addresses)
                .Include(d=>d.DefaultAddress)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

                if (customer == null)
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
            var updateCustomer = _context.Customers
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
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> AddVehicleInfo([FromBody]VehicleInfoModel c)
        {
            var updateCustomer = _context.Customers.Where(x => x.Id == c.DriverId).FirstOrDefault();
            if (updateCustomer != null)
            {
                updateCustomer.UpdateVehicleInfo(c.lincensePictureUri, c.vehiclePhotoUri, c.insurancePhotoUri, c.vehicleTypeId, c.VehicleMake, c.VehicleModel, c.VehicleColor, c.VehicleYear);                
                 
                _context.Update(updateCustomer);
                await _context.SaveChangesAsync();
            }
            return CreatedAtAction(nameof(GetbyId), new { id = updateCustomer.Id }, null);

        }


        
        // POST api/values
        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> NewDriver(int id,[FromBody]DriverModel c )
        {
            try
            {
                if (c == null || !ModelState.IsValid)
                {
                    return BadRequest(ErrorCode.DriverInformationRequired.ToString());
                }
                 var exists = await  _context.Customers.FindAsync(id);
                if (exists !=null)
                {
                    return StatusCode(StatusCodes.Status409Conflict, ErrorCode.DriverIDInUse.ToString());
                }

                var defaultAddres = new Address(c.DeliveryStreet, c.DeliveryCity,c.DeliveryState,c.DeliveryCountry, c.DeliveryZipCode,"","",c.DeliveryLatitude,c.DeliveryLongitude);

                var tmpUser = Guid.NewGuid().ToString();

                var newCustomer = new Customer(tmpUser, c.FirstName, c.LastName,transportTypeId: c.TransportTypeId,statusId: CustomerStatus.WaitingApproval.Id, email: c.Email, phone: c.Phone,
                        customerTypeId: CustomerType.Driver.Id, maxPackage: c.MaxPackage  , pickupRadius: c.PickupRadius  ,
                       deliverRadius: c.DeliverRadius  , commission: 0, userName: c.UserEmail, 
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



                await _emailSender.SendEmailAsync(newCustomer.UserName, "DriveDrop account created",
                    $"{newCustomer.FullName}: your account have been create and your status is {newCustomer.CustomerStatus.Name}, " +
                    $"we'll review you application ASAP, you can access your account by clicking here: <a href='{_settings.Value.MvcClient}'>link</a>");



                return Ok(newCustomer.Id);
                 


            }
            catch (Exception)
            {
                return BadRequest(ErrorCode.CouldNotCreateDriver.ToString());
            }
            
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
