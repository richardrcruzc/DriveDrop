
using DriveDrop.Core.Models.Commons;
using System;
using System.Collections.Generic;

namespace DriveDrop.Core.Models.Drivers
{ 
    public class NewDriver
    {
        public NewDriver()
        {
        CustomerTypeList = new List<Generic>();
        TransportTypeList = new List<Generic>();
        CustomerStatusList = new List<Generic>();
        PriorityTypeList = new List<Generic>();
        PackageSizeList = new List<Generic>();

        }

        public IEnumerable<Generic> CustomerTypeList { get; set; }
        public IEnumerable<Generic> TransportTypeList { get; set; }
        public IEnumerable<Generic> CustomerStatusList { get; set; }
        public IEnumerable<Generic> PriorityTypeList { get; set; }
        public IEnumerable<Generic> PackageSizeList { get; set; }



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
        public Generic TransportType { get; set; }
        public int CustomerStatusId { get; set; }
        public Generic CustomerStatus { get; set; }
        public int MaxPackage { get; set; }
        public int PickupRadius { get; set; }
        public int DeliverRadius { get; set; }


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


    }
}
