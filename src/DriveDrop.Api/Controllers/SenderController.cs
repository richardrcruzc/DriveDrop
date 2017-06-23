using ApplicationCore.Entities.ClientAgregate;
using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;
using ApplicationCore.Interfaces;
using DriveDrop.Api.Infrastructure;
using DriveDrop.Api.ViewModels;
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

namespace DriveDrop.Web.Controllers
{
    [Route("api/v1/[controller]")]
    public class SenderController : Controller
    {
        private readonly IHostingEnvironment _env; 
        private readonly IImageService _imageService;
        private readonly IAppLogger<SenderController> _logger;

        private readonly DriveDropContext _context;

        //  private readonly IIdentityParser<ApplicationUser> _appUserParser;


        public SenderController(IHostingEnvironment env, IImageService imageService,
            IAppLogger<SenderController> logger, DriveDropContext context)
        {
            _env = env; 
            _imageService = imageService;
            _logger = logger;
            _context = context;
        }
        //PUT api/v1/[controller]/New
        [Route("[action]")]
        [HttpPut]
        public async Task<IActionResult> New([FromBody]CustomerModel c, [FromBody]List<IFormFile> files)
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



                    var shipment = new Shipment(pickUpAddres, deliveryAddres, newCustomer, 0, 0, c.PriorityTypeId, c.PriorityTypeLevel, c.TransportTypeId ?? 0, c.Note, "", "");
                    _context.Add(shipment);

                    _context.SaveChanges();  

                    await _context.SaveChangesAsync();


                    Guid extName = Guid.NewGuid();
                    //saving files
                    long size = files.Sum(f => f.Length);

                    // full path to file in temp location
                    var filePath = Path.GetTempFileName();
                    var uploads = Path.Combine(_env.WebRootPath, "uploads\\img\\sender");
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
                            fileName = string.Format("uploads\\img\\sender\\{0}{1}", extName, extension);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await formFile.CopyToAsync(stream);
                            }
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(fileName))
                    {
                        shipment.SetPickupPictureUri(fileName);
                        _context.SaveChanges();
                        await _context.SaveChangesAsync();
                    }
                    return Ok(newCustomer.Id);
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

            PrepareCustomerModel(c);
            return Ok(c);
        }

        //PUT api/v1/[controller]/results/1
        [Route("[action]/{id:int}")]
        [HttpGet]
        public async Task<IActionResult> Result(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers 
                .Include(d => d.ShipmentSenders).ThenInclude(ShipmentSenders => ShipmentSenders.PriorityType)
               .Include(d => d.ShipmentSenders).ThenInclude(ShipmentSenders => ShipmentSenders.ShippingStatus)
               .Include(d=>d.ShipmentSenders).ThenInclude(ShipmentSenders => ShipmentSenders.PickupAddress)
               .Include(d => d.ShipmentSenders).ThenInclude(ShipmentSenders => ShipmentSenders.DeliveryAddress)
               .Include(s => s.TransportType).Include(t => t.CustomerStatus).Include(s => s.CustomerType)
               .SingleOrDefaultAsync(m => m.Id == id);


            var tttp = customer.ShipmentSenders;


            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        //PUT api/v1/[controller]/New
        [Route("[action]")]
        [HttpPut]
        public async Task<IActionResult> SaveNewShipment([FromBody]NewShipment c, [FromBody]List<IFormFile> files) 
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



                    var shipment = new Shipment(pickUpAddres, deliveryAddres, sender, 0, 0, c.PriorityTypeId, c.PriorityTypeLevel, c.TransportTypeId, c.Note, "", "");
                    _context.Add(shipment);

                    _context.SaveChanges();

                    await _context.SaveChangesAsync();



                    Guid extName = Guid.NewGuid();
                    //saving files
                    long size = files.Sum(f => f.Length);

                    // full path to file in temp location
                    var filePath = Path.GetTempFileName();
                    var uploads = Path.Combine(_env.WebRootPath, "uploads\\img\\shipment");
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
                            fileName = string.Format("uploads\\img\\shipment\\{0}{1}", extName, extension);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await formFile.CopyToAsync(stream);
                            }
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(fileName))
                    {
                        shipment.SetPickupPictureUri(fileName);
                        _context.SaveChanges();
                        await _context.SaveChangesAsync();
                    }

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

            PrepareCustomerModel(c);

        //    return ViewComponent("NewShipment", c);

            return Ok(c);
        }

       
        private ActionResult Json(bool v, object allowGet)
        {
            throw new NotImplementedException();
        }

        public NewShipment PrepareCustomerModel(NewShipment model)
        {
            model.CustomerTypeList = _context.CustomerTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            model.TransportTypeList = _context.TransportTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            model.CustomerStatusList = _context.CustomerStatuses.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            model.PriorityTypeList = _context.PriorityTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            return model;
        }
        public CustomerModel PrepareCustomerModel(CustomerModel model)
        {
            model.CustomerTypeList = _context.CustomerTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            model.TransportTypeList = _context.TransportTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            model.CustomerStatusList = _context.CustomerStatuses.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            model.PriorityTypeList = _context.PriorityTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            return model;
        }

    }
}
