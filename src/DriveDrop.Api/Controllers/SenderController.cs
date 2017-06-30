using ApplicationCore.Entities.ClientAgregate;
using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;
using ApplicationCore.Interfaces;
using DriveDrop.Api.Infrastructure;
using DriveDrop.Api.Infrastructure.Services;
using DriveDrop.Api.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Api.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    public class SenderController : Controller
    {
        private readonly DriveDropContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IIdentityService _identityService;
        public SenderController(IHostingEnvironment env, DriveDropContext context, IIdentityService identityService)
        {
            _context = context;
            _env = env;
            _identityService = identityService;
        }


        // GET api/values/5
        [HttpGet]
        [Route("[action]/{id:int}")]
        public async Task<IActionResult> GetbyId(int id)
        { 
            try
            {
                // var customer = await _context.Customers.FindAsync(id);

                var customer = await _context.Customers
                    .Include(s => s.TransportType).Include(t => t.CustomerStatus).Include(s => s.CustomerType)
                .FirstOrDefaultAsync(x => x.Id == id);

                if (customer == null)
                    return StatusCode(StatusCodes.Status409Conflict, "SenderNotFound");

                return Ok(customer);
 

            }
            catch (Exception exe)
            {
                return BadRequest("DriverNotFound" + exe.Message);
            }

        }


        //PUT api/v1/[controller]/New
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> NewSender([FromBody]CustomerModel c) //, [FromBody]List<IFormFile> files)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var deliveryAddres = new Address(c.DeliveryStreet, c.DeliveryCity, "WA", "USA", c.DeliveryZipCode, c.DeliveryPhone, c.DeliveryContact, 0, 0);
                    var pickUpAddres = new Address(c.PickupStreet, c.PickupCity, "WA", "USA", c.PickupZipCode, c.PickupPhone, c.PickupContact, 0, 0);

                    var tmpUser = Guid.NewGuid().ToString();

                    var newCustomer = new Customer(tmpUser, c.FirstName, c.LastName, null, CustomerStatus.WaitingApproval.Id, c.Email, c.Phone, CustomerType.Sender.Id, 0, 0, 0);

                    _context.Add(newCustomer);
                    _context.SaveChanges(); 

                    var shipment = new Shipment(pickUpAddres, deliveryAddres, newCustomer, 0, 0, c.PriorityTypeId, c.PriorityTypeLevel, c.TransportTypeId ?? 0, c.Note, c.FilePath, "");
                    _context.Add(shipment);

                    _context.SaveChanges();

                    await _context.SaveChangesAsync();

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


                    var deliveryAddres = new Address(c.DeliveryStreet, c.DeliveryCity, "WA", "USA", c.DeliveryZipCode, c.DeliveryPhone, c.DeliveryContact, 0, 0);
                    var pickUpAddres = new Address(c.PickupStreet, c.PickupCity, "WA", "USA", c.PickupZipCode, c.PickupPhone, c.PickupContact, 0, 0);

                    var tmpUser = Guid.NewGuid().ToString();



                    var shipment = new Shipment(pickUpAddres, deliveryAddres, sender, 0, 0, c.PriorityTypeId, c.PriorityTypeLevel, c.TransportTypeId, c.Note, c.PickupPictureUri , "");
                    _context.Add(shipment);

                    _context.SaveChanges();

                    await _context.SaveChangesAsync();



                    //Guid extName = Guid.NewGuid();
                    ////saving files
                    //long size = files.Sum(f => f.Length);

                    //// full path to file in temp location
                    //var filePath = Path.GetTempFileName();
                    //var uploads = Path.Combine(_env.WebRootPath, "uploads\\img\\shipment");
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
                    //        fileName = string.Format("uploads\\img\\shipment\\{0}{1}", extName, extension);

                    //        using (var stream = new FileStream(filePath, FileMode.Create))
                    //        {
                    //            await formFile.CopyToAsync(stream);
                    //        }
                    //    }
                    //}
                    //if (!string.IsNullOrWhiteSpace(fileName))
                    //{
                    //    shipment.SetPickupPictureUri(fileName);
                    //    _context.SaveChanges();
                    //    await _context.SaveChangesAsync();
                    //}

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



        //public async Task<NewShipment> PrepareCustomerModel(NewShipment model)
        //{
        //    model.CustomerTypeList = await _context.CustomerTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToListAsync();
        //    model.TransportTypeList = await _context.TransportTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToListAsync();
        //    model.CustomerStatusList =await  _context.CustomerStatuses.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToListAsync();
        //    model.PriorityTypeList = await _context.PriorityTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToListAsync();

        //    return model;
        //}
        //public CustomerModel PrepareCustomerModel(CustomerModel model)
        //{
        //    model.CustomerTypeList = _context.CustomerTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
        //    model.TransportTypeList = _context.TransportTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
        //    model.CustomerStatusList = _context.CustomerStatuses.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

        //    model.PriorityTypeList = _context.PriorityTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

        //    return model;
        //}

    }
}
