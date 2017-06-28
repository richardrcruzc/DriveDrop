using DriveDrop.Web.Infrastructure;
using DriveDrop.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopOnContainers.BuildingBlocks.Resilience.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using DriveDrop.Web.Services;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DriveDrop.Web.ViewComponents
{
    public class NewShipmentViewComponent : ViewComponent
    {

        private IHttpClient _apiClient;
        private readonly string _remoteServiceBaseUrl;
        private readonly string _remoteServiceCommonUrl;
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly IHttpContextAccessor _httpContextAccesor;
        private readonly IIdentityParser<ApplicationUser> _appUserParser;


        public NewShipmentViewComponent(IOptionsSnapshot<AppSettings> settings, IHttpContextAccessor httpContextAccesor,
            IHttpClient httpClient, IIdentityParser<ApplicationUser> appUserParser)
        {
            _remoteServiceCommonUrl = $"{settings.Value.DriveDropUrl}/api/v1/common/";
            _remoteServiceBaseUrl = $"{settings.Value.DriveDropUrl}/api/v1/admin";
            _settings = settings;
            _httpContextAccesor = httpContextAccesor;
            _apiClient = httpClient;
            _appUserParser = appUserParser;

        }
        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            var model = new NewShipment();
            await PrepareCustomerModelAsync(model);
            model.CustomerId = id;
            return View(model);
        }


 
        public async Task<NewShipment> PrepareCustomerModelAsync(NewShipment model)
        {
            var getUri = API.Common.GetAllPriorityTypes(_remoteServiceCommonUrl);
            var dataString = await _apiClient.GetStringAsync(getUri);
            var CustomerTypes = new List<SelectListItem>();
            CustomerTypes.Add(new SelectListItem() { Value = null, Text = "Priority", Selected = true });

           // var gets = JArray.Parse(dataString); 
            var responses = JsonConvert.DeserializeObject<List<ListData>>((dataString));

            foreach (var x in responses)
            {
                CustomerTypes.Add(new SelectListItem()
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                });
            }
            model.PriorityTypeList = CustomerTypes;

            //getUri = API.Common.GetAllCustomerStatus(_remoteServiceCommonUrl);
            //dataString = await _apiClient.GetStringAsync(getUri);
            //var customerStatus = new List<SelectListItem>();
            //customerStatus.Add(new SelectListItem() { Value = null, Text = "All", Selected = true });

            //gets = JArray.Parse(dataString);

            //foreach (var brand in gets.Children<JObject>())
            //{
            //    customerStatus.Add(new SelectListItem()
            //    {
            //        Value = brand.Value<string>("id"),
            //        Text = brand.Value<string>("name")
            //    });
            //}
            //model.CustomerStatusList = customerStatus;

            getUri = API.Common.GetAllTransportTypes(_remoteServiceCommonUrl);
            dataString = await _apiClient.GetStringAsync(getUri);
            var transportTypes = new List<SelectListItem>();
            transportTypes.Add(new SelectListItem() { Value = null, Text = "Transport Type", Selected = true });

              responses = JsonConvert.DeserializeObject<List<ListData>>((dataString));

            foreach (var x in responses)
            {
                transportTypes.Add(new SelectListItem()
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                });
            }
            model.TransportTypeList = transportTypes;

            return model;
        }

        
    }
    public class ListData
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
