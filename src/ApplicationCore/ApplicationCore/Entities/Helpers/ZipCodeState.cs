using ApplicationCore.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities.Helpers
{
    public class ZipCodeState: Entity
    {
        public string ZipCode { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string County { get; private set; }
        public ZipCodeState() { }
        public ZipCodeState(string zipCode, double latitude, double longitude, string city, string state, string county)
        {
            ZipCode = zipCode;
            Latitude = latitude;
            Longitude = longitude;
            City = city;
            State = state;
            County = county;
        }
    }
}
