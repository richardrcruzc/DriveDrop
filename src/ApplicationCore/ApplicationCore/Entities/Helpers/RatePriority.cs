using ApplicationCore.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities.Helpers
{
    public class RatePriority : Entity
    {
       
        public int PriorityId { get; private set; } 

        public decimal Charge { get; private set; }
        public bool ChargePercentage { get; private set; }

        public int RateId { get; private set; }
        public Rate Rate { get; private set; }

        public RatePriority() { }

        public RatePriority(int rateId, int priorityId, decimal charge, bool percentage)
        {
            RateId = rateId;
            PriorityId = priorityId; 
            Charge = charge;
            ChargePercentage = percentage;
        }
        public RatePriority Update(int rateId, int priorityId, decimal charge, bool percentage)
        {
            RateId = rateId;
            PriorityId = priorityId;
            Charge = charge;
            ChargePercentage = percentage;

            return this;
        }
    }
}
