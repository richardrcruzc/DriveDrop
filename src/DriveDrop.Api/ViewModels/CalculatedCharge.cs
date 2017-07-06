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

        public Decimal TaxRate { get; set; }
        public Decimal TaxAmount { get; set; }

        public Decimal AmountToCharge { get;  set; } 
    }
}
