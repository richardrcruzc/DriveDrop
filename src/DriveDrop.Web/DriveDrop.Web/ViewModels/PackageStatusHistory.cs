using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Web.ViewModels
{
    public class PackageStatusHistory
    {
        public PackageStatusHistory() { }
        public DateTime StatusDate { get;  set; }
        public int StatusId { get; private set; }
        public ShippingStatus ShippingStatus { get;  set; }


        public Shipment Shipment { get;  set; }
        public int ShipmentId { get;  set; }

        public int DriverId { get;  set; }

    }
}
