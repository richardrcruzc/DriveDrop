using ApplicationCore.SeedWork;
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
        public static ShippingStatus NoDriverAssigned = new ShippingStatus(1, "No Driver Assigned");
        public static ShippingStatus PendingPickUp = new ShippingStatus(2, "Pending Pick-Up");
        public static ShippingStatus Pickup = new ShippingStatus(3, "Pick-Up in Transit");
        public static ShippingStatus DeliveryInProcess = new ShippingStatus(4, "Accepted – Waiting Delivery");
        public static ShippingStatus PendingDelivery = new ShippingStatus(5, "Pending Delivery");
        public static ShippingStatus DeliveryInTransit = new ShippingStatus(6, "Delivery in Transit");
        public static ShippingStatus Delivered = new ShippingStatus(7, "Delivered");
        public static ShippingStatus Cancelled = new ShippingStatus(8, "Cancelled");

        protected ShippingStatus()
        {
        }

        public ShippingStatus(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<ShippingStatus> List()
        {
            return new[] { NoDriverAssigned, PendingPickUp, Pickup, DeliveryInProcess, PendingDelivery, DeliveryInTransit, Delivered, Cancelled };
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

