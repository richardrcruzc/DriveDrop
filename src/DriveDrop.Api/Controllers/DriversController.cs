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


        [HttpPut]
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
                _context.Update(driver);

                await _context.SaveChangesAsync();

                return Ok("driverAssigned");
            }

            catch (Exception)
            {
                return BadRequest("CouldNotAssignDriver");
            }
        }
        [HttpPut]
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
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {

            var root =  _context.Customers
                .Where(c => c.CustomerTypeId == CustomerType.Driver.Id);

            root = root.Where(c => c.Id == id);

            try
            {
                var driver = await root
                  .Include(d => d.ShipmentDrivers).ThenInclude(ShipmentDrivers => ShipmentDrivers.PriorityType)
               .Include(d => d.ShipmentDrivers).ThenInclude(ShipmentDrivers => ShipmentDrivers.ShippingStatus)
               .Include(d => d.ShipmentDrivers).ThenInclude(ShipmentDrivers => ShipmentDrivers.PickupAddress)
               .Include(d => d.ShipmentDrivers).ThenInclude(ShipmentDrivers => ShipmentDrivers.DeliveryAddress)
               .Include(d => d.ShipmentDrivers).ThenInclude(ShipmentDrivers => ShipmentDrivers.Sender)
               .Include(s => s.TransportType).Include(t => t.CustomerStatus).Include(s => s.CustomerType)
                    .FirstOrDefaultAsync();
            if(driver==null)
                return StatusCode(StatusCodes.Status409Conflict, ErrorCode.DriverNotFound.ToString());

            return Ok(driver);
            }
            catch (Exception)
            {
                return BadRequest(ErrorCode.DriverNotFound.ToString());
            }
            
        }

        // POST api/values
        [HttpPost("{id:int}")]
        public async Task<IActionResult> Post(int id,[FromBody]DriverModel c, [FromBody]List<IFormFile> files)
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



                Guid extName = Guid.NewGuid();
                //saving files
                long size = files.Sum(f => f.Length);

                // full path to file in temp location
                var filePath = Path.GetTempFileName();
                var uploads = Path.Combine(_env.WebRootPath, "uploads\\img\\driver");
                var fileName = "";

                foreach (var formFile in files)
                {

                    if (formFile.Length > 0)
                    {
                        var extension = ".jpg";
                        if (formFile.FileName.ToLower().EndsWith(".jpg"))
                            extension = ".jpg";
                        if (formFile.FileName.ToLower().EndsWith(".tif"))
                            extension = ".tif";
                        if (formFile.FileName.ToLower().EndsWith(".png"))
                            extension = ".png";
                        if (formFile.FileName.ToLower().EndsWith(".gif"))
                            extension = ".gif";




                        filePath = string.Format("{0}\\{1}{2}", uploads, extName, extension);
                        fileName = string.Format("uploads\\img\\driver\\{0}{1}", extName, extension);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    newCustomer.AddPicture(fileName);
                    _context.Update(newCustomer);
                    await _context.SaveChangesAsync();
                }



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
