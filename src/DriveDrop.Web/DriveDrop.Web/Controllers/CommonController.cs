using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.BuildingBlocks.Resilience.Http;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using DriveDrop.Web.Services;
using DriveDrop.Web.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using DriveDrop.Web.Infrastructure;
using Microsoft.AspNetCore.Authorization;

namespace DriveDrop.Web.Controllers
{
    [Authorize]
    public class CommonController : Controller
    {
        private IHttpClient _apiClient;
        private readonly string _remoteServiceCommonUrl;

        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly IHttpContextAccessor _httpContextAccesor;
        private readonly IIdentityParser<ApplicationUser> _appUserParser;

        private readonly IHostingEnvironment _env;

        public CommonController(IOptionsSnapshot<AppSettings> settings, IHttpContextAccessor httpContextAccesor,
            IHttpClient httpClient, IIdentityParser<ApplicationUser> appUserParser,
            IHostingEnvironment env)
        {
            _remoteServiceCommonUrl = $"{settings.Value.DriveDropUrl}/api/v1/common/";
           
            _settings = settings;
            _httpContextAccesor = httpContextAccesor;
            _apiClient = httpClient;
            _appUserParser = appUserParser;

            _env = env;
        }


        public IActionResult AddressAdd(int id)
        {
            ViewBag.Id = id;
            var model = new AddressModel { CustomerId = id };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<string> AddressAdd(AddressModel model)
        {
            var result = "Address added";
            ViewBag.Id = model.CustomerId;
            //call shipping api service
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var updateInfo = API.Common.AddAddress(_remoteServiceCommonUrl);

            var response = await _apiClient.PostAsync(updateInfo, model, token);
            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                //throw new Exception("Error creating Shipping, try later.");

                ModelState.AddModelError("", "Error creating Shipping, try later.");
                result = "Error creating Shipping, try later.";
            }

            return result;
        }

        public async Task<IActionResult> Address(int id)
        {
            ViewBag.Id = id;
            //call shipping api service
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var getUser = API.Common.GetUser(_remoteServiceCommonUrl, user.Email);

            var dataString = await _apiClient.GetStringAsync(getUser, token);

            var response = JsonConvert.DeserializeObject<Customer>((dataString));

            var listAddresses = new List<AddressModel>();
            foreach (var a in response.Addresses)
            {
                if (response.DefaultAddress.Id == a.Id)
                    a.TypeAddress = "default";
                listAddresses.Add(a);
            }

            return View(listAddresses);

        }



        //[HttpPost]
        // [ValidateAntiForgeryToken]
        public async Task<string> DefaultAddress(int id)
        {
            var result = "Address updated";

            //call shipping api service
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var getUser = API.Common.GetUser(_remoteServiceCommonUrl, user.Email);

            var dataString = await _apiClient.GetStringAsync(getUser, token);

            var customer = JsonConvert.DeserializeObject<Customer>((dataString));
            if (customer == null)
                return "Address Not Found";
            ViewBag.Id = customer.Id;

            var defaultAddress = customer.Addresses.Where(x => x.Id == id).FirstOrDefault();

            if (defaultAddress == null)
                return "Address Not Found";


            var updateInfo = API.Common.DefaultAddress(_remoteServiceCommonUrl, customer.Id, defaultAddress.Id);

            var dataString1 = await _apiClient.GetStringAsync(updateInfo,  token);
              result = JsonConvert.DeserializeObject<string>((dataString1)); 

            return result;
        }
         [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<string> AddressDelete(int id)
        {
            var result = "Address deleted";

            //call shipping api service
            var user = _appUserParser.Parse(HttpContext.User);
            var token = await GetUserTokenAsync();

            var getById = API.Common.GetUser(_remoteServiceCommonUrl, user.Email);

            var dataString = await _apiClient.GetStringAsync(getById, token);

            var customer = JsonConvert.DeserializeObject<Customer>((dataString));
            if (customer == null)
                return "Address Not Found";
            ViewBag.Id = customer.Id;

            var addressToDelete = customer.Addresses.Where(x => x.Id == id).FirstOrDefault();

            if (addressToDelete == null)
                return "Address Not Found";

            if (addressToDelete.TypeAddress.ToLower() == "home" || addressToDelete.Id == customer.DefaultAddress.Id)
                return "Cannot delete default address";


            var updateInfo = API.Common.DeleteAddress(_remoteServiceCommonUrl, customer.Id,addressToDelete.Id);

            var response = await _apiClient.PostAsync(updateInfo, addressToDelete, token);
            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                //throw new Exception("Error creating Shipping, try later.");

                ModelState.AddModelError("", "Error deleting address, try later.");
                result = "Error deleting address, try later.";
            }

            return result;
        }
        async Task<string> GetUserTokenAsync()
        {
            var context = _httpContextAccesor.HttpContext;

            return await context.Authentication.GetTokenAsync("access_token");
        }
    }
}