using ApplicationCore.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities.Helpers
{

    public class Tax : Entity
    {
        public string State { get; private set; }
        public string City{ get; private set; }        
        public decimal Rate { get; private set; }

        public Tax() { }
        public Tax(string state, string city, decimal rate)
        {
            State = state;
            City = city;
            Rate = rate; 
        }
        public Tax Update (string state, string city, decimal rate)
        {
            State = state;
            City = City;
            Rate = rate;

            return this;
        }
    }

}