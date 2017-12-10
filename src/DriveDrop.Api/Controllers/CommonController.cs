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
using DriveDrop.Api.Services;
using Microsoft.Extensions.Options;

namespace DriveDrop.Api.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    public class CommonController : Controller
    {
        private readonly IOptionsSnapshot<AppSettings> _settings;

        private readonly IEmailSender _emailSender;
        private readonly ICustomerService _cService;
        private readonly DriveDropContext _context;
        private readonly IHostingEnvironment _env;
        public CommonController(IOptionsSnapshot<AppSettings> settings, IEmailSender emailSender, ICustomerService cService, IHostingEnvironment env, DriveDropContext context)
        {
            _settings = settings;
            _emailSender = emailSender;
            _context = context;
            _env = env;
            _cService = cService;
        }
        [HttpGet]
        [Route("[action]/id/{id:int}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.Where(x=>x.Id==id&&x.Isdeleted==false).FirstOrDefaultAsync();
            if (customer == null)
                return NotFound();
            customer.Delete();
            _context.Update(customer);
            await _context.SaveChangesAsync();
            return Ok("CustomerDeleted");
        }
        [Route("[action]")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SendEmail([FromBody]SendEmailModel m)
        {
            await _emailSender.SendEmailAsync(m.UserName, m.Subject, m.Message);
            return Ok("Sent");
        }
        [HttpGet]
        [Route("[action]/userName/{userName}")]
        public async Task<IActionResult> WelcomeEmail(string userName)
        {
            try
            {
                // var customer = await _cService.Get(userName);
                var customer = await _context.Customers
                    .Include(c=>c.CustomerStatus)
                    .Where(x => x.UserName == userName).FirstOrDefaultAsync();
                var type = customer.CustomerTypeId;
                if(type ==2 )
                    await _emailSender.SendEmailAsync(customer.UserName, "DriveDrop account created",
                       $"{customer.FullName}: your account have been create and your status is {customer.CustomerStatus.Name},<br /> <br />  " +
                       $"Your login infomation:<br /> Email: {customer.UserName}<br /><br />  " +
                       $" you can access your account by clicking here: <a href='{_settings.Value.MvcClient}'>link</a>");

                else
                    await _emailSender.SendEmailAsync(customer.UserName, "DriveDrop account created",
                   $"{customer.FullName}: your account have been create and your status is {customer.CustomerStatus.Name}, " +
                   $"we'll review you application ASAP, you can access your account by clicking here: <a href='{_settings.Value.MvcClient}'>link</a>");



                // var customer = await _context.Customers
                //    .Include(s => s.TransportType)
                //    .Include(t => t.CustomerStatus)
                //    .Include(s => s.CustomerType)
                //    .Include(a => a.Addresses)
                //.Where(u => u.UserName == userName).FirstOrDefaultAsync();

                if (customer != null)
                {
                    return Ok("Sent");
                }


            }
            //catch (Exception exe)
            catch
            {
                //return BadRequest("CustomerNotFound" + exe.Message);
            }

            return Ok(null);
        }
        [HttpGet]
        [Route("[action]/user/{userName}")]
        public async Task<IActionResult> GetUser(string userName)
        {
            try
            {
                var customer =await  _cService.Get(userName);
                
               // var customer = await _context.Customers
               //    .Include(s => s.TransportType)
               //    .Include(t => t.CustomerStatus)
               //    .Include(s => s.CustomerType)
               //    .Include(a => a.Addresses)
               //.Where(u => u.UserName == userName).FirstOrDefaultAsync();

                if (customer != null)
                {
                    return Ok(customer);
                }


            }
            //catch (Exception exe)
            catch
            {
                //return BadRequest("CustomerNotFound" + exe.Message);
            }

            return Ok(null);

        }
        [HttpGet]
        [Route("[action]/user/{userName}/id/{id:int}")]
        public async Task<IActionResult> IdIsUser(  string userName,  int id)
        {
            try
            {
                // var customer = await _context.Customers.Where(u => u.UserName == userName && u.Id==id).FirstOrDefaultAsync();

                var customer = await _cService.Get(userName,id);


                if (customer != null)
                {                   
                        return Ok(true);
                }
                    

            }
            //catch (Exception exe)
            catch
            {
                //return BadRequest("CustomerNotFound" + exe.Message);
            }

            return Ok(false);

        }
        [HttpGet]
        [Route("[action]/{userName}")]
        public async Task<IActionResult> IsAdmin(string userName)
        {
            try
            {
                // var customer = await _context.Customers.Where(u => u.UserName == userName).FirstOrDefaultAsync();
                var customer = await _cService.Get(userName);
                if (customer != null)
                    return Ok(customer.IsAdmin);

                //if (customer != null)
                //{
                //    if (customer.CustomerTypeId==1)
                //        return Ok(true);

                //}


            }
            //catch (Exception exe)
            catch
            {
                //return BadRequest("CustomerNotFound" + exe.Message);
            }

            return Ok(false);

        }


        [AllowAnonymous]
        [HttpGet]
        [Route("[action]/{userName}")]
        public async Task<IActionResult> ValidateUserName(string userName)
        {
            try
            { 
                var customer = await _context.Customers.Where(u => u.UserName == userName).FirstOrDefaultAsync();
                    

                if (customer== null  )
                    return Ok("valid");

                if(customer.Isdeleted)
                    return Ok("duplicate");

            }
            //catch (Exception exe)
            catch
            {
                //return BadRequest("CustomerNotFound" + exe.Message);
            }

            return Ok("duplicate");

        }

        // GET api/values/5
        [HttpGet]
        [Route("[action]/{id:int}")]
        public async Task<IActionResult> GetbyId(int id)
        {
            try
            {
                var customer = await _cService.Get(id);

                // var customer = await _context.Customers.FindAsync(id);

                //var customer = await _context.Customers
                //    .Include(s => s.TransportType)
                //    .Include(t => t.CustomerStatus)
                //    .Include(s => s.CustomerType)
                //    .Include(a => a.Addresses)
                //    .Include(a => a.DefaultAddress)
                //.FirstOrDefaultAsync(x => x.Id == id);

                if (customer == null|| customer.IsDeleted)
                 return StatusCode(StatusCodes.Status409Conflict, "CustomerNotFound");

                return Ok(customer);


            }
            catch (Exception exe)
            {
                return BadRequest("CustomerNotFound" + exe.Message);
            }

        }
        [Route("[action]/customerId/{customerId:int}/addressid/{addressId:int}")]
        [HttpPost]
        public async Task<IActionResult> DeleteAddress(int customerId, int addressId)
        {
            var updateCustomer = _context.Customers
                .Include(a => a.Addresses)
                .Where(x => x.Id == customerId).FirstOrDefault();
            if (updateCustomer != null)
            {
                foreach (var a in updateCustomer.Addresses)
                    if (a.Id == addressId)
                    {
                        updateCustomer.DeleteAddress(a);
                        _context.Update(updateCustomer);
                        await _context.SaveChangesAsync();

                        break;
                    }

            }


            return CreatedAtAction(nameof(GetbyId), new { id = updateCustomer.Id }, null);

        }


        [Route("[action]")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddAddress([FromBody]AddressModel a) 
        {
            try
            { 

                var updateCustomer = _context.Customers.Where(x => x.Id == a.CustomerId).FirstOrDefault();
                if (updateCustomer != null)
                {
                    var addres = new Address(a.Street, a.City, a.State, a.Country, a.ZipCode, a.Phone, a.Contact, a.Latitude, a.Longitude, a.TypeAddress);  

                    updateCustomer.AddAddress(addres);

                    _context.Update(updateCustomer);
                    await _context.SaveChangesAsync();
                }

                return CreatedAtAction(nameof(GetbyId), new { id = updateCustomer.Id }, null);
                
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

        [Route("[action]/customerId/{id:int}/addressid/{addressId:int}")]
        [HttpGet]
        public async Task<IActionResult> DefaultAddress(int id , int addressId)
        {
            try
            {
                var updateCustomer = await _context
                    .Customers 
                    .Include(s => s.Addresses)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (updateCustomer == null)
                    return NotFound("UnableToSaveChanges");

                var currentDefaultAddress = updateCustomer.DefaultAddress;

                var newDefaultAddress = updateCustomer.Addresses.Where(x => x.Id == addressId).FirstOrDefault();

                if(newDefaultAddress == null)
                    return NotFound("UnableToSaveChanges");
                
                updateCustomer.AddDefaultAddress(newDefaultAddress);
                _context.Customers.Update(updateCustomer);
         

                currentDefaultAddress.UpdateType("pickup");
                _context.Addresses.Update(currentDefaultAddress);

                newDefaultAddress.UpdateType("home");
                _context.Addresses.Update(newDefaultAddress);


                 _context.Customers.Update(updateCustomer);

                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetbyId), new { id = updateCustomer.Id }, null);


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




        // POST api/Attachments
        // [Authorize]
        [AllowAnonymous]
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
        [AllowAnonymous]
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
        [AllowAnonymous]
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
        [AllowAnonymous]
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
        [AllowAnonymous]
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
        [AllowAnonymous]
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


        // GET api/v1/Common/CustomerTypes
        [AllowAnonymous]
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> PackageSizes()
        {
            try
            {

              //  var rate = await _context.Rates
              //.Include(x => x.PackageSize)
              //.OrderBy(x => x.Id)
              //.Select(x => new { Id = x.PackageSize.Id.ToString(), Name = x.PackageSize.Name, OverHead = x.OverHead })
              //.ToListAsync();


                var types = await (from p in _context.PackageSizes
                                  join r in _context.Rates on p.Id equals r.PackageSize.Id
                                  orderby p.Name
                                  select new { Id = p.Id.ToString(), Name = p.Name + " - $"+r.OverHead.ToString() }).ToListAsync();

                    //.OrderBy(x => x.Name).Select(x => new { Id = x.Id.ToString(), Name = x.Name }).ToListAsync();

                return Ok(types);
            }
            catch (Exception)
            {
                return BadRequest("PackageSizesNotFound");
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

         
     
    }
}



     