
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DriveDrop.Web.ViewModels
{
    public class Shipment 
    {
        public Shipment()
        {
            var ShippingStatusList= new List<SelectListItem>();
        }

        public Decimal ExtraCharge { get; set; }
        public string ExtraChargeNote { get; set; }
        public virtual ICollection<PackageStatusHistory> PackageStatusHistories { get; set; }

        public List<SelectListItem> ShippingStatusList { get; set; }

        public int Id { get; set; }
        public string IdentityCode { get;  set; }
        public string SecurityCode { get;  set; }

        public DateTime ShippingPickupDate { get;  set; }

        public DateTime ShippingCreateDate { get;  set; }
        public DateTime ShippingUpdateDate { get;  set; }

        public  int SenderId { get;  set; }
        public int? DriverId { get;  set; }

        public virtual Customer Sender { get;  set; }
        public virtual Customer Driver { get;  set; }

        [JsonProperty("pickupAddress")]
        public Address PickupAddress { get;  set; }
        [JsonProperty("deliveryAddress")]
        public Address DeliveryAddress { get;  set; }
        //public Address BillingAddress { get;  set; }

        // public Customer Sender { get;  set; }
        //public int SenderId { get;  set; }

        //public Customer Driver { get;  set; }
        //public int DriverId { get;  set; }         

        public ShippingStatus ShippingStatus { get;  set; }
        public int ShippingStatusId { get;  set; }

        public PriorityType PriorityType { get;  set; }
        public int PriorityTypeId { get;  set; }
        public int PriorityTypeLevel { get;  set; }
        public Decimal Distance { get;   set; }

        public int TransportTypeId { get;  set; }
        public TransportType TransportType { get;  set; }

        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public Decimal ChargeAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public Decimal AmountPay { get; set; }
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public Decimal Amount { get;  set; }
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public Decimal Tax { get;  set; }
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public Decimal Discount { get;  set; }
        public string PromoCode { get;  set; }
         
        public string PickupPictureUri { get;  set; }
        public string DeliveredPictureUri { get;  set; }
        public PackageSize PackageSize { get; set; }

        public Double Latitude { get; set; }
        public Double Longitude { get; set; }

        public string Note { get;  set; }

        public List<RatingModel> Reviews { get; set; }




        public double? PickupRadius { get; set; }
        public double? DeliverRadius { get; set; }


    }
}
