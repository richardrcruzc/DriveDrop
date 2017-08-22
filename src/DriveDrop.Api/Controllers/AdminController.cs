﻿using ApplicationCore.Entities.ClientAgregate;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Api.Controllers
{

    [Route("api/v1/[controller]")]
   // [Authorize]
    public class AdminController : Controller
    {
        private readonly ICustomerService _cService;
        private readonly DriveDropContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IIdentityService _identityService;
        public AdminController(ICustomerService cService, IHostingEnvironment env, DriveDropContext context, IIdentityService identityService)
        {
            _cService = cService;
            _context = context;
            _env = env;
            _identityService = identityService;
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
            var root = (IQueryable<Customer>)_context.Customers;

            if (customertype.HasValue)
            {
                root = root.Where(ci => ci.CustomerTypeId == customertype);
            }
            if (statusId.HasValue)
            {
                root = root.Where(ci => ci.CustomerStatus.Id == statusId);
            }

            if (transporTypeId.HasValue)
            {
                root = root.Where(ci => ci.TransportType.Id == transporTypeId);
            }
            if (LastName!="null")
            {
                root = root.Where(l => l.LastName.Contains(LastName));
                //root = root.Where(l => l.FirstName.Contains(LastName));
            }

            var totalItems = await root
                .LongCountAsync();

            var itemsOnPage = await root
                   .Include(x => x.CustomerType).Include(x => x.CustomerStatus).Include(x => x.DefaultAddress)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .Include(x => x.CustomerStatus)
                .Include(y => y.CustomerType)
                .Include(z => z.TransportType)
                .ToListAsync();


            var model = new PaginatedItemsViewModel<Customer>(
               pageIndex, pageSize, totalItems, itemsOnPage);



            //itemsOnPage = ComposePicUri(itemsOnPage);

            //var drivers = new CustomersList() { Data = model, PageIndex = pageIndex, Count = (int)totalItems, PageSize = pageSize };

            //var customerList = drivers.Data.Select(x => new  CustomerViewModel
            //{
            //    Email = x.Email,
            //    Commission = x.Commission,
            //    CustomerStatus = x.CustomerStatus.Name,
            //    CustomerType = x.CustomerType.Name,
            //    Id = x.Id,
            //    DeliverRadius = x.DeliverRadius,
            //    FirstName = x.FirstName,
            //    LastName = x.LastName,
            //    Phone = x.Phone,
            //    DriverLincensePictureUri = x.DriverLincensePictureUri,
            //    TransportType = x.TransportType.Name,
            //    MaxPackage = x.MaxPackage,
            //    PickupRadius = x.PickupRadius,
            //}).ToList();

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
            items.Add(new SelectListItem() { Value = null, Text = "All", Selected = true });
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
            items.Add(new SelectListItem() { Value = null, Text = "All", Selected = true });
            foreach (var itm in dataItems)
            {
                items.Add(new SelectListItem()
                {
                    Value = itm.Value,
                    Text = itm.Text
                });
            }
            vm.CustomerStatus = items;

              dataItems = await _context.TransportTypes.Select(x => (new SelectListItem { Value = x.Id.ToString(), Text = x.Name })).ToListAsync();
              items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Value = null, Text = "All", Selected = true });
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
