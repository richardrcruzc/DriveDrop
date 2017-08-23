using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Web.ViewModels
{
    
    public class Customer
    {
        public string UserName { get; set; }
        public int Id { get;  set; }
        public string IdentityGuid { get;  set; }
        public string UserGuid { get;  set; }
        public string PrimaryPhone { get;   set; }
        public string Email { get;  set; }
        public string Phone { get;  set; }
        public string LastName { get;  set; }
        public string FirstName { get;  set; }
        public CustomerType CustomerType { get;  set; }
       // public string CustomerType { get;  set; }
        public int CustomerTypeId { get;  set; }
        public int? TransportTypeId { get;  set; }
        public TransportType TransportType { get;  set; }
        //public string TransportType { get;  set; }
        public int CustomerStatusId { get;  set; }
        public CustomerStatus CustomerStatus { get;  set; }
        //public string CustomerStatus { get;  set; }
        public int? MaxPackage { get;  set; }
        public int? PickupRadius { get;  set; }
        public int? DeliverRadius { get;  set; }
        public string PersonalPhotoUri { get;  set; }
        public string VehicleInfo { get; set; }

        public decimal Commission { get;  set; }

        public Address DefaultAddress { get;  set; }

        //  public List<Shipment> Driver { get;  set; }
       public virtual ICollection<Shipment> ShipmentDrivers { get;  set; }
        public virtual ICollection<Shipment> ShipmentSenders { get;  set; }

        public List<AddressModel> Addresses { get;  set; }
        public int DefaultAddressId { get;   set; }

        public string DriverLincensePictureUri { get; set; } 
        public string VehiclePhotoUri { get; set; }
        public string InsurancePhotoUri { get; set; }


        public string VehicleMake { get;  set; }
        public string VehicleModel { get;  set; }
        public string VehicleColor { get;  set; }
        public string VehicleYear { get;  set; }


        public string FullName
        {
            get
            {
                return LastName + ", " + FirstName;
            }
        }
         
         
    }
}
