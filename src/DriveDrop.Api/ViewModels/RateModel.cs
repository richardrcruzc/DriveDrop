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
        public DateTime StartDate { get;  set; }
        public DateTime EndDate { get;  set; }
        public decimal FixChargePerShipping { get; set; }
        public bool FixChargePercentage { get;  set; }
        public decimal ChargePerItem { get;  set; }
        public decimal Tax { get;  set; }
        public bool Active { get;  set; }

        public List<RateDetailModel> RateDetails { get;  set; }

        public List<RatePriorityModel> RatePriorities { get; set; }
        public List<RatePackageSizeModel> PackageSizes { get; set; }


        public IEnumerable<SelectListItem> CustomerTypeList { get; set; }
        public IEnumerable<SelectListItem> TransportTypeList { get; set; }
        public IEnumerable<SelectListItem> CustomerStatusList { get; set; }
        public IEnumerable<SelectListItem> PriorityTypeList { get; set; }



    }

    public class RateDetailModel
    {
        public int Id { get; set; }
        public string WeightOrDistance { get;  set; }
        public string MileOrLbs { get;  set; }
        public decimal From { get;  set; }
        public decimal To { get;  set; }
        public decimal Charge { get;  set; }

        public int RateId { get;  set; }          
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

    public class RatePriorityModel
    {
        public int Id { get; set; }
        public int RateId { get; set; }
        public int PriorityId { get;   set; }
        public decimal Charge { get;   set; }
        public bool ChargePercentage { get; set; }
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
