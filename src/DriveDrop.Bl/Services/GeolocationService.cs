using DriveDrop.Bl.Models;
using DriveDrop.Bl.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DriveDrop.Bl.Services
{
    public class GeolocationService: IGeolocationService
    {
        private readonly HttpClient _apiClient;
        public GeolocationService(IHttpClientAccessor httpClientAccessor)
        {
            _apiClient = httpClientAccessor.HttpClient;
        }
        
             public async Task<List<string>> Autocomplete(string address)
        {   
            //var requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/json?address={0}&sensor=true&key=AIzaSyCwSXLEryUNhIeBBzRN1qxNrqs7Tq15P6o", Uri.EscapeDataString(address));
            var requestUri = string.Format("https://maps.googleapis.com/maps/api/place/autocomplete/json?input={0}&key=AIzaSyCwSXLEryUNhIeBBzRN1qxNrqs7Tq15P6o", Uri.EscapeDataString(address));
            
            var uri = requestUri +
               Uri.EscapeDataString(address);

            var currentDataString = await _apiClient.GetStringAsync(requestUri);

            var results = JsonConvert.DeserializeObject<GoogleGeoCodeResponse>(currentDataString);
            var model = new List<string>();
            foreach (var ad in results.predictions)            
                model.Add(ad.description);
            
            return model;

        }
        public async Task<AddressModel> GetCompleAddress(string address)
        {

            var requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/json?address={0}&sensor=true&key=AIzaSyCwSXLEryUNhIeBBzRN1qxNrqs7Tq15P6o", Uri.EscapeDataString(address));


            var currentDataString = await _apiClient.GetStringAsync(requestUri);
            GoogleGeoCodeResponse results = JsonConvert.DeserializeObject<GoogleGeoCodeResponse>(currentDataString);


            double lat = 0;
            double lng = 0;

            double.TryParse(results.results[0].geometry.location.lat, out lat);
            double.TryParse(results.results[0].geometry.location.lng, out lng);


            var model = new AddressModel
            {
                Id=0,
                CustomerId = 0,
                Street=  results.results[0].address_components[1].long_name,
                City=   results.results[0].address_components[2].short_name,
                State=   results.results[0].address_components[4].short_name,
                Country=     results.results[0].address_components[5].short_name,
                ZipCode = results.results[0].address_components[6].short_name,
                Phone =    "",
                Contact= "",
                Latitude =  lat,
                Longitude= lng
            };

            return model;

        }

        public async Task<AddressModel> Get(string latitude, string longitude)
        {
            var model = new AddressModel { };
            var address = "5215 90th  ";
            //var requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/json?address={0}&sensor=true&key=AIzaSyCwSXLEryUNhIeBBzRN1qxNrqs7Tq15P6o", Uri.EscapeDataString(address));
            var requestUri = string.Format("https://maps.googleapis.com/maps/api/place/autocomplete/json?input={0}&key=AIzaSyCwSXLEryUNhIeBBzRN1qxNrqs7Tq15P6o", Uri.EscapeDataString(address));


            var uri = requestUri +
               Uri.EscapeDataString(address) ;

            var currentDataString = await _apiClient.GetStringAsync(requestUri);

            var currentAdmin = JsonConvert.DeserializeObject<GoogleGeoCodeResponse>(currentDataString);


            return model;
            
        }
        

    }
}

