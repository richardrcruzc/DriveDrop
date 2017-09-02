using DriveDrop.Api.ViewModels;
using Microsoft.eShopOnContainers.BuildingBlocks.Resilience.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DriveDrop.Api.Services
{
    public class GeolocationService: IGeolocationService
    {
        private IHttpClient _apiClient;
        public GeolocationService(IHttpClient httpClient)
        {
            _apiClient = httpClient;
        }
        
        public async Task<AddressModel> Get(string latitude, string longitude)
        {
            var model = new AddressModel(0,0,"", "", "", "", "", "", "", 0, 0);
            var address = "5215 90th ste e 98446 tacoma wa";

            var requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?key=AIzaSyCwSXLEryUNhIeBBzRN1qxNrqs7Tq15P6o&address={0}&sensor=false", Uri.EscapeDataString(address));


            var currentDataString = await _apiClient.GetStringAsync(requestUri);

            var currentAdmin = JsonConvert.DeserializeObject<GoogleGeoCodeResponse>((currentDataString));


            return model;
            
        }
        public async Task<AddressModel> Get(AddressModel model)
        {

             var address = "5215 90th ste e 98446 tacoma wa";

            var requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false", Uri.EscapeDataString(address));


            var currentDataString = await _apiClient.GetStringAsync(requestUri);



            return model;

        }

    }
}
