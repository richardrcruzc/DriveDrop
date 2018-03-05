using System;
using System.Collections.Generic;
using System.Text;

namespace GoDriveDrop.Core.Models
{
    public class NewDriverModel
    {
        public NewDriverModel()
        {
            CustomerTypeList = new List<GenericModel>();
            TransportTypeList = new List<GenericModel>();
            CustomerStatusList = new List<GenericModel>();
            PriorityTypeList = new List<GenericModel>();
            PackageSizeList = new List<GenericModel>();

        }

        public IEnumerable<GenericModel> CustomerTypeList { get; set; }
        public IEnumerable<GenericModel> TransportTypeList { get; set; }
        public IEnumerable<GenericModel> CustomerStatusList { get; set; }
        public IEnumerable<GenericModel> PriorityTypeList { get; set; }
        public IEnumerable<GenericModel> PackageSizeList { get; set; }

        public bool FromXamarin { get; set; } 

        //[Required]
        //public List<IFormFile> Personalfiles { get; set; }

        //[Required]
        //public List<IFormFile> Licensefiles { get; set; }

        //[Required]
        //public List<IFormFile> Vehiclefiles { get; set; }

        //[Required]
        //public List<IFormFile> Insurancefiles { get; set; }

        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string PrimaryPhone { get; set; }
        public string Phone { get; set; }

        public int TransportTypeId { get; set; }
        public GenericModel TransportType { get; set; }
        public int CustomerStatusId { get; set; }
        public GenericModel CustomerStatus { get; set; }
        public string MaxPackage { get; set; }
        public string PickupRadius { get; set; }
        public string DeliverRadius { get; set; }


        public string VehicleMake { get; set; }
        public string VehicleModel { get; set; }
        public string VehicleColor { get; set; }
        public string VehicleYear { get; set; }


        public string UserEmail { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; } 


        public string DriverLincensePictureUri { get; set; }
        public string PersonalPhotoUri { get; set; }
        public string VehiclePhotoUri { get; set; }
        public string InsurancePhotoUri { get; set; }


        public int CustomerId { get; set; }


        public string FilePath { get; set; } 


        public string IdentityGuid { get; set; }
        public string UserGuid { get; set; }
        public string Email { get; set; } 


        public Double DeliveryLatitude { get; set; }
        public Double DeliveryLongitude { get; set; }

        public String DeliveryStreet { get; set; }
        public String DeliveryCity { get; set; }
        public String DeliveryState { get; set; }
        public String DeliveryCountry { get; set; }
        public String DeliveryZipCode { get; set; }


        public String ErrorMsg { get; set; }

    }
}
