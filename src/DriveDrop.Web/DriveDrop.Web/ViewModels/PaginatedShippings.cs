using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Web.ViewModels
{
    public class PaginatedShippings
    {
        public  PaginatedShippings()
        {
            ShippingStatusList = new List<SelectListItem>();
            Data = new List<Shipment>();
        }
            public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }
        public List<Shipment> Data { get; set; }


        public IEnumerable<SelectListItem> ShippingStatusList { get; set; }
    }
    
}
