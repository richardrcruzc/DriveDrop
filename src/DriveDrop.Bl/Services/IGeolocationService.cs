
using ApplicationCore.Entities.ClientAgregate;
using DriveDrop.Bl.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Bl.Services
{
    public interface IGeolocationService
    {

        Task<AddressModel> Get(string latitude, string longitude);
        Task<AddressModel> GetCompleAddress(string address);
        Task<List<string>> Autocomplete(string address);

    }
}
