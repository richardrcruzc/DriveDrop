using ApplicationCore.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApplicationCore.Entities.Helpers
{
   public class Rate: Entity
    { 
        public decimal OverHead { get; private set; }
        public PackageSize PackageSize { get; private set; }
         
        public List<RatePriority> RatePriorities { get; private set; }
        public Rate AddPriority(decimal charge, int priotityId  )
        {
            if (charge == 0  && priotityId ==0)
                return this;

            var rp = new RatePriority( priotityId, charge, false);

           var a = RatePriorities.Where(r => r.PriorityTypeId == priotityId).FirstOrDefault();
            if (a == null)
                RatePriorities.Add(rp);
            else 
                a.Update(this.Id, priotityId, charge, false); 

            return this;
        }
        //public Rate RemovePriority(int detailId)
        //{
        //    var rd = RatePriorities.Where(x => x.Id == detailId).FirstOrDefault();
        //    if (rd != null)
        //        RatePriorities.Remove(rd);

        //    return this;
        //} 

        public Rate()
        { 
            RatePriorities = new List<RatePriority>(); 
        }
        public Rate(decimal overHead, PackageSize packageSize ):this()
        {
            OverHead = overHead;
            PackageSize = packageSize;
        }

        public Rate Update(decimal overHead )
        {
            OverHead = overHead;
            
            return this;
        }

    }
}

