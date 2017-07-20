using ApplicationCore.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApplicationCore.Entities.Helpers
{
   public class Rate: Entity
    {

        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public decimal FixChargePerShipping { get; private set; }
        public bool FixChargePercentage { get; private set; }
        public decimal ChargePerItem { get; private set; }
        public decimal Tax { get; private set; }
        public bool Active { get; private set; }

        public List<RateDetail> RateDetails { get; private set; }
        public List<RatePriority> RatePriorities { get; private set; }
        public List<PackageSize> PackageSizes { get; private set; }


        public Rate() { RateDetails = new List<RateDetail>(); }

        public Rate(DateTime startDate, DateTime endDate, decimal markUp, decimal chargePerItem, decimal tax) {

            StartDate = startDate;
            EndDate = endDate;
            FixChargePerShipping = markUp;
            ChargePerItem = chargePerItem;
            Tax = tax;
            Active = true;
        }
        public Rate Update(DateTime startDate, DateTime endDate, decimal markUp, decimal chargePerItem, decimal tax, bool active)
        {

            StartDate = startDate;
            EndDate = endDate;
            FixChargePerShipping = markUp;
            ChargePerItem = chargePerItem;
            Tax = tax;
            Active = active;


            return this;
        }
        public Rate AddDetails(RateDetail detail)
        {
            foreach (var r in RateDetails.ToList())
            {
                if (r.From != detail.From && r.Charge != detail.Charge
                    && r.MileOrLbs != detail.MileOrLbs)
                    RateDetails.Add(detail);

                //if (r.Id == detail.Id)
                //    RateDetails.Remove(detail);                
                //RateDetails.Add(detail);
            }

            /*
              
            && r.From==detail.From 
            && r.Charge = detail.Charge 
            && r.MileOrLbs == detail.MileOrLbs
             */
            return this;
        }
    }
}

