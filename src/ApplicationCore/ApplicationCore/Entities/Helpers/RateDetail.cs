using ApplicationCore.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities.Helpers
{
    public class RateDetail: Entity
    {
        public string WeightOrDistance { get; private set; }
        public string MileOrLbs { get; private set; }
        public decimal From { get; private set; }
        public decimal To { get; private set; }
        public decimal Charge { get; private set; } 

        public RateDetail() { }

        public RateDetail( string weightOrDistance, string mileOrLbs, decimal from, decimal to, decimal charge)
        {
            
            WeightOrDistance = weightOrDistance;
            MileOrLbs = mileOrLbs;
            From = from;
            To = to;
            Charge = charge;
        }
        public RateDetail Update(  string weightOrDistance, string mileOrLbs, decimal from, decimal to, decimal charge)
        {
           
            WeightOrDistance = weightOrDistance;
            MileOrLbs = mileOrLbs;
            From = from;
            To = to;
            Charge = charge;

            return this;
        }
    }
}
