//using ApplicationCore.SeedWork;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace ApplicationCore.Entities.Helpers
//{
//    public class RateTranportType : Entity
//    {
       
//        public int TranportTypeId { get; private set; } 

//        public decimal Charge { get; private set; }
//        public bool ChargePercentage { get; private set; }

//        public int RateId { get; private set; }
//        public Rate Rate { get; private set; }

//        public RateTranportType() { }

//        public RateTranportType(int rateId, int tranportTypeId, decimal charge, bool percentage)
//        {
//            RateId = rateId;
//            TranportTypeId = tranportTypeId; 
//            Charge = charge;
//            ChargePercentage = percentage;
//        }
//        public RateTranportType Update(int rateId, int tranportTypeId, decimal charge, bool percentage)
//        {
//            RateId = rateId;
//            TranportTypeId = tranportTypeId;
//            Charge = charge;
//            ChargePercentage = percentage;

//            return this;
//        }
//    }
//}
