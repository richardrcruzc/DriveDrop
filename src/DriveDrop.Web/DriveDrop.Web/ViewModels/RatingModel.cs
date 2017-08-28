using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Web.ViewModels
{
    public class RatingModel
    {
        public int Id { get; set; }
        public Shipment Shipping { get;  set; }
        public Customer Sender { get;  set; }
        public Customer Driver { get;  set; }
        public string Reviewed { get;  set; }
        [MinLength(5)]
        [MaxLength(1024)]
        public string Comment { get;  set; }
        public DateTime DateCreated { get;  set; }
        public bool Published { get;  set; }
        public List<ReviewDetail> Details { get;  set; }

    }
    public class ReviewDetail
    {
        public int Id { get; set; }
       // public RatingModel Review { get;  set; }
        public ReviewQuestion ReviewQuestion { get;  set; }
        public int Values { get;  set; }
    }

    public class ReviewQuestion
    {
        public int Id { get; set; }
        public string Group { get;  set; }
        public string Description { get;  set; }
    }
}
