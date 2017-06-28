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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DriveDrop.Api.Controllers
{
    [Route("api/v1/[controller]")]
    public class CommonController : Controller
    {

        private readonly DriveDropContext _context;
        private readonly IHostingEnvironment _env;
        public CommonController(IHostingEnvironment env, DriveDropContext context)
        {
            _context = context;
            _env = env;
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

        // GET api/v1/Common/CustomerTypes
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetNotAssignedShipping()
        {
            try
            {
                var model = await _context.Shipments
               .Where(x => x.ShippingStatusId == ShippingStatus.PendingPickUp.Id && x.DriverId == null)
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


        // GET api/v1/Common/CustomerTypes
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> CustomerTypes()
        {
            try
            {
                var types = await _context.CustomerTypes.Select(x => new   { Id = x.Id.ToString(), Name = x.Name }).ToListAsync();

                return Ok(types);
            }
            catch (Exception)
            {
                return BadRequest("CustomerTypesNotFound");
            }
        }
        // GET api/v1/Common/CustomerTypes
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ShippingStatuses()
        {
            try
            {
                var types = await _context.ShippingStatuses.Select(x => new   { Id = x.Id.ToString(), Name = x.Name }).ToListAsync();

                return Ok(types);
            }
            catch (Exception)
            {
                return BadRequest("ShippingStatusesNotFound");
            }
        }
        // GET api/v1/Common/CustomerTypes
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> PriorityTypes()
        {
            try
            {
                //  var types = await _context.PriorityTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToListAsync();
                var types = await _context.PriorityTypes.Select(x => new  {Id = x.Id.ToString(), Name = x.Name }).ToListAsync();

                return Ok(types);
            }
            catch (Exception)
            {
                return BadRequest("PriorityTypesNotFound");
            }
        }
        // GET api/v1/Common/CustomerTypes
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> CustomerStatuses()
        {
            try
            {
                var types = await _context.CustomerStatuses.Select(x =>   new { Id = x.Id.ToString(), Name = x.Name }).ToListAsync();

                return Ok(types);
            }
            catch (Exception)
            {
                return BadRequest("CustomerStatusesNotFound");
            }
        }
        // GET api/v1/Common/CustomerTypes
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> TransportTypes()
        {
            try
            {
                var types = await _context.TransportTypes.Select(x => new { Id = x.Id.ToString(), Name = x.Name }).ToListAsync();

                return Ok(types);
            }
            catch (Exception)
            {
                return BadRequest("TransportTypesNotFound");
            }
        }

        
        //[Route("[action]/{uri}")]
        //// GET: /<controller>/
        //public IActionResult GetImage(string uri)
        //{
        //    var webRoot = _env.WebRootPath;
        //    var path = Path.Combine(webRoot, uri);

        //    var buffer = System.IO.File.ReadAllBytes(path);

        //    return File(buffer, "image/png");
        //}

        [HttpGet]
        [Route("[action]/{useremail}")]
        public JsonResult ValidateUserName(string UserEmail)
        {
            return Json(!UserEmail.Equals("duplicate"));
        }
     
    }
}



     