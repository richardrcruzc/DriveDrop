using DriveDrop.Web.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DriveDrop.Web.ViewModels; 

namespace DriveDrop.Web.Services
{
    //public class CustomerService: ICustomerService
    //{

    //    private readonly DriveDropContext _context;
    //    private readonly IOptionsSnapshot<DriveDropSettings> _settings;
    //    private readonly ILogger<CustomerService> _logger;


    //    public CustomerService(DriveDropContext context,
    //      IOptionsSnapshot<DriveDropSettings> settings,
    //      ILoggerFactory loggerFactory)
    //    {
    //        _context = context;
    //        _settings = settings;
    //        _logger = loggerFactory.CreateLogger<CustomerService>();
    //    }
         


    //    public async Task<CustomersList> GetCustomers(int pageIndex, int itemsPage, int? typeId, int? status, int? transport, string lastName)
    //    {
    //        _logger.LogInformation("GetCustomers called.");

    //        var root = (IQueryable<Customer>)_context.Customers;

    //        if (typeId.HasValue)
    //        {
    //            root = root.Where(ci => ci.CustomerType.Id == typeId);
    //        }

    //        if (status.HasValue)
    //        {
    //            root = root.Where(ci => ci.CustomerStatus.Id == status);
    //        }

    //        if (transport.HasValue)
    //        {
    //            root = root.Where(ci => ci.TransportType.Id == transport);
    //        }
    //        if (!string.IsNullOrEmpty(lastName))
    //        {
    //            root = root.Where(l => l.LastName.Contains(lastName));
    //        }

    //        var totalItems = await root
    //            .LongCountAsync();

    //        var itemsOnPage = await root
    //            .Skip(itemsPage * pageIndex)
    //            .Take(itemsPage)
    //            .ToListAsync();

    //        itemsOnPage = ComposePicUri(itemsOnPage);

    //        return new CustomersList() { Data = itemsOnPage, PageIndex = pageIndex, Count = (int)totalItems };
    //    }

    //    public async Task<IEnumerable<SelectListItem>> GetCustomerStatus()
    //    {
    //        _logger.LogInformation("GetStatus called.");

    //        var status = await _context.CustomerStatuses.ToListAsync();

    //        var items = new List<SelectListItem>
    //        {
    //            new SelectListItem() { Value = null, Text = "All", Selected = true }
    //        };
    //        foreach (var item in status)
    //        {
    //            items.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Name });
    //        }
    //        return items;
    //    }

    //    public async Task<IEnumerable<SelectListItem>> GetCustomerTrasnport()
    //    {
    //        _logger.LogInformation("GetTrasnport called.");

    //        var status = await _context.TransportTypes.ToListAsync();

    //        var items = new List<SelectListItem>
    //        {
    //            new SelectListItem() { Value = null, Text = "All", Selected = true }
    //        };
    //        foreach (var item in status)
    //        {
    //            items.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Name });
    //        }
    //        return items; 
    //    }

    //    public async Task<IEnumerable<SelectListItem>> GetCustomerType()
    //    {
    //        _logger.LogInformation("GetType called.");

    //        var status = await _context.CustomerTypes.ToListAsync();

    //        var items = new List<SelectListItem>
    //        {
    //            new SelectListItem() { Value = null, Text = "All", Selected = true }
    //        };
    //        foreach (var item in status)
    //        {
    //            items.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Name });
    //        }
    //        return items;
    //    }

         

    //    private List<Customer> ComposePicUri(List<Customer> items)
    //    {
    //        //var baseUri = _settings.Value.DriveDropBaseUrl;
    //        //items.ForEach(x =>
    //        //{
    //        //    x.AddPicture(x.DriverLincensePictureUri.Replace("http://catalogbaseurltobereplaced", baseUri));
    //        //});

    //        return items;
    //    }

        
    //}
}
