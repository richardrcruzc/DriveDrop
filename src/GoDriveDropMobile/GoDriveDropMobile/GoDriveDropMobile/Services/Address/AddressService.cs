using GoDriveDrop.Core.Identity;
using GoDriveDrop.Core.Models.Commons;
using GoDriveDrop.Core.Services.RequestProvider;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace GoDriveDrop.Core.Services.Address
{
    public    class AddressService: IAddressService
    { 
    private IIdentityService _identityService;
    private readonly IRequestProvider _requestProvider;
    private const string ApiUrlBase = "address/";
    public AddressService(
        IIdentityService identityService,
        IRequestProvider requestProvider)
    {
        _identityService = identityService;
        _requestProvider = requestProvider;
    }

        public Task<string> CreateAsync(AddressModel address)
        {
            throw new NotImplementedException();
        }

        public Task<ObservableCollection<AddressModel>> GetAsync(string token)
        {
            throw new NotImplementedException();
        }

        public Task<AddressModel> GetAsync(int driverId, string token)
        {
            throw new NotImplementedException();
        }

        public Task<AddressModel> GetAsync(string userName, string token)
        {
            throw new NotImplementedException();
        }
    }
 }