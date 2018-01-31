
using DriveDrop.Bl.Infrastructure;
using DriveDrop.Bl.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options; 
using Microsoft.AspNetCore.Http;
using DriveDrop.Bl.Services;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json.Linq;
using DriveDrop.Bl.Data;

namespace DriveDrop.Bl.ViewComponents
{
    public class ShippingByCustomerViewComponent : ViewComponent
    {



        private readonly DriveDropContext _context;
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly IHttpContextAccessor _httpContextAccesor;
        private readonly IIdentityParser<Models.ApplicationUser> _appUserParser;


        public ShippingByCustomerViewComponent(IOptionsSnapshot<AppSettings> settings, IHttpContextAccessor httpContextAccesor,
            IIdentityParser<Models.ApplicationUser> appUserParser, DriveDropContext context)
        {
             
            _settings = settings;
            _httpContextAccesor = httpContextAccesor;
            _context = context;
             _appUserParser = appUserParser;

        }


         

        public async Task<IViewComponentResult> InvokeAsync(int customerId)
        {
            try
            {
                var model = await _context.Shipments
                    .OrderBy(s => s.SenderId)
               .Where(x => x.SenderId == customerId)
               .Include(d => d.DeliveryAddress)
               .Include(d => d.PickupAddress)
               .Include(d => d.ShippingStatus)
               .Include(d => d.PriorityType)
               .Include(s => s.Reviews)
               .OrderByDescending(x => x.Id)
               .ToListAsync();

                return View(model);


            }
            catch (Exception)
            {
                return View();
            }

        }


        public async Task<NewShipment> PrepareCustomerModelAsync(NewShipment model)
        {
            var types = await _context.CustomerTypes.Select(x => new { Id = x.Id.ToString(),   x.Name }).ToListAsync();
            var CustomerTypes = new List<SelectListItem>();
            CustomerTypes.Add(new SelectListItem() { Value = null, Text = "All", Selected = true });

             
            foreach (var brand in types)
            {
                CustomerTypes.Add(new SelectListItem()
                {
                    Value = brand.Id.ToString(),
                    Text = brand.Name
                });
            }
            model.CustomerTypeList = CustomerTypes;

            var cStatus = await _context.CustomerStatuses.Select(x => new { Id = x.Id.ToString(),   x.Name }).ToListAsync();

            var customerStatus = new List<SelectListItem>();
            customerStatus.Add(new SelectListItem() { Value = null, Text = "All", Selected = true });
             

            foreach (var brand in cStatus)
            {
                customerStatus.Add(new SelectListItem()
                {
                    Value = brand.Id.ToString(),
                    Text = brand.Name
                });
            }
            model.CustomerStatusList = customerStatus;

            var tTypes = await _context.TransportTypes.Select(x => new { Id = x.Id.ToString(), Name = x.Name }).ToListAsync();
            var transportTypes = new List<SelectListItem>();
            transportTypes.Add(new SelectListItem() { Value = null, Text = "All", Selected = true });

            

            foreach (var brand in tTypes)
            {
                transportTypes.Add(new SelectListItem()
                {
                    Value = brand.Id.ToString(),
                    Text = brand.Name
                });
            }
            model.TransportTypeList = transportTypes;

            return model;
        }
       
    }
}
