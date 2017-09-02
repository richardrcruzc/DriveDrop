using DriveDrop.Api.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Api.Services
{
    public interface IGeolocationService
    {

        Task<AddressModel> Get(string latitude, string longitude);
        Task<AddressModel> Get(AddressModel model);

    }
}
