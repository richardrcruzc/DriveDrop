

using System;
using System.Collections.Generic;

namespace DriveDrop.Bl.ViewModels
{
//    public class ReviewModel
//    {
//        public  ReviewModel()
//        {
//            Details = new List<ReviewDetail>();
//}
//        public int Id { get; set; }
//        public int ShippingId { get; set; }
//        public int SenderId { get; set; }
//        public int DriverId { get; set; }
//        public string Reviewed { get; set; }
//        public string Comment { get; set; }
//        public bool Published { get; set; }
//        public DateTime DateCreated { get; set; }
        
//        public List<ReviewDetail> Details { get; set; }
//    }


    public class ReviewModel
    {

        public int ShippingId { get; set; }
        public int SenderId { get; set; }
        public int DriverId { get; set; }
        public string Reviewed { get; set; }
        public string Comment { get; set; }
        public bool Published { get; set; }
        public List<ReviewDetailModel> Details { get; set; }
    }


}
