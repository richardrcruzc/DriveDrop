using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Api.Infrastructure.Services
{
    public interface IIdentityService
    {
        string GetUserIdentity();
    }
}
