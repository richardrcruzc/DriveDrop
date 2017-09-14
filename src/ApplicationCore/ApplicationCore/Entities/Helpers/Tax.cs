using ApplicationCore.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities.Helpers
{

    public class Tax : Entity
    {
        public string State { get; private set; }
        public string County { get; private set; }
        public string City{ get; private set; }        
        public decimal Rate { get; private set; }
        public bool RateDefault { get; private set; }

        public Tax() { }
        public Tax(string state,string county,  string city, decimal rate, bool rateDefault)
        {
            State = state;
            County = county;
            City = city;
            Rate = rate;
            RateDefault = rateDefault;
        }
        public Tax Update (string state, string county, string city, decimal rate, bool rateDefault)
        {
            State = state;
            County = county;
            City = City;
            Rate = rate;
            RateDefault = rateDefault;
            return this;
        }
        public Tax SetDefault(bool rateDefault)
        {
            
            RateDefault = rateDefault;
            return this;
        }
    }

}