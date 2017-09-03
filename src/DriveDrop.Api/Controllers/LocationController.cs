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
        public async Task<IActionResult> GetByLatAndLong([FromQuery]string latitude, [FromQuery]string longitude)
        {
           
           // var address = "5215 90th ste e 98446 tacoma wa";

            //var requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/json?key=AIzaSyCwSXLEryUNhIeBBzRN1qxNrqs7Tq15P6o&address={0}&sensor=false", Uri.EscapeDataString(address));
            var requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/json?key=AIzaSyCwSXLEryUNhIeBBzRN1qxNrqs7Tq15P6o&latlng={0},{1}&sensor=false", latitude, longitude);

            var client = new HttpClient();
            var stringTask = await client.GetStringAsync(requestUri);
            var response = JsonConvert.DeserializeObject<GoogleGeoCodeResponse>(stringTask);
            if (!response.status.Contains("OK"))
                return BadRequest(null);




            var actualAddress = response.results.Where(x => x.types[0].ToString() == "street_address").Select(x=>new { address = x.formatted_address, lat = x.geometry.location.lat, lng = x.geometry.location.lng }).FirstOrDefault();
            if (actualAddress == null)
                return BadRequest(null);

            var aAddress = actualAddress.address.Split(',');
            var lat = double.Parse(actualAddress.lat);
            var lng = double.Parse(actualAddress.lng);

            var street = aAddress[0];
            var city = aAddress[1];
            var state = aAddress[2].Trim().Split(' ')[0];
            var zipcode = aAddress[2].Trim().Split(' ')[1];
            var country = aAddress[3];


            var model = new AddressModel(0, 0, street,city,state,country, zipcode,"","", lat,lng);


            return Ok(model);

        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetByStreetAddress([FromBody]AddressModel model)
        {
            
            var address = string.Format("{0},{1},{2} {3},{4}", model.Street, model.City, model.State, model.ZipCode, model.Country  );

             var requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/json?key=AIzaSyCwSXLEryUNhIeBBzRN1qxNrqs7Tq15P6o&address={0}&sensor=false", Uri.EscapeDataString(address));
            
            var client = new HttpClient();
            var stringTask = await client.GetStringAsync(requestUri);
            var response = JsonConvert.DeserializeObject<GoogleGeoCodeResponse>(stringTask);
            if (!response.status.Contains("OK"))
                return BadRequest(null);




            var actualAddress = response.results.Where(x => x.types[0].ToString() == "street_address").Select(x => new { address = x.formatted_address, lat = x.geometry.location.lat, lng = x.geometry.location.lng }).FirstOrDefault();
            if(actualAddress==null)
                return BadRequest(null);
            var aAddress = actualAddress.address.Split(',');
            var lat = double.Parse(actualAddress.lat);
            var lng = double.Parse(actualAddress.lng);

            model.Latitude = lat;
            model.Longitude = lng;

            //var street = aAddress[0];
            //var city = aAddress[1];
            //var state = aAddress[2].Trim().Split(' ')[0];
            //var zipcode = aAddress[2].Trim().Split(' ')[1];
            //var country = aAddress[3];




            return Ok(model);


        }
    }
}