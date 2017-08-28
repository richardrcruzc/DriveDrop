

using ApplicationCore.Entities.Helpers;
using System.Collections.Generic;

namespace DriveDrop.Api.ViewModels
{
    public class ReviewModel
    { 
        public int ShippingId { get;   set; }
        public int SenderId { get;   set; }
        public int DriverId { get;   set; }
        public string Reviewed { get;  set; }
        public string Comment { get; set; }
        public bool Published { get; set; }
        public List<ReviewDetail> Details { get; set; }
    }

}
