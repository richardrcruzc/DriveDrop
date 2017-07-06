using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Api.ViewModels
{
    public class RateModel
    {
        public int Id { get; set; }
        public DateTime StartDate { get;  set; }
        public DateTime EndDate { get;  set; }
        public decimal MarkUp { get;  set; }
        public decimal ChargePerItem { get;  set; }
        public decimal Tax { get;  set; }
        public bool Active { get;  set; }

        public List<RateDetailModel> RateDetails { get;  set; }

    }

    public class RateDetailModel
    {
        public string WeightOrDistance { get;  set; }
        public string MileOrLbs { get;  set; }
        public decimal From { get;  set; }
        public decimal To { get;  set; }
        public decimal Charge { get;  set; }

        public int RateId { get;  set; } 
         
    }
}
