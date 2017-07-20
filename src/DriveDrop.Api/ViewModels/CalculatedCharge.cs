using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Api.ViewModels
{
    public class CalculatedCharge
    {
        
        public Decimal Distance { get;  set; }
        public Decimal PriorityAmount { get; set; }
        public Decimal DistanceAmount { get; set; }
        public Decimal WeightAmount { get; set; }
        public Decimal TransportTypeAmount { get; set; }
        public Decimal AmountPerSize { get; set; }

        public Decimal TaxRate { get; set; }
        public Decimal TaxAmount { get; set; }

        public Decimal Discount { get; set; }


        public Decimal AmountToCharge { get;  set; }

        public string StrSubTotal
        {
            get
            {
                return string.Format("{0:0.##}", AmountToCharge);
            }
        }


        public string StrTaxAmount
        {
            get
            {
                return   string.Format("{0:0.##}", TaxAmount);
            }
        }

        public string StrDiscount
        {
            get
            {
                return string.Format("{0:0.##}", Discount);
            }
        }
        public string StrPriorityAmount
        {
            get
            {
                return string.Format("{0:0.##}", PriorityAmount);
            }
        }
        public string StrDistanceAmount
        {
            get
            {
                return string.Format("{0:0.##}", DistanceAmount);
            }
        }
        public string StrWeightAmount
        {
            get
            {
                return string.Format("{0:0.##}", WeightAmount);
            }
        }
        public string StrTransportTypeAmount
        {
            get
            {
                return string.Format("{0:0.##}", TransportTypeAmount);
            }
        }
        public string StrAmountToCharge
        {
            get
            {
                return string.Format("US${0:0.##}", AmountToCharge+ TaxAmount- Discount );
            }
        }



    }
}
