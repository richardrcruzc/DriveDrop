using ApplicationCore.Entities.ClientAgregate;
using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;
using DriveDrop.Api.Infrastructure;
using DriveDrop.Api.Infrastructure.Services;
using DriveDrop.Api.Services;
using DriveDrop.Api.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Api.Controllers
{

    [Route("api/v1/[controller]")]
   [Authorize]
    public class AdminController : Controller
    {
        private readonly ICustomerService _cService;
        private readonly DriveDropContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IIdentityService _identityService;

        private readonly IEmailSender _emailSender;

        private readonly IOptionsSnapshot<AppSettings> _settings;


        public AdminController(ICustomerService cService, IHostingEnvironment env, DriveDropContext context, 
            IIdentityService identityService, IEmailSender emailSender, IOptionsSnapshot<AppSettings> settings)
        {
            _emailSender = emailSender;
            _cService = cService;
            _context = context;
            _env = env;
            _identityService = identityService;
            _settings = settings;
        }


        [HttpGet]
        [Route("[action]/adminUser/{adminUser}")]
        public async Task<IActionResult> EndImpersonated(string adminUser)
        {
            var isImpersonated = await _cService.EndImpersonated(adminUser);
            return Ok(isImpersonated);
        }

        [HttpGet]
        [Route("[action]/adminUser/{adminUser}/userName/{userName}")]
        public async Task<IActionResult> SetImpersonate(string adminUser, string userName)
        {
            var isImpersonated  =  await _cService.SetImpersonate(adminUser, userName);
            return Ok(isImpersonated);
        }



        [HttpGet]
        [Route("[action]/CustomerId/{CustomerId:int}/statusId/{statusId:int}")]
        public async Task<IActionResult> ChangeCustomerStatus(int CustomerId, int statusId)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(CustomerId);
            if(customer==null)
                return Ok("CustomerNoFound");

            var status = await _context.CustomerStatuses.FindAsync(statusId);

            if (status == null)
                return Ok("CustomerstatusNoFound");


            customer.ChangeStatus(status);
            _context.Update(customer);
            await _context.SaveChangesAsync(); 

                await _emailSender.SendEmailAsync(customer.UserName, "Status updated",
                    $"{customer.FullName}: your status have been updated, access your account by clicking here: <a href='{_settings.Value.MvcClient}'>link</a>");
                 
                return Ok("CustomerstatusChanged");
        }
            catch (Exception exe)
            {
                return BadRequest("DriverNotFound"+exe.Message);
    }
}


        // GET api/values
        [HttpGet]
        [Route("[action]/type/{customertype}/status/{statusId}/transporType/{transporTypeId}/LastName/{LastName}")]
        public async Task<IActionResult> Items(int? customertype, int? statusId, int? transporTypeId, string LastName, [FromQuery]int pageIndex = 0, [FromQuery]int pageSize = 10)
        {
            var root = (IQueryable<Customer>)_context.Customers.OrderBy(i=>i.Id).Where(x=>x.Isdeleted==false);

            if (customertype.HasValue)
            {
                root = root.OrderBy(t=>t.CustomerTypeId).Where(ci => ci.CustomerTypeId == customertype);
            }
            if (statusId.HasValue)
            {
                root = root.OrderBy(c=>c.CustomerStatusId).Where(ci => ci.CustomerStatus.Id == statusId);
            }

            if (transporTypeId.HasValue)
            {
                root = root.OrderBy(t=>t.TransportTypeId).Where(ci => ci.TransportType.Id == transporTypeId);
            }
            if (LastName!="null")
            {
                root = root.OrderBy(n=>n.LastName).Where(l => l.LastName.Contains(LastName));
                //root = root.Where(l => l.FirstName.Contains(LastName));
            }

            var totalItems = await root
                .LongCountAsync();

            var itemsOnPage = await root 
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .Include(x => x.CustomerStatus)
                .Include(y => y.CustomerType)
                .Include(z => z.TransportType)
                .Include(x => x.DefaultAddress)
                .ToListAsync();


            var model = new PaginatedItemsViewModel<Customer>(
               pageIndex, pageSize, totalItems, itemsOnPage);

            

            var vm = new CustomerIndex()
            {
                 CustomerList = model.Data,
                 TypeFilterApplied = customertype,
                StatusFilterApplied = statusId,
                TransportFilterApplied = transporTypeId,
                LastName = LastName,
                PaginationInfo = new PaginationInfo()
                {
                    ActualPage = pageIndex,
                    ItemsPerPage = model.Data.Count(),
                    TotalItems =(int)model.Count,
                    TotalPages = int.Parse(Math.Ceiling(((decimal)model.Count / pageSize)).ToString())
                }
            };


            var dataItems = await _context.CustomerTypes.Select(x => (new SelectListItem { Value = x.Id.ToString(), Text = x.Name })).ToListAsync();
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Value = null, Text = "All Customers", Selected = true });
            foreach (var itm in dataItems)
            {
                items.Add(new SelectListItem()
                {
                    Value = itm.Value,
                    Text = itm.Text
                });
           }
            vm.CustomerType = items;


              dataItems = await _context.CustomerStatuses.Select(x => (new SelectListItem { Value = x.Id.ToString(), Text = x.Name })).ToListAsync();
              items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Value = null, Text = "All Status", Selected = true });
            foreach (var itm in dataItems)
            {
                items.Add(new SelectListItem()
                {
                    Value = itm.Value,
                    Text = itm.Text
                });
            }
            vm.CustomerStatus = items;

              dataItems = await _context.TransportTypes.OrderBy(x=>x.Name).Select(x => (new SelectListItem { Value = x.Id.ToString(), Text = x.Name })).ToListAsync();
              items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Value = null, Text = "All Transport", Selected = true });
            foreach (var itm in dataItems)
            {
                items.Add(new SelectListItem()
                {
                    Value = itm.Value,
                    Text = itm.Text
                });
            }

            vm.TransportType = items;



           //vm.CustomerType = await _context.CustomerTypes.Select(x => (new SelectListItem { Value = x.Id.ToString(), Text = x.Name })).ToListAsync();
           // vm.CustomerStatus = await _context.CustomerStatuses.Select(x => (new SelectListItem { Value = x.Id.ToString(), Text = x.Name })).ToListAsync();
           // vm.TransportType = await _context.TransportTypes.Select(x => (new SelectListItem { Value = x.Id.ToString(), Text = x.Name })).ToListAsync();


           vm.PaginationInfo.Next = (vm.PaginationInfo.ActualPage == vm.PaginationInfo.TotalPages - 1) ? "is-disabled" : "";
            vm.PaginationInfo.Previous = (vm.PaginationInfo.ActualPage == 0) ? "is-disabled" : "";

            return Ok(vm);


            //return Ok(drivers);

        }


        // GET api/values/5
        [HttpGet]
        [Route("[action]/{id:int}")]
        public async Task<IActionResult> GetbyId(int id)
        {
  

            try
            {
                var model = await _cService.Get(id);

             //   var model = await _context.Customers
             //    .Include(s => s.TransportType)
             //    .Include(t => t.CustomerStatus)
             //    .Include(s => s.CustomerType)
             //    .Include(a => a.Addresses)
             //    .Include("ShipmentDrivers.ShippingStatus")
             //    .Include("ShipmentDrivers.PickupAddress")
             //    .Include("ShipmentDrivers.DeliveryAddress")
             //    .Include("ShipmentSenders.ShippingStatus")
             //        .Include("ShipmentSenders.PickupAddress")
             //    .Include("ShipmentSenders.DeliveryAddress")
             //    .Where(x => x.Id == id)
             //.FirstOrDefaultAsync(); 

                return Ok(model);
                 
            }
            catch (Exception exe)
            {
                return BadRequest("DriverNotFound"+exe.Message);
            }

        }


        [HttpGet]
        [Route("[action]/{userName}")]
        public async Task<IActionResult> GetbyUserName(string userName)
        {
            var c = await _cService.Get(userName);
            if (c == null)
                return Ok("CustomerNoFound");


            return Ok(c);
        }
    }
}
