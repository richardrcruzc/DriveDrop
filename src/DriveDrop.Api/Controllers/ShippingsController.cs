using ApplicationCore.Entities.ClientAgregate;
using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;
using DriveDrop.Api.Infrastructure;
using DriveDrop.Api.Infrastructure.Services;
using DriveDrop.Api.Services;
using DriveDrop.Api.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DriveDrop.Api.Controllers
{

   //[Authorize]
    [Route("api/v1/[controller]")]
    public class ShippingsController : Controller
    {
        
        private readonly IDistanceService _distanceService;
        private readonly DriveDropContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IIdentityService _identityService;
        private readonly IRateService _rateService;
        private readonly IPayPalStandardPaymentProcessor _pp;
        private readonly IOptionsSnapshot<AppSettings> _settings;

        public ShippingsController(IHostingEnvironment env, DriveDropContext context, IIdentityService identityService, 
            IRateService rateService, IPayPalStandardPaymentProcessor pp, IOptionsSnapshot<AppSettings> settings, IDistanceService distanceService)
        {
            _settings = settings;
            _context = context;
            _env = env;
            _identityService = identityService;
            _rateService = rateService;
            _pp = pp;
            _distanceService = distanceService;
        }

        
        [HttpGet]
        [Route("[action]/userEmail{userEmail}/packageId{packageId}")]
        public async Task<IActionResult> ProcessPayment(string userEmail, int packageId, decimal amount)
        {
            try
            {
                var customer = await _context.Customers
                    .Include(s=>s.ShipmentSenders)
                    .Where(x => x.UserName == userEmail).FirstOrDefaultAsync();
                if (customer != null)
                    return BadRequest("customerNotFound");

                var shipping =  customer.ShipmentSenders.Where(x => x.Id == packageId).FirstOrDefault();
                if (shipping != null)
                    return BadRequest("ShipmentNotFound");

                shipping.ApplyPayment(amount);
                _context.Update(shipping);
                await _context.SaveChangesAsync();

                return Ok("updated");


            }
            catch (Exception)
            {
                return BadRequest("CustomerTypesNotFound");
            }


            //try
            //{
            //     await _pp.PostProcessPayment(shipmentId: packageId);
            //    return Ok("updated");


            //}
            //catch (Exception)
            //{
            //    return BadRequest("CustomerTypesNotFound");
            //}
        }
        /// <summary>
        /// Gets Paypal URL
        /// </summary>
        /// <returns></returns>
        private string GetPaypalUrl()
        {
            return _settings.Value.UseSandbox ? "https://www.sandbox.paypal.com/us/cgi-bin/webscr" :
                "https://www.paypal.com/us/cgi-bin/webscr";
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Return(string info, int customerId)
        {
            ViewBag.CustomerId = customerId;
            ViewData["Message"] = "Your contact page.";
             
            return Ok("updated");
        }
        [HttpGet]
        [Route("[action]")]
        public  IActionResult  NotifyFromPaypalAsync()
        {
            string firstName = HttpContext.Request.Form["firstname"];

            return Ok("updated");
        }
         [HttpGet]
        [Route("[action]/shippingid/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                
                var root =await  _context.Shipments
              .Where(x => x.Id == id)
              .Include(d => d.DeliveryAddress)
              .Include(d => d.PickupAddress)
              .Include(d => d.ShippingStatus)
              .Include(d => d.PriorityType)
                .Include(d => d.Reviews)
                .Include(d => d.Sender)
                .Include(d => d.Driver)
                .Include(d => d.PackageSize)
                .Include(d => d.PackageStatusHistories)
                .ThenInclude(d=>d.ShippingStatus)
                

                .FirstOrDefaultAsync();

                return Ok(root);
                
            }
            catch (Exception)
            {
                return BadRequest("CustomerTypesNotFound");
            }
        }
     

        [HttpGet]
        [Route("[action]/{id:int}")]
        public async Task<IActionResult> UpdateDriver(int id, int driverId)
        {
            try
            {
                var shipping = await _context.Shipments.FindAsync(id);
                if (shipping != null)
                {
                    var driver = _context.Customers.Where(c => c.Id == driverId).FirstOrDefault();
                    if (driver != null)
                    { 
                        shipping.SetDriver(driver);
                        _context.Update(shipping);
                        await _context.SaveChangesAsync();
                    }
                }
                return Ok("updated");


            }
            catch (Exception)
            {
                return BadRequest("CustomerTypesNotFound");
            }
        }
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SetDeliveredPictureUri([FromBody]AcceptModel model)
        {
            try
            {
                var shipping = _context.Shipments.Find(model.Id);

                if (shipping == null)
                    return NotFound("PackageNoFound");
                if (model.StatusId > 0)
                    shipping.ChangeStatus(model.StatusId);
                else
                {
                    shipping.ChangeStatus(ShippingStatus.DeliveryInProcess.Id);

                }
                shipping.SetDeliveredPictureUri(System.Net.WebUtility.UrlDecode(model.fileName));
                _context.Update(shipping);
                await _context.SaveChangesAsync();

                return Ok("SetDeliveredPictureUri");
            }

            catch (Exception)
            {
                return BadRequest("cannotChangeShippingDeveleryUri");
            }

        }


        [HttpGet]
        [Route("[action]/shippingId/{shippingId:int}/shippingStatusId/{shippingStatusId:int}")]
        public async Task<IActionResult> UpdatePackageStatus(int shippingId, int shippingStatusId)
        {
            try
            {
                var shipping = await _context.Shipments.Where(x=>x.Id == shippingId).FirstOrDefaultAsync();
                if (shipping != null)
                {
                    shipping.ChangeStatus(shippingStatusId);


                    //    if (shipping.ShippingStatusId == 1)
                    //        shipping.ChangeStatus(2);
                    //    else if (shipping.ShippingStatusId == 2)
                    //        shipping.ChangeStatus(3);
                    //    else if (shipping.ShippingStatusId == 3)
                    //        shipping.ChangeStatus(4);
                    //    else if (shipping.ShippingStatusId == 4)
                    //        shipping.ChangeStatus(5);

                    _context.Update(shipping);
                    await _context.SaveChangesAsync();

                }
                return Ok("updated");


            }
            catch (Exception)
            {
                return BadRequest("CustomerTypesNotFound");
            }
        }
        
        [HttpGet]
        [Route("[action]/{id:int}")]
        public async Task<IActionResult> GetShippingByDriverId(int id, [FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0)
        {
            try
            {
               // var model = await _context.Shipments
               //.Where(x => x.DriverId == id)
               //.Include(d => d.DeliveryAddress)
               //.Include(d => d.PickupAddress)
               //.Include(d => d.ShippingStatus)
               //.Include(d => d.PriorityType)
               //.ToListAsync();

               // return Ok(model);


                var root = _context.Shipments
              .Where(x => x.DriverId == id)
              .Include(d => d.DeliveryAddress)
              .Include(d => d.PickupAddress)
              .Include(d => d.ShippingStatus)
              .Include(d => d.PriorityType);

                var totalItems = await root
                 .LongCountAsync();

                var itemsOnPage = await root
               .Skip(pageSize * pageIndex)
               .Take(pageSize)
               .ToListAsync();

                itemsOnPage = ChangeUriPlaceholder(itemsOnPage);

                var model = new PaginatedItemsViewModel<Shipment>(
                    pageIndex, pageSize, totalItems, itemsOnPage);



                return Ok(model);




            }
            catch (Exception)
            {
                return BadRequest("CustomerTypesNotFound");
            }
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetByDriverIdAndStatusId([FromQuery]int id, [FromQuery]int[] statusId, [FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0)
        {
            try
            {
                // var model = await _context.Shipments
                //.Where(x => x.DriverId == id)
                //.Include(d => d.DeliveryAddress)
                //.Include(d => d.PickupAddress)
                //.Include(d => d.ShippingStatus)
                //.Include(d => d.PriorityType)
                //.ToListAsync();

                // return Ok(model);


                var root = _context.Shipments
              .Where(x => x.DriverId == id && statusId.Contains( x.ShippingStatusId))
              .Include(d => d.DeliveryAddress)
              .Include(d => d.PickupAddress)
              .Include(d => d.ShippingStatus)
              .Include(d => d.PriorityType)
              .Include(s => s.Reviews);


                var totalItems = await root
                 .LongCountAsync();

                var itemsOnPage = await root
               .Skip(pageSize * pageIndex)
               .Take(pageSize)
               .ToListAsync();

                itemsOnPage = ChangeUriPlaceholder(itemsOnPage);

                var model = new PaginatedItemsViewModel<Shipment>(
                    pageIndex, pageSize, totalItems, itemsOnPage);



                return Ok(model);




            }
            catch (Exception)
            {
                return BadRequest("CustomerTypesNotFound");
            }
        }
        [HttpGet]
        [Route("[action]/{id:int}")]
        public async Task<IActionResult> GetShippingByCustomerId(int id)
        {
            try
            {
                var model = await _context.Shipments
               .Where(x => x.SenderId == id)
               .Include(d => d.DeliveryAddress)
               .Include(d => d.PickupAddress)
               .Include(d => d.ShippingStatus)
               .Include(d => d.PriorityType)
               .Include(s => s.Reviews)
               .OrderByDescending(x=>x.Id)
               .ToListAsync();

                return Ok(model);


            }
            catch (Exception)
            {
                return BadRequest("CustomerTypesNotFound");
            }
        }

        [HttpGet]
        [Route("[action]/shippingStatusId/{shippingStatusId:int}/priorityTypeId/{priorityTypeId:int}/identityCode/{identityCode}")]
        public async Task<IActionResult> GetShipping(int shippingStatusId, int priorityTypeId, string identityCode, [FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0)
        {
            try
            {

                var root = _context.Shipments
              .Where(x => x.Id > 0);
                 
                if(shippingStatusId > 0)
                    root = root.Where(x => x.ShippingStatusId == shippingStatusId);

                if (priorityTypeId > 0)
                    root = root.Where(x => x.PriorityTypeId == priorityTypeId);

                if( identityCode!="null")
                    root = root.Where(x => x.IdentityCode == identityCode);

                var totalItems = await root
                 .LongCountAsync();

                var itemsOnPage = await root
                    .Include(d => d.DeliveryAddress)
               .Include(d => d.PickupAddress)
               .Include(d => d.ShippingStatus)
               .Include(d => d.PriorityType)
               .Include(s=>s.Sender)
               .Include(s => s.Driver)
                 .Include(s => s.Reviews)
               .OrderBy(x=>x.ShippingCreateDate).ThenBy(x=>x.PriorityTypeId)
               .Skip(pageSize * pageIndex)
               .Take(pageSize)               
               .ToListAsync();

                // itemsOnPage = ChangeUriPlaceholder(itemsOnPage);

                var tt1 = new SelectListItem {Text ="All Priorities",Value="0" };
                var tt2 = new SelectListItem { Text = "All Status", Value = "0" };

                var shippingStatu = new List<SelectListItem>();
                var priorityType = new List<SelectListItem>();

                shippingStatu.Add(tt2);
                priorityType.Add(tt1);

                shippingStatu.AddRange( await _context.ShippingStatuses.OrderBy(x => x.Name).Select(x=>new  SelectListItem { Text = x.Name, Value=x.Id.ToString() }).ToListAsync());
                 priorityType.AddRange(await _context.PriorityTypes.OrderBy(x => x.Name).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToListAsync()) ;

                var model = new PaginatedItemsViewModel<Shipment>(
                    pageIndex, pageSize, totalItems, itemsOnPage, shippingStatu, priorityType, null);
 

                var vm = new ShippingIndex()
                {
                     ShippingList =  model.Data,
                     PriorityTypeFilterApplied= priorityTypeId,
                     ShippingStatusFilterApplied =priorityTypeId,
                      PriorityType= priorityType,
                       ShippingStatus = shippingStatu,

                    PaginationInfo = new PaginationInfo()
                    {
                        ActualPage = pageIndex,
                        ItemsPerPage = model.Data.Count(),
                        TotalItems = (int)model.Count,
                        TotalPages = int.Parse(Math.Ceiling(((decimal)model.Count / pageSize)).ToString())
                    }

                };
                vm.PaginationInfo.Next = (vm.PaginationInfo.ActualPage == vm.PaginationInfo.TotalPages - 1) ? "is-disabled" : "";
                vm.PaginationInfo.Previous = (vm.PaginationInfo.ActualPage == 0) ? "is-disabled" : "";


                return Ok(vm);
                 





            }
            catch (Exception)
            {
                return BadRequest("CustomerTypesNotFound");
            }
        }



        [HttpGet]
        [Route("[action]/driverId/{driverId:int}")]
        public async Task<IActionResult> GetPackagesReadyForDriver(int driverId, [FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0)
        {
            try
            {
                var driver = _context
                    .Customers
                    .Include(x=>x.DefaultAddress)
                    .Where(x => x.Id == driverId && x.CustomerType.Id==3).FirstOrDefault();
                if (driver == null)
                    return NotFound(null);


                var driverActualLat = driver.DefaultAddress.Latitude;
                var driverActualLng = driver.DefaultAddress.Longitude;

                var driverPickupDistance = driver.PickupRadius ?? 0;
                var driverDelivertDistance = driver.DeliverRadius ?? 0;


                var ready = new List<Shipment>();



                var root = await _context.Shipments
               .Where(x => x.ShippingStatusId == ShippingStatus.NoDriverAssigned.Id 
                && x.DriverId == null && x.Distance<= driverDelivertDistance)
               .Include(d => d.DeliveryAddress)
               .Include(d => d.PickupAddress)
               .Include(d => d.ShippingStatus)
               .Include(d => d.PriorityType)
               .ToListAsync(); 

                //root = root.Where(d=>d.Distance<=driver.PickupRadius)
                foreach (var dir in root)
                {

                    var pickUpLat = dir.PickupAddress.Latitude;
                    var pickUpLng = dir.PickupAddress.Longitude;

                    var deliveryLat = dir.DeliveryAddress.Latitude;
                    var deliverypLng = dir.DeliveryAddress.Longitude;
                    

                    var pickupDistance  = DistanceAlgorithm.DistanceBetweenPlaces(driverActualLng, driverActualLat, pickUpLng, pickUpLat);

                    var deliveryDistance = driverDelivertDistance;
                    if (deliverypLng!=0 && deliveryLat!=0)
                     deliveryDistance = DistanceAlgorithm.DistanceBetweenPlaces(pickUpLng, pickUpLat, deliverypLng, deliveryLat);

                    if (pickupDistance > driverPickupDistance || deliveryDistance > driverDelivertDistance)
                        continue; 

                    

                    ready.Add(dir); 

                }
                 



                var totalItems =   ready
                 .LongCount();

                var itemsOnPage =   ready
               .Skip(pageSize * pageIndex)
               .Take(pageSize)
               .ToList();

                itemsOnPage = ChangeUriPlaceholder(itemsOnPage);

                var model = new PaginatedItemsViewModel<Shipment>(
                    pageIndex, pageSize, totalItems, itemsOnPage);



                return Ok(model);


                //var model = root.Select(x=> new ShipmentViewModel
                //{
                //    Amount = x.Amount,
                //    DeliveredPictureUri = x.DeliveredPictureUri,
                //    DeliveryAddress = x.DeliveryAddress,
                //    Discount = x.Discount,
                //    Driver = x.Driver,
                //    DriverId = x.DriverId,
                //    Id = x.Id,
                //    IdentityCode = x.IdentityCode,
                //    Note = x.Note,
                //    PickupAddress = x.PickupAddress,
                //    PickupPictureUri = x.PickupPictureUri,
                //    PriorityType = x.PriorityType,
                //    PriorityTypeId = x.PriorityTypeId,
                //    PriorityTypeLevel = x.PriorityTypeLevel,
                //    PromoCode = x.PromoCode,
                //    Sender = x.Sender,
                //    SenderId = x.SenderId,
                //    ShippingCreateDate = x.ShippingCreateDate,
                //    ShippingStatus = x.ShippingStatus,
                //    ShippingStatusId = x.ShippingStatusId,
                //    ShippingUpdateDate = x.ShippingUpdateDate,
                //    Tax=x.Tax,
                //    TransportType = x.TransportType,
                //    TransportTypeId = x.TransportTypeId

                //}).ToList();





            }
            catch (Exception)
            {
                return BadRequest("CustomerTypesNotFound");
            }
        }



        // GET api/v1/Shippings/GetNotAssignedShipping
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetNotAssignedShipping([FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0)
        {
            try
            {
                var root = _context.Shipments
               .Where(x => x.ShippingStatusId == ShippingStatus.PendingPickUp.Id && x.DriverId == null)
               .Include(d => d.DeliveryAddress)
               .Include(d => d.PickupAddress)
               .Include(d => d.ShippingStatus)
               .Include(d => d.PriorityType);

                var totalItems = await root
                 .LongCountAsync();

                var itemsOnPage = await root
               .Skip(pageSize * pageIndex)
               .Take(pageSize)
               .ToListAsync();

                itemsOnPage = ChangeUriPlaceholder(itemsOnPage);

                var model = new PaginatedItemsViewModel<Shipment>(
                    pageIndex, pageSize, totalItems, itemsOnPage);



                return Ok(model);


                //var model = root.Select(x=> new ShipmentViewModel
                //{
                //    Amount = x.Amount,
                //    DeliveredPictureUri = x.DeliveredPictureUri,
                //    DeliveryAddress = x.DeliveryAddress,
                //    Discount = x.Discount,
                //    Driver = x.Driver,
                //    DriverId = x.DriverId,
                //    Id = x.Id,
                //    IdentityCode = x.IdentityCode,
                //    Note = x.Note,
                //    PickupAddress = x.PickupAddress,
                //    PickupPictureUri = x.PickupPictureUri,
                //    PriorityType = x.PriorityType,
                //    PriorityTypeId = x.PriorityTypeId,
                //    PriorityTypeLevel = x.PriorityTypeLevel,
                //    PromoCode = x.PromoCode,
                //    Sender = x.Sender,
                //    SenderId = x.SenderId,
                //    ShippingCreateDate = x.ShippingCreateDate,
                //    ShippingStatus = x.ShippingStatus,
                //    ShippingStatusId = x.ShippingStatusId,
                //    ShippingUpdateDate = x.ShippingUpdateDate,
                //    Tax=x.Tax,
                //    TransportType = x.TransportType,
                //    TransportTypeId = x.TransportTypeId

                //}).ToList();





            }
            catch (Exception)
            {
                return BadRequest("CustomerTypesNotFound");
            }
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

                //if (ModelState.IsValid)
                //{

                    var sender = _context.Customers.Find(c.CustomerId);


                    var deliveryAddres = new Address(c.DeliveryStreet, c.DeliveryCity,c.DeliveryState,c.DeliveryCountry, c.DeliveryZipCode, c.DeliveryPhone, c.DeliveryContact, c.DeliveryLatitude,c.DeliveryLongitude, "drop");
                    var pickUpAddres = new Address(c.PickupStreet, c.PickupCity,c.PickupState,c.PickupCountry, c.PickupZipCode, c.PickupPhone, c.PickupContact,c.PickupLatitude,c.PickupLongitude, "pickup");

                    var tmpUser = Guid.NewGuid().ToString();


                // var rate = await _rateService.CalculateAmount(int.Parse(c.PickupZipCode), int.Parse(c.DeliveryZipCode), c.ShippingWeight, c.Quantity, c.PriorityTypeId, c.TransportTypeId, c.PromoCode);
                var rate = await _rateService.CalculateAmount(c.Distance, c.ShippingWeight, c.PriorityTypeId, c.PromoCode,c.PackageSizeId, c.ExtraCharge, c.ExtraChargeNote, c.PickupState, c.PickupCity);

                 
                var test = rate.AmountToCharge+ c.ExtraCharge;

                rate.AmountToCharge += c.ExtraCharge;

                    var shipment = new Shipment(pickup: pickUpAddres, delivery: deliveryAddres, sender: sender, amount: c.Amount, discount: rate.Discount,
                        shippingWeight: c.ShippingWeight, priorityTypeId: c.PriorityTypeId, transportTypeId: c.TransportTypeId,note: c.Note, pickupPictureUri: c.PickupPictureUri, deliveredPictureUri: "", 
                        distance: rate.Distance, chargeAmount:rate.AmountToCharge, promoCode: c.PromoCode, tax:rate.TaxAmount, packageSizeId: c.PackageSizeId,extraCharge: c.ExtraCharge,extraChargeNote:c.ExtraChargeNote );

                    _context.Add(shipment);

                    _context.SaveChanges();

                    await _context.SaveChangesAsync();


                sender.AddAddress(deliveryAddres);
                sender.AddAddress(pickUpAddres);

                _context.Update(sender);

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
                //}
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
           // var customer = await _context.Customers.FirstOrDefaultAsync();

            //await PrepareCustomerModel(c);

            //    return ViewComponent("NewShipment", c);

           // return Ok(c);
        }
        private List<Shipment> ChangeUriPlaceholder(List<Shipment> items)
        {
            //var baseUri = _settings.ExternalCatalogBaseUrl;
            var baseUri = "";

            items.ForEach(x =>
            {
                x.SetDeliveredPictureUri(baseUri+"/"+x.DeliveredPictureUri);
            });

            return items;
        }

    }
}
