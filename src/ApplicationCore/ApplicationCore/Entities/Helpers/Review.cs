using ApplicationCore.Entities.ClientAgregate;
using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;
using ApplicationCore.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ApplicationCore.Entities.Helpers
{
    public class Review : Entity
    {
        public Shipment Shipping { get; private set; }
        public Customer Sender { get; private set; }
        public Customer Driver { get; private set; }
        public string Reviewed { get; private set; }
        public string Comment { get; private set; }
        public DateTime DateCreated { get; private set; }
        public bool Published { get; private set; } 
        public List<ReviewDetail> Details { get; private set; }


        public Review AddDetails(ReviewDetail detail)
        {
            if (Details == null)
                Details = new List<ReviewDetail>();
            
                var index = Details.FindIndex(x => x.ReviewQuestion.Id > detail.ReviewQuestion.Id);
                if (index < 0)
                    Details.Add(detail);
                else
                    Details[index] = detail;
             
            return this;
        }

        public Review()
        {
            Details = new List<ReviewDetail>();
        }
        public void Publish()
        {
            Published = true;
        }

        public Review(Shipment shipping, Customer sender, Customer driver, string reviewed,  string comment, bool published)
        {
            Shipping = shipping;
            Sender = sender;
            Driver = driver;
            Reviewed = reviewed; 
            Comment = comment;
            Published = published;
            DateCreated = DateTime.Now;
        }
    }
    
    }
