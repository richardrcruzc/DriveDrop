
using ApplicationCore.Entities.ClientAgregate;
using DriveDrop.Bl.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DriveDrop.Bl.Services
{    
        public interface ICustomerService
        {
            Task<CurrentCustomerModel> Get(string user,int customerId);
        Task<CurrentCustomerModel> Get(int customerId);
        Task<CurrentCustomerModel> Get(string user);
        Task<CustomerIndex> Get(int? customertype, int? statusId, int? transporTypeId, string LastName = "", int pageIndex = 0, int pageSize = 10);
        Task<bool> SetImpersonate(string adminUser, string userName);
        Task<bool> IsImpersonate(string adminUser);
        Task<bool> EndImpersonated(string adminUser);
    }
    }
 