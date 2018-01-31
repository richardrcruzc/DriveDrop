using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Bl.ViewModels
{
    public class ShippingIndex
    {

        public IEnumerable<ShipmentModel> ShippingList { get; set; }
        public IEnumerable<SelectListItem> ShippingStatus { get; set; }
        public IEnumerable<SelectListItem> PriorityType { get; set; }

        public int? ShippingStatusFilterApplied { get; set; }
        public int? PriorityTypeFilterApplied { get; set; }
        public string IdentityCode { get; set; }

        public PaginationInfo PaginationInfo { get; set; }


    }
}
