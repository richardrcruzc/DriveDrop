using GoDriveDrop.Core.Models.Commons;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoDriveDrop.Core.Models.Shippments
{
    public class ShipmentModel
    {
        public ShipmentModel()
        {
         
        }

        public Decimal ExtraCharge { get; set; }
        public string ExtraChargeNote { get; set; }
        public virtual ICollection<GenericModel> PackageStatusHistories { get; set; }

        public List<GenericModel> ShippingStatusList { get; set; }

        public int Id { get; set; }
        public string IdentityCode { get; set; }
        public string SecurityCode { get; set; }

        public DateTime ShippingPickupDate { get; set; }

        public DateTime ShippingCreateDate { get; set; }
        public DateTime ShippingUpdateDate { get; set; }

        public int SenderId { get; set; }
        public int? DriverId { get; set; }

        public virtual CustomerModel Sender { get; set; }
        public virtual CustomerModel Driver { get; set; }

        public AddressModel PickupAddress { get; set; }
        public AddressModel DeliveryAddress { get; set; }
        //public Address BillingAddress { get;  set; }

        // public Customer Sender { get;  set; }
        //public int SenderId { get;  set; }

        //public Customer Driver { get;  set; }
        //public int DriverId { get;  set; }         

        public GenericModel ShippingStatus { get; set; }
        public int ShippingStatusId { get; set; }

        public GenericModel PriorityType { get; set; }
        public int PriorityTypeId { get; set; }
        public int PriorityTypeLevel { get; set; }
        public Decimal Distance { get; set; }

        public int TransportTypeId { get; set; }
        public GenericModel TransportType { get; set; }

        public Decimal ChargeAmount { get; set; }
        public Decimal AmountPay { get; set; }
        public Decimal Amount { get; set; }
        public Decimal Tax { get; set; }
        public Decimal Discount { get; set; }
        public string PromoCode { get; set; }

        public string PickupPictureUri { get; set; }
        public string DeliveredPictureUri { get; set; }
        public GenericModel PackageSize { get; set; }

        public Double Latitude { get; set; }
        public Double Longitude { get; set; }

        public string Note { get; set; }

        public List<ReviewModel> Reviews { get; set; }




        public double? PickupRadius { get; set; }
        public double? DeliverRadius { get; set; }


        public string DropPictureUri { get; set; }
        public string DropComment { get; set; }
        public DateTime Dropby { get; set; }


    }
}
