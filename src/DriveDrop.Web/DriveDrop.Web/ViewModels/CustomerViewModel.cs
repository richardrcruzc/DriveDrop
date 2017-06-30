using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Web.ViewModels
{
    public class CustomerViewModel
    {

        public int Id { get; set; }
        public string IdentityGuid { get; set; }
        public string UserGuid { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string CustomerType { get; set; }
        public int CustomerTypeId { get; set; }
        public int? TransportTypeId { get; set; }
        public string TransportType { get; set; }
        public int CustomerStatusId { get; set; }
        public string CustomerStatus { get; set; }
        public int? MaxPackage { get; set; }
        public int? PickupRadius { get; set; }
        public int? DeliverRadius { get; set; }

        public decimal Commission { get; set; }

        //public Address DefaultAddress { get; set; }

        //public List<Shipment> Driver { get; set; }
        public virtual ICollection<Shipment> ShipmentDrivers { get; set; }
        public virtual ICollection<Shipment> ShipmentSenders { get; set; }

        public string DriverLincensePictureUri { get; set; }

        public string FullName
        {
            get
            {
                return LastName + ", " + FirstName;
            }
        }


    }
}
