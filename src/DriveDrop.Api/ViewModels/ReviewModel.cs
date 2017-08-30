

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
        public List<ReviewDetailModel> Details { get; set; }
    }




    public class ReviewDetailModel
    {
        public Review Review { get;   set; }
        public ReviewQuestionModel ReviewQuestion { get;   set; }
        public int Values { get;   set; }

        
    }

    public class ReviewQuestionModel 
    {

        public int Id { get; set; }
        public string Group { get;   set; }
        public string Description { get;   set; }

        
    }

}
