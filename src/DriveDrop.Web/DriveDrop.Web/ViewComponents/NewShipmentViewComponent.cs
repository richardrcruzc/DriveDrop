using DriveDrop.Web.Infrastructure;
using DriveDrop.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DriveDrop.Web.ViewComponents
{
    public class NewShipmentViewComponent : ViewComponent
    {

        private readonly DriveDropContext _context;
        public NewShipmentViewComponent(DriveDropContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id )
        {
           var model  = new NewShipment();
           await PrepareCustomerModelAsync(model);
            model.CustomerId = id;
            return View(model);
        }
        


        public async Task<NewShipment> PrepareCustomerModelAsync(NewShipment model)
        {
            model.CustomerTypeList = await  _context.CustomerTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToListAsync();
            model.TransportTypeList = await _context.TransportTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToListAsync();
            model.CustomerStatusList = await _context.CustomerStatuses.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToListAsync();

            model.PriorityTypeList = await _context.PriorityTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToListAsync();
             
            return model;
        }
    }
}
