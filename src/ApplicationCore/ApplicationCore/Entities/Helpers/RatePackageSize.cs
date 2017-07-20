using ApplicationCore.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities.Helpers
{
    public class RatePackageSize : Entity
    {
       
        public int PackageSizeId { get; private set; } 

        public decimal Charge { get; private set; }
        public bool ChargePercentage { get; private set; }

        public int RateId { get; private set; }
        public Rate Rate { get; private set; }

        public RatePackageSize() { }

        public RatePackageSize(int rateId, int packageSizeId, decimal charge, bool percentage)
        {
            RateId = rateId;
            PackageSizeId = packageSizeId; 
            Charge = charge;
            ChargePercentage = percentage;
        }
        public RatePackageSize Update(int rateId, int packageSizeId, decimal charge, bool percentage)
        {
            RateId = rateId;
            PackageSizeId = packageSizeId;
            Charge = charge;
            ChargePercentage = percentage;

            return this;
        }
    }
}
