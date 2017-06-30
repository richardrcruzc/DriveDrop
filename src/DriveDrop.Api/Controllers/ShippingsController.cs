using ApplicationCore.Entities.ClientAgregate;
using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;
using DriveDrop.Api.Infrastructure;
using DriveDrop.Api.Infrastructure.Services;
using DriveDrop.Api.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Api.Controllers
{

   [Authorize]
    [Route("api/v1/[controller]")]
    public class ShippingsController : Controller
    {
        private readonly DriveDropContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IIdentityService _identityService;
        public ShippingsController(IHostingEnvironment env, DriveDropContext context, IIdentityService identityService)
        {
            _context = context;
            _env = env;
            _identityService = identityService;
        }
        [HttpGet]
        [Route("[action]/{id:int}")]
        public async Task<IActionResult> UpdatePackageStatus(int id)
        {
            try
            {
                var shipping = await _context.Shipments.FindAsync(id);
                if(shipping!=null)
                {
                    if (shipping.ShippingStatusId == 1)
                        shipping.ChangeStatus(2);
                    else if (shipping.ShippingStatusId == 2)
                        shipping.ChangeStatus(3);
                    else if (shipping.ShippingStatusId == 3)
                        shipping.ChangeStatus(4);
                    else if (shipping.ShippingStatusId == 4)
                        shipping.ChangeStatus(5);

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
               .ToListAsync();

                return Ok(model);


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

                if (ModelState.IsValid)
                {

                    var sender = _context.Customers.Find(c.CustomerId);


                    var deliveryAddres = new Address(c.DeliveryStreet, c.DeliveryCity, "WA", "USA", c.DeliveryZipCode, c.DeliveryPhone, c.DeliveryContact, 0, 0);
                    var pickUpAddres = new Address(c.PickupStreet, c.PickupCity, "WA", "USA", c.PickupZipCode, c.PickupPhone, c.PickupContact, 0, 0);

                    var tmpUser = Guid.NewGuid().ToString();



                    var shipment = new Shipment(pickUpAddres, deliveryAddres, sender, 0, 0, c.PriorityTypeId, c.PriorityTypeLevel, c.TransportTypeId, c.Note, c.PickupPictureUri, "");
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
