using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Web.ViewModels
{
    public class RateModel
    {
        public int Id { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Rate valid from")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Required]
        [Display(Name = "To")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Charge per Transaction")]
        [Required]
        public decimal FixChargePerShipping { get; set; }
        [Display(Name = "Charge percentage?")]
        public bool FixChargePercentage { get; set; }
        [Column(TypeName = "money")]
        [Required]
        [Display(Name = "Charge per Item")]
        public decimal ChargePerItem { get; set; }
        [Required]
        [Display(Name = "Tax %")]
        public decimal Tax { get; set; }
        public bool Active { get; set; }
         

       
        public List<RateDetailModel> WeightRateDetails { get; set; }
        public List<RateDetailModel> MileRateDetails { get; set; }

        public List<RateDetailModel> RateDetails { get; set; }
        public List<RatePriorityModel> RatePriorities { get; set; }
        public List<RatePackageSizeModel> PackageSizes { get; set; }


           public List<RateTranportTypeModel> RateTranportTypeDetails { get; set; }

        public List<SelectListItem> PriorityTypeList { get; set; }
        public List<SelectListItem> PackageSizeList { get; set; }


        // public RateModel() { RateDetails = new List<RateDetailModel>(); }
        public RateModel()
        {
            RateTranportTypeDetails = new List<RateTranportTypeModel>();
             MileRateDetails = new List<RateDetailModel>();
            WeightRateDetails = new List<RateDetailModel>();
          


            PriorityTypeList = new List<SelectListItem>();

            PackageSizeList = new List<SelectListItem>();

            RatePriorities = new List<RatePriorityModel>();
            PackageSizes = new List<RatePackageSizeModel>();


    }

    //public RateModel(DateTime startDate, DateTime endDate, decimal markUp, decimal chargePerItem, decimal tax)
    //{

    //    StartDate = startDate;
    //    EndDate = endDate;
    //    MarkUp = markUp;
    //    ChargePerItem = chargePerItem;
    //    Tax = tax;
    //}

    //public RateModel AddDetails(RateDetailModel detail)
    //{
    //    foreach (var r in RateDetails.ToList())
    //    {
    //        if (r.From == detail.From
    //            && r.Charge == detail.Charge
    //            && r.MileOrLbs == detail.MileOrLbs
    //            && r.To == detail.To
    //            && r.WeightOrDistance == detail.WeightOrDistance)
    //            continue;
    //        //RateDetails.Add(detail);
    //    }

    //    /*

    //    && r.From==detail.From 
    //    && r.Charge = detail.Charge 
    //    && r.MileOrLbs == detail.MileOrLbs
    //     */
    //    return this;
    //}
}

    public class RateTranportTypeModel
    {
        public int Id { get; set; }
        public int RateId { get; set; }
        public int TranportTypeId { get;  set; }

        public decimal Charge { get;  set; }
        public bool ChargePercentage { get;  set; }
        public string Name { get;  set; }

    }
        public class RatePriorityModel
    {
        public int Id { get; set; }
        public int RateId { get; set; }
        public int PriorityId { get;  set; }

        public decimal Charge { get;  set; }
        public bool ChargePercentage { get;  set; }
        public string Name { get;  set; }

    }
        public class RatePackageSizeModel
    {
        public int Id { get; set; }
        public int RateId { get; set; }
        public int PackageSizeId { get;  set; } 

        public decimal Charge { get;  set; }
        public bool ChargePercentage { get;  set; }
        public string Name { get;  set; }

    }
}
