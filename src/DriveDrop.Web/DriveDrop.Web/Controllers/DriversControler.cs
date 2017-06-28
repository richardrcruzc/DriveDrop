 
using DriveDrop.Web.Infrastructure;
using DriveDrop.Web.Services;
using DriveDrop.Web.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Web.Controllers
{
    [Route("api/[controller]")]
    public class DriversControler : Controller
    {
        private readonly IHostingEnvironment _env; 
        //private readonly IImageService _imageService;
       
       
        //  private readonly IIdentityParser<ApplicationUser> _appUserParser;
         

        public DriversControler(IHostingEnvironment env 
            )
        {
            _env = env; 
            
        }

        [HttpGet]
        public IActionResult List()
        {
            return Ok("0");
            //return Ok(await _context.Customers.Where(c=>c.CustomerTypeId == CustomerType.Driver.Id).ToListAsync());
        }

        //public async Task<IActionResult> List()
        //{
        //    return Ok("0");
        //    //return Ok(await _context.Customers.Where(c=>c.CustomerTypeId == CustomerType.Driver.Id).ToListAsync());
        //}


        //public IActionResult New()
        //{

        //    var model = new DriverModel();

        //    PrepareCustomerModel(model);


        //    return View(model);
        //}
        [Route("new")]
        [HttpPost]
        public async Task<IActionResult> New([FromBody]DriverModel c)
        {
            //try
            //{
            //    if (ModelState.IsValid)
            //    {
            //        var defaultAddres = new Address(c.DeliveryStreet, c.DeliveryCity, "WA", "USA", c.DeliveryZipCode, c.DeliveryPhone, c.DeliveryContact, 0, 0);
            //        //    var pickUpAddres = new Address(c.PickupStreet, c.PickupCity, "WA", "USA", c.PickupZipCode, c.PickupPhone, c.PickupContact, 0, 0);

            //        var tmpUser = Guid.NewGuid().ToString();

            //        var newCustomer = new Customer(tmpUser, c.FirstName, c.LastName, c.TransportTypeId, CustomerStatus.WaitingApproval.Id, c.Email, c.Phone, CustomerType.Driver.Id, c.MaxPackage ?? 0, c.PickupRadius ?? 0, c.DeliverRadius ?? 0);

            //        _context.Add(newCustomer);
            //        _context.SaveChanges();

            //        newCustomer.AddDefaultAddress(defaultAddres);
            //        _context.Update(newCustomer);

            //        //var shipment = new Shipment(pickUpAddres, deliveryAddres, newCustomer, 0, 0, c.PriorityTypeId, c.PriorityTypeLevel, c.TransportTypeId ?? 0, c.Note, "", "");
            //        //_context.Add(shipment);

            //        // _context.SaveChanges();  

            //        await _context.SaveChangesAsync();

            //        return Ok(newCustomer.Id);
                    
            //    }
            //}
            //catch (DbUpdateException ex)
            //{
            //    //Log the error (uncomment ex variable name and write a log.
            //    var error = string.Format("Unable to save changes. " +
            //        "Try again, and if the problem persists " +
            //        "see your system administrator. {0}", ex.Message);

            //    ModelState.AddModelError("", error);

            //   return BadRequest(ErrorCode.CouldNotCreateItem.ToString());
               
            //}

            PrepareCustomerModel(c);
            return View(c);
        }
          DriverModel PrepareCustomerModel(DriverModel model)
        {
            //model.CustomerTypeList = _context.CustomerTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            //model.TransportTypeList = _context.TransportTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            //model.CustomerStatusList = _context.CustomerStatuses.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            //model.PriorityTypeList = _context.PriorityTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            return model;
        }
          CustomerModel PrepareCustomerModel(CustomerModel model)
        {
            //model.CustomerTypeList = _context.CustomerTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            //model.TransportTypeList = _context.TransportTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            //model.CustomerStatusList = _context.CustomerStatuses.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            //model.PriorityTypeList = _context.PriorityTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            return model;
        }

        public enum ErrorCode
        {
            TodoItemNameAndNotesRequired,
            TodoItemIDInUse,
            RecordNotFound,
            CouldNotCreateItem,
            CouldNotUpdateItem,
            CouldNotDeleteItem
        }
    }
}
