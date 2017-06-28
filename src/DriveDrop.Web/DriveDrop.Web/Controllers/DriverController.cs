 
using DriveDrop.Web.Infrastructure;
using DriveDrop.Web.Services;
using DriveDrop.Web.ViewModels;
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


namespace DriveDrop.Web.Controllers
{
   // [Route("api/v1/[controller]")]
    public class DriverController : Controller
    {
        private readonly IHostingEnvironment _env; 
       

        //  private readonly IIdentityParser<ApplicationUser> _appUserParser;

        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly string _remoteServiceBaseUrl;


        public DriverController(IHostingEnvironment env ,
              IOptionsSnapshot<AppSettings> settings)
        {
            _env = env; 
           
            _settings = settings;

            _remoteServiceBaseUrl = $"{_settings.Value.DriveDropUrl}/api/v1/Drivers/";
        }

        public IActionResult Index()
        {

            return View();
        }
        public IActionResult New()
        {

            var model = new DriverModel();

            PrepareCustomerModel(model); 


            return View(model);
        }

       
        //public async Task<IActionResult> AssignDriver(int id, int shipingId)
        //{
        //    var shipping = _context.Shipments.Find(shipingId);

        //    if (shipping==null)
        //        return RedirectToAction("Result", new { id = id });

        //    var driver = _context.Customers.Find(id);
        //    if (driver == null)
        //        return RedirectToAction("Result", new { id = id });

        //    shipping.SetDriver(driver);
        //    _context.Update(driver);
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction("Result", new { id = id });

        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ChangeShippingStatus(int customerId, int shipingId, int ShippingStatusId)
        //{
        //    var shipping = _context.Shipments.Find(shipingId);

        //    if (shipping == null)
        //        return RedirectToAction("Result", new { id = customerId });

        //    var driver = _context.Customers.Find(customerId);
        //    if (driver == null)
        //        return RedirectToAction("Result", new { id = customerId });

        //    shipping.ChangeStatus(ShippingStatusId);
        //    _context.Update(shipping);
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction("Result", new { id = customerId });
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken] 
        //public async Task<IActionResult> New(DriverModel c, List<IFormFile> files)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var defaultAddres = new Address(c.DeliveryStreet, c.DeliveryCity, "WA", "USA", c.DeliveryZipCode, c.DeliveryPhone, c.DeliveryContact, 0, 0);
        //        //    var pickUpAddres = new Address(c.PickupStreet, c.PickupCity, "WA", "USA", c.PickupZipCode, c.PickupPhone, c.PickupContact, 0, 0);

        //            var tmpUser = Guid.NewGuid().ToString();

        //            var newCustomer = new Customer(tmpUser, c.FirstName, c.LastName, c.TransportTypeId, CustomerStatus.WaitingApproval.Id, c.Email, c.Phone, CustomerType.Driver.Id,c.MaxPackage??0,c.PickupRadius ?? 0, c.DeliverRadius ?? 0);

        //            _context.Add(newCustomer);
        //              _context.SaveChanges();

        //            newCustomer.AddDefaultAddress(defaultAddres);
        //            _context.Update(newCustomer); 
                     

        //            await _context.SaveChangesAsync();

        //            Guid extName = Guid.NewGuid() ;
        //            //saving files
        //            long size = files.Sum(f => f.Length);

        //            // full path to file in temp location
        //            var filePath = Path.GetTempFileName();
        //            var uploads = Path.Combine(_env.WebRootPath, "uploads\\img\\driver");
        //            var fileName = "";

        //            foreach (var formFile in files)
        //            {
                        
        //                if (formFile.Length > 0)
        //                {
        //                    var extension = ".jpg";
        //                        if(formFile.FileName.ToLower().EndsWith(".jpg"))
        //                        extension = ".jpg";
        //                    if (formFile.FileName.ToLower().EndsWith(".tif"))
        //                        extension = ".tif";
        //                    if (formFile.FileName.ToLower().EndsWith(".png"))
        //                        extension = ".png";
        //                    if (formFile.FileName.ToLower().EndsWith(".gif"))
        //                        extension = ".gif";



                           
        //                    filePath = string.Format("{0}\\{1}{2}", uploads, extName, extension);
        //                    fileName = string.Format("uploads\\img\\driver\\{0}{1}",  extName, extension);

        //                    using (var stream = new FileStream(filePath, FileMode.Create))
        //                    {
        //                        await formFile.CopyToAsync(stream);
        //                    }
        //                }
        //            }
        //            if (!string.IsNullOrWhiteSpace(fileName))
        //            {
        //                newCustomer.AddPicture(fileName);
        //                _context.Update(newCustomer);
        //                await _context.SaveChangesAsync();
        //            }

        //            return CreatedAtAction(nameof(Result), new { id = newCustomer.Id }, null);
        //        }
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        //Log the error (uncomment ex variable name and write a log.
        //        var error = string.Format("Unable to save changes. " +
        //            "Try again, and if the problem persists " +
        //            "see your system administrator. {0}", ex.Message);

        //        ModelState.AddModelError("", error);
        //    }

        //    PrepareCustomerModel(c);
        //    return View(c);
        //}

        
        //public async Task<IActionResult> Result(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var customer = await _context.Customers 
        //        .Where(x=>x.CustomerTypeId == CustomerType.Driver.Id)
        //        .Include(d => d.ShipmentDrivers).ThenInclude(ShipmentDrivers => ShipmentDrivers.PriorityType)
        //       .Include(d => d.ShipmentDrivers).ThenInclude(ShipmentDrivers => ShipmentDrivers.ShippingStatus)
        //       .Include(d=>d.ShipmentDrivers).ThenInclude(ShipmentDrivers => ShipmentDrivers.PickupAddress)
        //       .Include(d => d.ShipmentDrivers).ThenInclude(ShipmentDrivers => ShipmentDrivers.DeliveryAddress)
        //       .Include(s => s.TransportType).Include(t => t.CustomerStatus).Include(s => s.CustomerType)
        //       .SingleOrDefaultAsync(m => m.Id == id);


        //    var tttp = customer.ShipmentSenders;


        //    if (customer == null)
        //    {
        //        return NotFound();
        //    }

        //    ViewBag.DriverId = id;

        //    ViewBag.ShippingStatuses = _context.ShippingStatuses.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();




        //    return View(customer);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> SaveNewShipment(NewShipment c) 
        //{
        //    try
        //    {


        //        foreach (var state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
        //        {
        //            var tt = state.Errors.ToString();
        //        }

        //        if (ModelState.IsValid)
        //        {

        //            var sender = _context.Customers.Find(c.CustomerId);


        //            var deliveryAddres = new Address(c.DeliveryStreet, c.DeliveryCity, "WA", "USA", c.DeliveryZipCode, c.DeliveryPhone, c.DeliveryContact, 0, 0);
        //            var pickUpAddres = new Address(c.PickupStreet, c.PickupCity, "WA", "USA", c.PickupZipCode, c.PickupPhone, c.PickupContact, 0, 0);

        //            var tmpUser = Guid.NewGuid().ToString();                     



        //            var shipment = new Shipment(pickUpAddres, deliveryAddres, sender, 0, 0, c.PriorityTypeId, c.PriorityTypeLevel, c.TransportTypeId, c.Note, "", "");
        //            _context.Add(shipment);

        //            _context.SaveChanges();

        //            await _context.SaveChangesAsync();


        //            return RedirectToAction("result", new { id = sender.Id });
        //            //return CreatedAtAction(nameof(Result), new { id = sender.Id }, null);
        //        }
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        //Log the error (uncomment ex variable name and write a log.
        //        var error = string.Format("Unable to save changes. " +
        //            "Try again, and if the problem persists " +
        //            "see your system administrator. {0}", ex.Message);

        //        ModelState.AddModelError("", error);
        //    }
        //    var customer = await _context.Customers.FirstOrDefaultAsync();

        //    PrepareCustomerModel(c);

        //    return ViewComponent("NewShipment", c);

        //    //return View(c);
        //}

        public IActionResult ValidateUserName(string UserEmail)
        {
            return Json(!UserEmail.Equals("duplicate") );
        }

        private ActionResult Json(bool v, object allowGet)
        {
            throw new NotImplementedException();
        }

        public DriverModel PrepareCustomerModel(DriverModel model)
        {
            //model.CustomerTypeList = _context.CustomerTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            //model.TransportTypeList = _context.TransportTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            //model.CustomerStatusList = _context.CustomerStatuses.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            //model.PriorityTypeList = _context.PriorityTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            return model;
        }
        public CustomerModel PrepareCustomerModel(CustomerModel model)
        {
            //model.CustomerTypeList = _context.CustomerTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            //model.TransportTypeList = _context.TransportTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            //model.CustomerStatusList = _context.CustomerStatuses.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            //model.PriorityTypeList = _context.PriorityTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            return model;
        }

    }
}
