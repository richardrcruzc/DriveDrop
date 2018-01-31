using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Bl.ViewModels
{
    public class UpdatePackageStatusModel
    {
        public int ShippingId { get; set; }
        public int DriverId { get; set; }
        public UpdatePackageStatusItemModel Item { get; set; }
        
    }
    public class UpdatePackageStatusItemModel
    {
        public int ShippingStatusId { get; set; } 

    }
}
