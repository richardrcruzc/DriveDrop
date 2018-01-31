
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
using DriveDrop.Bl.Models;
using AutoMapper;

namespace DriveDrop.Bl.ViewComponents
{
    public class ShippingByDriverViewComponent : ViewComponent
    {
        private readonly IMapper _mapper;
        private readonly DriveDropContext _context;

        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly IHttpContextAccessor _httpContextAccesor; 


        public ShippingByDriverViewComponent(IOptionsSnapshot<AppSettings> settings,
            IHttpContextAccessor httpContextAccesor,
          IMapper mapper,
            DriveDropContext context)
        {
            _mapper = mapper;
            _settings = settings;
            _httpContextAccesor = httpContextAccesor;
 
            _context = context;
        }



        public async Task<IViewComponentResult> InvokeAsync(int driverId)
        {
            int pageSize = 10;
            int pageIndex = 0;


            var root = _context.Shipments
          .Where(x => x.DriverId == driverId).OrderBy(x => x.DriverId)
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

            var query = _mapper.Map<List<ApplicationCore.Entities.ClientAgregate.ShipmentAgregate.Shipment>, List<ShipmentModel>>(itemsOnPage);

            query = ChangeUriPlaceholder(query);

            var model = new PaginatedItemsViewModel<ShipmentModel>(
                pageIndex, pageSize, totalItems, query); 

            return View(model);

        }



        public async Task<ViewModels.NewShipment> PrepareCustomerModelAsync(ViewModels.NewShipment model)
        {
            var types = await _context.CustomerTypes.Select(x => new { Id = x.Id.ToString(), x.Name }).ToListAsync();
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

            var cStatus = await _context.CustomerStatuses.Select(x => new { Id = x.Id.ToString(), x.Name }).ToListAsync();

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

            var tTypes = await _context.TransportTypes.Select(x => new { Id = x.Id.ToString(),   x.Name }).ToListAsync();
            var transportTypes = new List<SelectListItem>
            {
                new SelectListItem() { Value = null, Text = "All", Selected = true }
            };



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
        private List<ShipmentModel> ChangeUriPlaceholder(List<ShipmentModel> items)
        {
            //var baseUri = _settings.ExternalCatalogBaseUrl;
            var baseUri = "";

            items.ForEach(x =>
            {
               // x.SetDeliveredPictureUri(baseUri + "/" + x.DeliveredPictureUri);
            });

            return items;
        }
    }
}
