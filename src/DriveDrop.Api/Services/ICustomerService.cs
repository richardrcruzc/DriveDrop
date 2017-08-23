using DriveDrop.Api.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Api.Services
{    
        public interface ICustomerService
        {
            Task<CurrentCustomerModel> Get(string user,int customerId);
        Task<CurrentCustomerModel> Get(int customerId);
        Task<CurrentCustomerModel> Get(string user);
        Task<bool> SetImpersonate(string adminUser, string userName);
        Task<bool> IsImpersonate(string adminUser);
        Task<bool> EndImpersonated(string adminUser);
    }
    }
 