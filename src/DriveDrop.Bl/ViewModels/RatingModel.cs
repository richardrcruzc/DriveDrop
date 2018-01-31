using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Bl.ViewModels
{
    public class RatingModel
    {
        public string UserName { get; set; }
        public int Id { get; set; }
        public ShipmentModel Shipping { get;  set; }
        public CustomerModel Sender { get;  set; }
        public CustomerModel Driver { get;  set; }
        public string Reviewed { get;  set; }
        [MinLength(5)]
        [MaxLength(1024)]
        public string Comment { get;  set; }
        public DateTime DateCreated { get;  set; }
        public bool Published { get;  set; }
        public List<ReviewDetailModel> Details { get;  set; }

    }
    public class ReviewDetailModel
    {
        public int Id { get; set; }
       // public RatingModel Review { get;  set; }
        public ReviewQuestionModel ReviewQuestion { get;  set; }
        public int Values { get;  set; }
    }

    public class ReviewQuestionModel
    {
        public int Id { get; set; }
        public string Group { get;  set; }
        public string Description { get;  set; }
    }
}
