using ApplicationCore.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.ClientAgregate
{
    public class Address
       : ValueObject
    {
        public String Street { get; private set; }

        public String City { get; private set; }

        public String State { get; private set; }

        public String Country { get; private set; }

        public String ZipCode { get; private set; }

        public String Phone { get; private set; }
        public String Contact { get; private set; }


        public Double Latitude { get; private set; }
        public Double Longitude { get; private set; }
        protected Address() { }

        public Address(string street, string city, string state, string country, string zipcode,string phone, string contact, Double latitude, Double longitude )
        {
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

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Street;
            yield return City;
            yield return State;
            yield return Country;
            yield return Phone;
            yield return Contact;
            yield return Latitude;
            yield return Longitude;
        }
    }
}
