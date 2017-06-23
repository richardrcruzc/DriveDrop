using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DriveDrop.Web.ViewModels;
using ApplicationCore.Entities.ClientAgregate;
using ApplicationCore.Entities.Helpers; 

namespace DriveDrop.Web.Services
{
    public class CachedCustomerService //: ICustomerService
    {
        private readonly IMemoryCache _cache;
        private readonly CustomerService _customerService;

        private readonly ICustomerService _icustomerrService;

        private static readonly string _typeKey = "type";
        private static readonly string _statusKey = "status";
        private static readonly string _transportKey = "transport";
        private static readonly string _itemsKeyTemplate = "customers-{0}-{1}-{2}-{3}-{4}";
        private static readonly TimeSpan _defaultCacheDuration = TimeSpan.FromSeconds(30);

        private readonly IIdentityParser<ApplicationUser> _appUserParser;


        public CachedCustomerService(IMemoryCache cache, ICustomerService icustomerrService,
        CustomerService customerService, IIdentityParser<ApplicationUser> appUserParser)
        {
            _cache = cache;
            _customerService = customerService;
            _appUserParser = appUserParser;
            _icustomerrService = icustomerrService;
        }

         

        public async Task<CustomersList> GetCustomers(int pageIndex, int itemsPage, int? type, int? status, int? transport, string lastName)
        {
            string cacheKey = String.Format(_itemsKeyTemplate, pageIndex, itemsPage, type, status, transport);
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = _defaultCacheDuration;
                return await _customerService.GetCustomers(pageIndex, itemsPage,type,status,transport, lastName);
            });
        }

        public async Task<IEnumerable<SelectListItem>> GetCustomerType()
        {
            return await _cache.GetOrCreateAsync(_typeKey, async entry =>
            {
                entry.SlidingExpiration = _defaultCacheDuration;
                return await _customerService.GetCustomerType();
            });
        }

        public async Task<IEnumerable<SelectListItem>> GetCustomerStatus()
        {
            return await _cache.GetOrCreateAsync(_statusKey, async entry =>
            {
                entry.SlidingExpiration = _defaultCacheDuration;
                return await _customerService.GetCustomerStatus();
            });
        }

        public async Task<IEnumerable<SelectListItem>> GetCustomerTrasnport()
        {
            return await _cache.GetOrCreateAsync(_transportKey, async entry =>
            {
                entry.SlidingExpiration = _defaultCacheDuration;
                return await _customerService.GetCustomerType();
            });
        }
    }
}
