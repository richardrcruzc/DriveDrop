using DriveDrop.Bl.Infrastructure;
using DriveDrop.Bl.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; 
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using DriveDrop.Bl.Services;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using DriveDrop.Bl.Data;

namespace DriveDrop.Bl.ViewComponents
{
    public class NewShipmentViewComponent : ViewComponent
    {

        private readonly DriveDropContext _context;
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly IHttpContextAccessor _httpContextAccesor;
        private readonly IIdentityParser<Models.ApplicationUser> _appUserParser;


        public NewShipmentViewComponent(
            DriveDropContext context,
            IOptionsSnapshot<AppSettings> settings, 
            IHttpContextAccessor httpContextAccesor,
             IIdentityParser<Models.ApplicationUser> appUserParser)
        { 
            _settings = settings;
            _httpContextAccesor = httpContextAccesor;
          
            _appUserParser = appUserParser;
            _context = context;



        }
        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
           
            var model = new NewShipment();
            await PrepareCustomerModelAsync(model);
            model.CustomerId = id;
            return View(model);
        }


 
        public async Task<NewShipment> PrepareCustomerModelAsync(NewShipment model)
        {
            var types = await _context.PriorityTypes.Select(x => new { Id = x.Id.ToString(), Name = x.Name }).ToListAsync();

            var CustomerTypes = new List<SelectListItem>();
            CustomerTypes.Add(new SelectListItem() { Value = null, Text = "Priority", Selected = true }); 
           
            foreach (var x in types)
            {
                CustomerTypes.Add(new SelectListItem()
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                });
            }
            model.PriorityTypeList = CustomerTypes;

            var transportTypesObj = await _context.TransportTypes.Select(x => new { Id = x.Id.ToString(), Name = x.Name }).ToListAsync();

            var transportTypes = new List<SelectListItem>();
            transportTypes.Add(new SelectListItem() { Value = null, Text = "Transport Type", Selected = true });
             

            foreach (var x in transportTypesObj)
            {
                transportTypes.Add(new SelectListItem()
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                });
            }
            model.TransportTypeList = transportTypes;

            return model;
        }

        
    }
    public class ListData
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
