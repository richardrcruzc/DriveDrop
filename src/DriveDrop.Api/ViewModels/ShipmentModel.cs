﻿using ApplicationCore.Entities.ClientAgregate;
using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Api.ViewModels
{

    public class ShipmentViewModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }
        public List<ShipmentModel> Data { get; set; }
    }


    public class ShipmentModel
    {

        public bool NeedaVanOrPickup { get; set; }
        public int Id { get; set; }
        public string IdentityCode { get; set; }
        public string SecurityCode { get; set; }

        public DateTime ShippingCreateDate { get; set; }
        public DateTime ShippingUpdateDate { get; set; }

        public int SenderId { get; set; }
        public int? DriverId { get; set; }

        public virtual Customer Sender { get; set; }
        public virtual Customer Driver { get; set; }


        public Address PickupAddress { get; set; }
        public Address DeliveryAddress { get; set; }
        //public Address BillingAddress { get;  set; }

        // public Customer Sender { get;  set; }
        //public int SenderId { get;  set; }

        //public Customer Driver { get;  set; }
        //public int DriverId { get;  set; }         

        public ShippingStatus ShippingStatus { get; set; }
        public int ShippingStatusId { get; set; }

        public PriorityType PriorityType { get; set; }
        public int PriorityTypeId { get; set; }
        public int PriorityTypeLevel { get; set; }


        public int TransportTypeId { get; set; }
        public TransportType TransportType { get; set; }


        public Decimal Amount { get; set; }
        public Decimal Tax { get; set; }
        public Decimal Discount { get; set; }
        public string PromoCode { get; set; }

        public string PickupPictureUri { get; set; }
        public string DeliveredPictureUri { get; set; }


        public string Note { get; set; } 

    }
}
