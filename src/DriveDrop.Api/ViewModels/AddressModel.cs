using ApplicationCore.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Api.ViewModels
{
    public class AddressModel 
    {
        public int CustomerId { get; set; }
        public String TypeAddress { get;  set; }

        public String Street { get;  set; }

        public String City { get;  set; }

        public String State { get;  set; }

        public String Country { get;  set; }

        public String ZipCode { get;  set; }

        public String Phone { get;  set; }
        public String Contact { get;  set; }


        public Double Latitude { get;  set; }
        public Double Longitude { get;  set; }
        protected AddressModel() { }

        public AddressModel(string street, string city, string state, string country, string zipcode,string phone, string contact, Double latitude, Double longitude, string typeAddress ="home" )
        {
            TypeAddress = typeAddress;
            Street = street;
            City = city;
            State = state;
            Country = country;
            ZipCode = zipcode;
            Phone = phone;
            Contact = contact;
            Latitude = latitude;
            Longitude = longitude;
        }

         
    }
}
