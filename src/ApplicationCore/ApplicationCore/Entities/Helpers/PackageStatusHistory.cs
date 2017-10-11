using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;
using ApplicationCore.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities.Helpers
{
  public  class PackageStatusHistory : Entity
    {
        public PackageStatusHistory() { }
        public DateTime StatusDate { get; private set; }
        public int StatusId { get; private set; }
        public ShippingStatus ShippingStatus { get; private set; }
        

        public Shipment Shipment { get; private set; }
        public int ShipmentId { get; private set; }

        public int DriverId { get; private set; }
         

        public PackageStatusHistory(int statusId, int packageId, int driverId) {
            StatusDate = DateTime.Now;
            StatusId = statusId;
            DriverId = driverId;
            ShipmentId = packageId;
        }
    }
}
