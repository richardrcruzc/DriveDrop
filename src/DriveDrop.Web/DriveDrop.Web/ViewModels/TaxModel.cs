using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Web.ViewModels
{
    public class TaxModel
    {
        public int Id { get; set; }
        public string State { get; set; }
        public string County { get; set; }
        public string City { get; set; }
        public decimal Rate { get; set; }
        public bool RateDefault { get; set; }
    }
}
