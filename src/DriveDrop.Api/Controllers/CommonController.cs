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
using Microsoft.AspNetCore.Authorization;

namespace DriveDrop.Api.Controllers
{
    //[Authorize]
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
        // POST api/Attachments
       // [Authorize]
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> PostFiles([FromBody]ICollection<IFormFile> files)
        {

            string belongTo = "sender";
            try
            {
                var fileUri = string.Format("uploads\\img\\{0}", belongTo);

                Guid extName = Guid.NewGuid();
                //saving files
                long size = files.Sum(f => f.Length);

                // full path to file in temp location
                var filePath = Path.GetTempFileName();
                var uploads = Path.Combine(_env.WebRootPath, fileUri);
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
                        fileName = string.Format("{0}\\{1}{2}", fileUri, extName, extension);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                    }
                }
                //if (!string.IsNullOrWhiteSpace(fileName))
                //{
                //    //shipment.SetPickupPictureUri(fileName);
                //    //_context.SaveChanges();
                //    //await _context.SaveChangesAsync();
                //}

                return Ok(new { fileName = fileName });
            }
            catch (DbUpdateException ex)
            {
                //Log the error (uncomment ex variable name and write a log.
                var error = string.Format("Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator. {0}", ex.Message);

                return NotFound("UnableToSaveChanges");
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



     