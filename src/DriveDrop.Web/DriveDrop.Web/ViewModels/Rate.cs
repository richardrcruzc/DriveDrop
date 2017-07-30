﻿using System;
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
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Required]
        public DateTime EndDate { get; set; }


        [Required]
        public decimal FixChargePerShipping { get; set; }

        public bool FixChargePercentage { get; set; }
        [Column(TypeName = "money")]
        [Required]
        public decimal ChargePerItem { get; set; }
        [Required]
        public decimal Tax { get; set; }
        public bool Active { get; set; }

        public RateDetailModel RateDetail { get; set; }

        public List<RateDetailModel> RateDetails { get; set; }
        public List<RateDetailModel> WeightRateDetails { get; set; }
        public List<RateDetailModel> MileRateDetails { get; set; }
        public List<RatePackageSizeModel> RatePackageSizeDetails { get; set; }
        public List<RatePriorityModel> RatePriorityDetails { get; set; }
        public List<RateTranportTypeModel> RateTranportTypeDetails { get; set; }


        // public RateModel() { RateDetails = new List<RateDetailModel>(); }
        public RateModel()
        {

            MileRateDetails = new List<RateDetailModel>();
            WeightRateDetails = new List<RateDetailModel>();
            RateDetail = new RateDetailModel();

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
        public int TranportTypeId { get;  set; }

        public decimal Charge { get;  set; }
        public bool ChargePercentage { get;  set; }
        public string Name { get;  set; }

    }
        public class RatePriorityModel
    { 
        public int PriorityId { get;  set; }

        public decimal Charge { get;  set; }
        public bool ChargePercentage { get;  set; }
        public string Name { get;  set; }

    }
        public class RatePackageSizeModel
    {

        public int PackageSizeId { get;  set; }

        public decimal Charge { get;  set; }
        public bool ChargePercentage { get;  set; }
        public string Name { get;  set; }

    }
}