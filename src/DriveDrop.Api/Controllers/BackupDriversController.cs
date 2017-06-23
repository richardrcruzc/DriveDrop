//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using ApplicationCore.Interfaces;
//using DriveDrop.Api.Infrastructure;
//using DriveDrop.Api.ViewModels;
//using ApplicationCore.Entities.ClientAgregate;
//using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Mvc.Rendering;

//namespace DriveDrop.Api.Controllers
//{

//    [Route("api/[controller]")]
//    public class DriversControler : Controller
//    {
//        private readonly DriveDropContext _context;
//        public DriversControler( DriveDropContext context)
//        {

//            _context = context;
//        }
//        // GET api/values
//        [HttpGet]
//        public IEnumerable<string> Get()
//        {
//            var tt = _context.CustomerTypes;
//            return new string[] { tt.FirstOrDefault().Name, "value2" };
//        }
//        // GET api/values/5
//        [HttpGet("{id}")]
//        public string Get(int id)
//        {
//            var tt = _context.CustomerTypes.Find(id);
//            return tt.Name;
//        }

//        // POST api/values
//        [HttpPost]
//        public void Post([FromBody]string value)
//        {
//        }

//        // PUT api/values/5
//        [HttpPut("{id}")]
//        public void Put(int id, [FromBody]string value)
//        {
//        }

//        // DELETE api/values/5
//        [HttpDelete("{id}")]
//        public void Delete(int id)
//        {
//        }
//    }
//}
////        private readonly IImageService _imageService;
////        private readonly IAppLogger<DriversControler> _logger;

////        private readonly DriveDropContext _context;

////        //  private readonly IIdentityParser<ApplicationUser> _appUserParser;


////        public DriversControler( IImageService imageService,
////            IAppLogger<DriversControler> logger, DriveDropContext context)
////        { 
////            _imageService = imageService;
////            _logger = logger;
////            _context = context;
////        }

////        [HttpGet]
////        public IActionResult List()
////        {
////            return Ok("0");
////            //return Ok(await _context.Customers.Where(c=>c.CustomerTypeId == CustomerType.Driver.Id).ToListAsync());
////        }

////        //public async Task<IActionResult> List()
////        //{
////        //    return Ok("0");
////        //    //return Ok(await _context.Customers.Where(c=>c.CustomerTypeId == CustomerType.Driver.Id).ToListAsync());
////        //}


////        //public IActionResult New()
////        //{

////        //    var model = new DriverModel();

////        //    PrepareCustomerModel(model);


////        //    return View(model);
////        //}
////        [Route("new")]
////        [HttpPost]
////        public async Task<IActionResult> New([FromBody]DriverModel c)
////        {
////            try
////            {
////                if (ModelState.IsValid)
////                {
////                    var defaultAddres = new Address(c.DeliveryStreet, c.DeliveryCity, "WA", "USA", c.DeliveryZipCode, c.DeliveryPhone, c.DeliveryContact, 0, 0);
////                    //    var pickUpAddres = new Address(c.PickupStreet, c.PickupCity, "WA", "USA", c.PickupZipCode, c.PickupPhone, c.PickupContact, 0, 0);

////                    var tmpUser = Guid.NewGuid().ToString();

////                    var newCustomer = new Customer(tmpUser, c.FirstName, c.LastName, c.TransportTypeId, CustomerStatus.WaitingApproval.Id, c.Email, c.Phone, CustomerType.Driver.Id, c.MaxPackage ?? 0, c.PickupRadius ?? 0, c.DeliverRadius ?? 0);

////                    _context.Add(newCustomer);
////                    _context.SaveChanges();

////                    newCustomer.AddDefaultAddress(defaultAddres);
////                    _context.Update(newCustomer);

////                    //var shipment = new Shipment(pickUpAddres, deliveryAddres, newCustomer, 0, 0, c.PriorityTypeId, c.PriorityTypeLevel, c.TransportTypeId ?? 0, c.Note, "", "");
////                    //_context.Add(shipment);

////                    // _context.SaveChanges();  

////                    await _context.SaveChangesAsync();

////                    return Ok(newCustomer.Id);

////                }
////            }
////            catch (DbUpdateException ex)
////            {
////                //Log the error (uncomment ex variable name and write a log.
////                var error = string.Format("Unable to save changes. " +
////                    "Try again, and if the problem persists " +
////                    "see your system administrator. {0}", ex.Message);

////                ModelState.AddModelError("", error);

////                return BadRequest(ErrorCode.CouldNotCreateItem.ToString());

////            }

////            PrepareCustomerModel(c);
////            return View(c);
////        }
////        DriverModel PrepareCustomerModel(DriverModel model)
////        {
////            model.CustomerTypeList = _context.CustomerTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
////            model.TransportTypeList = _context.TransportTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
////            model.CustomerStatusList = _context.CustomerStatuses.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

////            model.PriorityTypeList = _context.PriorityTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

////            return model;
////        }
////        CustomerModel PrepareCustomerModel(CustomerModel model)
////        {
////            model.CustomerTypeList = _context.CustomerTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
////            model.TransportTypeList = _context.TransportTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
////            model.CustomerStatusList = _context.CustomerStatuses.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

////            model.PriorityTypeList = _context.PriorityTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

////            return model;
////        }

////        public enum ErrorCode
////        {
////            TodoItemNameAndNotesRequired,
////            TodoItemIDInUse,
////            RecordNotFound,
////            CouldNotCreateItem,
////            CouldNotUpdateItem,
////            CouldNotDeleteItem
////        }
////    }
////}



////[Route("api/[controller]")]
////public class DriversControllers : Controller
////{
////    // GET api/values
////    [HttpGet]
////    public IEnumerable<string> Get()
////    {
////        return new string[] { "value1", "value2" };
////    }

////    // GET api/values/5
////    [HttpGet("{id}")]
////    public string Get(int id)
////    {
////        return "value";
////    }

////    // POST api/values
////    [HttpPost]
////    public void Post([FromBody]string value)
////    {
////    }

////    // PUT api/values/5
////    [HttpPut("{id}")]
////    public void Put(int id, [FromBody]string value)
////    {
////    }

////    // DELETE api/values/5
////    [HttpDelete("{id}")]
////    public void Delete(int id)
////    {
////    }
////}