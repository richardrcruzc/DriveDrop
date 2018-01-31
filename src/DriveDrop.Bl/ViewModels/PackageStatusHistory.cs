using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Bl.ViewModels
{
    public class PackageStatusHistoryModel
    {
        public PackageStatusHistoryModel() { }
        public DateTime StatusDate { get;  set; }
        public int StatusId { get; private set; }
        public ShippingStatusModel ShippingStatus { get;  set; }


        public ShipmentModel Shipment { get;  set; }
        public int ShipmentId { get;  set; }

        public int DriverId { get;  set; }

    }
}
