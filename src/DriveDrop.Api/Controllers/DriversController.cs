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

namespace DriveDrop.Api.Controllers
{
    [Route("api/v1/[controller]")]
   [Authorize]
    public class DriversController : Controller
    {
        private readonly DriveDropContext _context;
        private readonly IHostingEnvironment _env;
        public DriversController(IHostingEnvironment env, DriveDropContext context)
        {
            _context = context;
            _env = env;
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
                .Include(s => s.TransportType).Include(t => t.CustomerStatus).Include(s => s.CustomerType) 
                .FirstOrDefaultAsync(x => x.Id == id);

                if (customer == null)
                    return StatusCode(StatusCodes.Status409Conflict, "DriverNotFound");
                return Ok(customer);


                //var root = await _context.Customers
                // .Include(d => d.ShipmentDrivers).ThenInclude(ShipmentDrivers => ShipmentDrivers.PriorityType)
                // .Include(s => s.TransportType).Include(t => t.CustomerStatus).Include(s => s.CustomerType)
                // //.Include(d => d.ShipmentDrivers)
                // //.Include(d => d.ShipmentDrivers).ThenInclude(ShipmentDrivers => ShipmentDrivers.ShippingStatus)
                // //.Include(d => d.ShipmentDrivers).ThenInclude(ShipmentDrivers => ShipmentDrivers.PickupAddress)
                // //.Include(d => d.ShipmentDrivers).ThenInclude(ShipmentDrivers => ShipmentDrivers.DeliveryAddress)
                // //.Include(d => d.ShipmentDrivers).ThenInclude(ShipmentDrivers => ShipmentDrivers.Sender)
                // // .Include(d => d.ShipmentSenders)
                // //.Include(d => d.ShipmentSenders).ThenInclude(ShipmentSenders => ShipmentSenders.ShippingStatus)
                // //.Include(d => d.ShipmentSenders).ThenInclude(ShipmentSenders => ShipmentSenders.PickupAddress)
                // //.Include(d => d.ShipmentSenders).ThenInclude(ShipmentSenders => ShipmentSenders.DeliveryAddress)
                // //.Include(d => d.ShipmentSenders).ThenInclude(ShipmentSenders => ShipmentSenders.Sender)

                // .FirstOrDefaultAsync(x => x.Id == id);

                //if (root == null)
                //    return StatusCode(StatusCodes.Status409Conflict, "DriverNotFound");

                //var name = root.FullName;

                ////var shipmentDrivers = root.ShipmentDrivers.ToList();
                ////var shipmentSenders = root.ShipmentSenders.ToList();


                //var customer = new CustomerViewModel
                //{
                //    Id = root.Id,
                //    Commission = root.Commission,
                //    CustomerStatus = root.CustomerStatus.Name,
                //    CustomerStatusId = root.CustomerStatusId,
                //    CustomerType = root.CustomerType.Name,
                //    CustomerTypeId = root.CustomerTypeId,
                //    DeliverRadius = root.DeliverRadius,
                //    DriverLincensePictureUri = root.DriverLincensePictureUri,
                //    Email = root.Email,
                //    FirstName = root.FirstName,
                //    IdentityGuid = root.IdentityGuid,
                //    LastName = root.LastName,
                //    MaxPackage = root.MaxPackage,
                //    Phone = root.Phone,
                //    PickupRadius = root.PickupRadius,
                //    TransportType = root.TransportType.Name,
                //    TransportTypeId = root.TransportTypeId,
                //    UserGuid = root.UserGuid,
                //   // ShipmentDrivers = root.ShipmentDrivers,
                //   // ShipmentSenders = root.ShipmentSenders
                //    //ShipmentDrivers = shipmentDrivers,
                //    //ShipmentSenders = shipmentSenders

                //};
                //return Ok(customer);
            }
            catch (Exception exe)
            {
                return BadRequest("DriverNotFound" + exe.Message);
            }

        }

        // POST api/values
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

                var defaultAddres = new Address(c.DeliveryStreet, c.DeliveryCity, "WA", "USA", c.DeliveryZipCode, c.DeliveryPhone, c.DeliveryContact, 0, 0);

                var tmpUser = Guid.NewGuid().ToString();

                var newCustomer = new Customer(tmpUser, c.FirstName, c.LastName, c.TransportTypeId, CustomerStatus.WaitingApproval.Id, c.Email, c.Phone, CustomerType.Driver.Id, c.MaxPackage ?? 0, c.PickupRadius ?? 0, c.DeliverRadius ?? 0, c.Commission);

                _context.Add(newCustomer);
                _context.SaveChanges();

                newCustomer.AddDefaultAddress(defaultAddres);

                _context.Update(newCustomer);
               
                await _context.SaveChangesAsync();
                return Ok(newCustomer.Id);


                //Guid extName = Guid.NewGuid();
                ////saving files
                //long size = files.Sum(f => f.Length);

                //// full path to file in temp location
                //var filePath = Path.GetTempFileName();
                //var uploads = Path.Combine(_env.WebRootPath, "uploads\\img\\driver");
                //var fileName = "";

                //foreach (var formFile in files)
                //{

                //    if (formFile.Length > 0)
                //    {
                //        var extension = ".jpg";
                //        if (formFile.FileName.ToLower().EndsWith(".jpg"))
                //            extension = ".jpg";
                //        if (formFile.FileName.ToLower().EndsWith(".tif"))
                //            extension = ".tif";
                //        if (formFile.FileName.ToLower().EndsWith(".png"))
                //            extension = ".png";
                //        if (formFile.FileName.ToLower().EndsWith(".gif"))
                //            extension = ".gif";




                //        filePath = string.Format("{0}\\{1}{2}", uploads, extName, extension);
                //        fileName = string.Format("uploads\\img\\driver\\{0}{1}", extName, extension);

                //        using (var stream = new FileStream(filePath, FileMode.Create))
                //        {
                //            await formFile.CopyToAsync(stream);
                //        }
                //    }
                //}
                //if (!string.IsNullOrWhiteSpace(fileName))
                //{
                //    newCustomer.AddPicture(fileName);
                //    _context.Update(newCustomer);
                //    await _context.SaveChangesAsync();
                //}



            }
            catch (Exception)
            {
                return BadRequest(ErrorCode.CouldNotCreateDriver.ToString());
            }
            return Ok(c);
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
