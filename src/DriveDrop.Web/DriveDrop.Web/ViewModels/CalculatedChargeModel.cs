using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Web.ViewModels
{
    public class CalculatedChargeModel
    {


        public Decimal ExtraCharge { get; set; }
        public string ExtraChargeDetail { get; set; }


        public double Distance { get; set; }
        public Decimal PriorityAmount { get; set; }
        public string PriorityAmountDetail { get; set; }
        public Decimal DistanceAmount { get; set; }
        public string DistanceAmountDetails { get; set; }
        public Decimal WeightAmount { get; set; }
        public string WeightAmountDetails { get; set; }
        public Decimal TransportTypeAmount { get; set; }
        public string TransportTypeAmountDetails { get; set; }
        public Decimal AmountPerSize { get; set; }
        public string AmountPerSizeDetails { get; set; }

        public Decimal TaxRate { get; set; }
        public Decimal TaxAmount { get; set; }
        public string TaxAmountDetails { get; set; }

        public Decimal Discount { get; set; }


        public Decimal AmountToCharge { get;  set; }


        public bool Valid { get; set; }

        
        public string AmountSizePriority
        {
            get
            {
                return string.Format("{0:0.00}", PriorityAmount + AmountPerSize);
            }
        }
        
        public string DistanceWeight
        {
            get
            {
                return string.Format("{0:0.00}", DistanceAmount + WeightAmount);
            }
        }

        public string DistanceWeightDetails
        {
            get
            {
                return string.Format("{0} + {1}", DistanceAmountDetails, WeightAmountDetails);
            }
        }




        public string StrExtraCharge
        {
            get
            {
                return string.Format("{0:0.00}", ExtraCharge);
            }
        }


        public string StrAmountPerSize
        {
            get
            {
                return string.Format("{0:0.00}", AmountPerSize);
            }
        }

        public string StrSubTotal
        {
            get
            {
                return string.Format("{0:0.00}", AmountToCharge);
            }
        }


        public string StrTaxAmount
        {
            get
            {
                return string.Format("{0:0.00}", TaxAmount);
            }
        }

        public string StrDiscount
        {
            get
            {
                return string.Format("{0:0.00}", Discount);
            }
        }
        public string StrPriorityAmount
        {
            get
            {
                return string.Format("{0:0.00}", PriorityAmount);
            }
        }
        public string StrDistanceAmount
        {
            get
            {
                return string.Format("{0:0.00}", DistanceAmount);
            }
        }
        public string StrWeightAmount
        {
            get
            {
                return string.Format("{0:0.00}", WeightAmount);
            }
        }
        public string StrTransportTypeAmount
        {
            get
            {
                return string.Format("{0:0.00}", TransportTypeAmount);
            }
        }
        public string StrAmountToCharge
        {
            get
            {
                return string.Format("${0:0.00}", AmountToCharge + TaxAmount - Discount);
            }
        }




    }
}
