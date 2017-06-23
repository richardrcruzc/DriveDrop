using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;
using DriveDrop.Web.Infrastructure;
using DriveDrop.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Web.ViewComponents
{
    public class ShippingNoAssignedViewComponent : ViewComponent
    {

        private readonly DriveDropContext _context;
        public ShippingNoAssignedViewComponent(DriveDropContext context)
        {
            _context = context;
        }


        public async Task<IViewComponentResult> InvokeAsync( )
        {
            //var model = new NewShipment();
            //await PrepareCustomerModelAsync(model);
            //model.CustomerId = id;

            var model = await   _context.Shipments
                .Where(x => x.ShippingStatusId == ShippingStatus.PendingPickUp.Id && x.DriverId == null)
                .Include(d => d.DeliveryAddress) 
                .Include(d => d.PickupAddress)
                .Include(d => d.ShippingStatus)
                .ToListAsync();

            return View(model);
        }


        public async Task<NewShipment> PrepareCustomerModelAsync(NewShipment model)
        {
            model.CustomerTypeList = await _context.CustomerTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToListAsync();
            model.TransportTypeList = await _context.TransportTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToListAsync();
            model.CustomerStatusList = await _context.CustomerStatuses.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToListAsync();

            model.PriorityTypeList = await _context.PriorityTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToListAsync();

            return model;
        }

    }
}
