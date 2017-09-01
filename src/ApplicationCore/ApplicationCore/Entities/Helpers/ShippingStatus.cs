﻿using ApplicationCore.SeedWork;
using ApplicationCore.Execeptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.ClientAgregate.ShipmentAgregate
{
    public class ShippingStatus
         : Enumeration
    {
        public static ShippingStatus PendingPickUp = new ShippingStatus(1, "Pending PickUp");
        public static ShippingStatus Pickup = new ShippingStatus(2, nameof(Pickup).ToLowerInvariant());
        public static ShippingStatus DeliveryInProcess = new ShippingStatus(3,"Delivery in Process");
        public static ShippingStatus Delivered = new ShippingStatus(4, "Delivered");
        public static ShippingStatus Cancelled = new ShippingStatus(5, "Cancelled");

        protected ShippingStatus()
        {
        }

        public ShippingStatus(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<ShippingStatus> List()
        {
            return new[] { PendingPickUp, Pickup, DeliveryInProcess, Delivered, Cancelled };
        }

        public static ShippingStatus FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new DriveDropException($"Possible values for ShippingStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static ShippingStatus From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new DriveDropException($"Possible values for ShippingStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}

