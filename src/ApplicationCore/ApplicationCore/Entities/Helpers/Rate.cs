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
        public Rate AddDetails(RateDetail detail)
        {
            if (detail.Charge == 0 || detail.From == 0 || detail.To == 0 || (detail.WeightOrDistance != "distance" && detail.MileOrLbs != "miles"))
                return this;

            var d = RateDetails.FirstOrDefault(r => r.From == detail.From && r.Charge == detail.Charge && r.MileOrLbs == detail.MileOrLbs);

            if(d==null)
                RateDetails.Add(detail);
            
            return this;
        }
        public Rate RemoveDetails(int detailId)
        {
            var rd = RateDetails.Where(x => x.Id == detailId).FirstOrDefault();
            if(rd!=null)
                RateDetails.Remove(rd);
 
            return this;
        }
        public List<RatePriority> RatePriorities { get; private set; }
        public Rate AddPriority(RatePriority detail)
        {
            if (detail.Charge == 0 && detail.PriorityId == 0)
                return this;

           var a = RatePriorities.Where(r => r.Charge == detail.Charge && r.ChargePercentage == detail.ChargePercentage && r.PriorityId == detail.PriorityId);
            if(a==null)
                RatePriorities.Add(detail);
            
            return this;
        }
        public Rate RemovePriority(int detailId)
        {
            var rd = RatePriorities.Where(x => x.Id == detailId).FirstOrDefault();
            if (rd != null)
                RatePriorities.Remove(rd);

            return this;
        }
        public List<RatePackageSize> PackageSizes { get; private set; }
        public Rate AddSize(RatePackageSize detail)
        {
            if (detail.Charge == 0 || detail.PackageSizeId == 0)
                return this;

            var s = PackageSizes.FirstOrDefault(r => r.Charge == detail.Charge && r.ChargePercentage == detail.ChargePercentage
                    && r.PackageSizeId == detail.PackageSizeId);
            
            if (s != null)
                return this;

             
                PackageSizes.Add(detail);
            return this;
        }
        public Rate RemoveSize(int detailId)
        {
            var rd = PackageSizes.FirstOrDefault(x => x.Id == detailId);
            if (rd != null)
                PackageSizes.Remove(rd);

            return this;
        }

        public Rate()
        {
            RateDetails = new List<RateDetail>();
            RatePriorities = new List<RatePriority>();
            PackageSizes = new List<RatePackageSize>();
        }

        public Rate(DateTime startDate, DateTime endDate, decimal markUp, decimal chargePerItem, decimal tax) : this()
        {

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
       
    }
}

