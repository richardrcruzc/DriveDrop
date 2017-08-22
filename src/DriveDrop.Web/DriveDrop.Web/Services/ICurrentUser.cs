using DriveDrop.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Web.Services
{
    public interface ICurrentUser
    { 
              Task<CurrentCustomerModel> Get(string user, int customerId, string impersonateUser);
    }
}
