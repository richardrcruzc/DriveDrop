using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DriveDrop.Api.Services;
using DriveDrop.Api.ViewModels;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Net.Http;
using Newtonsoft.Json;

namespace DriveDrop.Api.Controllers
{
    [Route("api/v1/[controller]")]
    public class LocationController : Controller
    {
         
        public LocationController(  )
        {
            
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Index()
        {
            var model = new AddressModel(0, 0, "", "", "", "", "", "", "", 0, 0);
            var address = "5215 90th ste e 98446 tacoma wa";

            var requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/json?key=AIzaSyCwSXLEryUNhIeBBzRN1qxNrqs7Tq15P6o&address={0}&sensor=false", Uri.EscapeDataString(address));
            requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/json?key=AIzaSyCwSXLEryUNhIeBBzRN1qxNrqs7Tq15P6o&latlng={0},{1}&sensor=false", "47.1763006", "-122.359928");

            var client = new HttpClient();
            var stringTask = await client.GetStringAsync(requestUri);
            var response = JsonConvert.DeserializeObject<GoogleGeoCodeResponse>(stringTask);
            if (!response.status.Contains("Ok"))
                return BadRequest(model);
            foreach (var a in response.results)
            {

                var ac = a.address_components;

                

            }



            // var currentAdmin = JsonConvert.DeserializeObject<GoogleGeoCodeResponse>((currentDataString));


            return Ok(model);

        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Index(AddressModel model)
        {
             var address = "5215 90th ste e 98446 tacoma wa";

            var requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/json?key=AIzaSyCwSXLEryUNhIeBBzRN1qxNrqs7Tq15P6o&address={0}&sensor=false", Uri.EscapeDataString(address));
            requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/json?key=AIzaSyCwSXLEryUNhIeBBzRN1qxNrqs7Tq15P6o&latlng={0},{1}&sensor=false", "47.1763006", "-122.359928");

            var client = new HttpClient();
            var stringTask = await client.GetStringAsync(requestUri);
            var response = JsonConvert.DeserializeObject<GoogleGeoCodeResponse>(stringTask);


            // var currentAdmin = JsonConvert.DeserializeObject<GoogleGeoCodeResponse>((currentDataString));


            return Ok(model);

        }
    }
}