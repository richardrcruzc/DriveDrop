using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Bl.ViewModels
{
    public class HomeQuote
    {
        public   HomeQuote()
        {
            PriorityTypeList = new List<SelectListItem>();
            PackageSizeList = new List<SelectListItem>();
            TransportTypeList = new List<SelectListItem>();
        }

        public IEnumerable<SelectListItem> TransportTypeList { get; set; }
        public IEnumerable<SelectListItem> PackageSizeList { get; set; }
        public IEnumerable<SelectListItem> PriorityTypeList { get; set; }

        public int TransportTypeId { get; set; }
        public int PriorityTypeId { get; set; }
        public int PackageSizeId { get; set; }

        public Decimal Amount { get; set; }
    }
}
