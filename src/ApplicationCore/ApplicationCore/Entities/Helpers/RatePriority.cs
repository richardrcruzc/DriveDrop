using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;
using ApplicationCore.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities.Helpers
{
    public class RatePriority : Entity
    {
        public int PriorityTypeId { get; private set; }
        public PriorityType PriorityType { get; private set; } 
        
        public decimal Charge { get; private set; }
        public bool ChargePercentage { get; private set; }

        public int RateId { get; private set; }
        public Rate Rate { get; private set; }

        public RatePriority() { }

        public RatePriority(int priorityTypeId, decimal charge, bool percentage)
        {
            
            PriorityTypeId= priorityTypeId;  
            Charge = charge;
            ChargePercentage = percentage;
        }
        public RatePriority Update(int rateId, int priorityTypeId,  decimal charge, bool percentage)
        {
            RateId = rateId;
            PriorityTypeId = priorityTypeId;
            Charge = charge;
            ChargePercentage = percentage;

            return this;
        }
    }
}
