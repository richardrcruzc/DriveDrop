 
using DriveDrop.Web.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DriveDrop.Web.Services
{
    public interface ICustomerService
    {         
        Task<CustomersList> GetCustomers(int pageIndex, int itemsPage, int? type, int? status, int? transport, string lastName);
        Task<IEnumerable<SelectListItem>> GetCustomerType();
        Task<IEnumerable<SelectListItem>> GetCustomerStatus();
        Task<IEnumerable<SelectListItem>> GetCustomerTrasnport();

    }
}
