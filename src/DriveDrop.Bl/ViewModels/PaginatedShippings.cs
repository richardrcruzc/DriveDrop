using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Bl.ViewModels
{
    public class PaginatedShippings
    {
        public  PaginatedShippings()
        {
            ShippingStatusList = new List<SelectListItem>();
            ListOne = new List<SelectListItem>();
            ListTwo = new List<SelectListItem>();
            ListThree = new List<SelectListItem>();
            Data = new List<ShipmentModel>();
        }
            public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public long Count { get; set; }
        public List<ShipmentModel> Data { get; set; }

        public PaginationInfo PaginationInfo { get; set; }

        public int One { get; set; }
        public int Two { get; set; }
        public int Three { get; set; }

        public double PickupDistance { get; set; }
        public double DeliverDistance { get; set; }
        public int CustomerId { get; set; }

        public IEnumerable<SelectListItem> ShippingStatusList { get; set; }
        public IEnumerable<SelectListItem> ListOne { get; set; }
        public IEnumerable<SelectListItem> ListTwo { get; set; }
        public IEnumerable<SelectListItem> ListThree { get; set; }
    }
    
}
