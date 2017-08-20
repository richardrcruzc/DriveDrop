using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Api.ViewModels
{
    public class RateModel
    {
        public int Id { get; set; }
        public decimal OverHead { get;  set; }
        public TypeModel PackageSize { get;  set; }

        public List<RatePriorityModel> RatePriorities { get;  set; } 

    }


    public class RatePriorityModel
    {
        public int PriorityTypeId { get; set; }
        public TypeModel PriorityType { get;  set; }

        public decimal Charge { get;  set; }
        public bool ChargePercentage { get;  set; }

        public int RateId { get;  set; } 

    }

    public class TypeModel
    {
        public string Name { get; private set; }

        public int Id { get; private set; }

    }
    public class RateDetailModel
    {
        public int Id { get; set; }
        public string WeightOrDistance { get;  set; }
        public string MileOrLbs { get;  set; }
        public decimal From { get;  set; }
        public decimal To { get;  set; }
        public decimal Charge { get;  set; }
        
    }

    public class RatePackageSizeModel
    {
        public int Id { get; set; }
        public int RateId { get; set; }
        public int PackageSizeId { get;   set; }
        public decimal Charge { get;   set; }
        public bool ChargePercentage { get;   set; }
        public string Name { get; set; }
    }


public class RateDeleteDetailModel
    {
        public int RateId { get; set; }
        public List<RateDetailModel> RateDetails { get; set; }
        public List<RatePackageSizeModel> RatePackageSizes { get; set; }
        public List<RatePriorityModel> RatePriorities { get; set; }

    }
}
