using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD.Mobile.Models.Drivers
{
    public class DriverEntry
    {
        public DriverEntry()
        {
            //CustomerTypeList = new Dictionary<int, String>();
            //TransportTypeList = new Dictionary<int, String>();
            //CustomerStatusList = new Dictionary<int, String>();
            //PriorityTypeList = new Dictionary<int, String>();
            //PackageSizeList = new Dictionary<int, String>();

        }

        //public Dictionary<int, String> CustomerTypeList { get; set; }
        //public Dictionary<int, String> TransportTypeList { get; set; }
        //public Dictionary<int, String> CustomerStatusList { get; set; }
        //public Dictionary<int, String> PriorityTypeList { get; set; }
        //public Dictionary<int, String> PackageSizeList { get; set; }


         

        public string Personalfiles { get; set; }
       

        public string Licensefiles { get; set; }
        

        public string Vehiclefiles { get; set; }
        

        public string Insurancefiles { get; set; }



        public string DriverLincensePictureUri { get; set; }
        public string PersonalPhotoUri { get; set; }
        public string VehiclePhotoUri { get; set; }
        public string InsurancePhotoUri { get; set; }


        public int CustomerId { get; set; }


        public string FilePath { get; set; }
         public string VehicleMake { get; set; }
         public string VehicleModel { get; set; }
         public string VehicleColor { get; set; }
          public string VehicleYear { get; set; }

      public string UserEmail { get; set; }
     public string Password { get; set; }

         public string ConfirmPassword { get; set; }


          public string PrimaryPhone { get; set; }
     public string Phone { get; set; }
        public string IdentityGuid { get; set; }
        public string UserGuid { get; set; }


           public string Email { get; set; }
          public string LastName { get; set; }
           public string FirstName { get; set; }
        public int TransportTypeId { get; set; }
      //  public TransportType TransportType { get; set; }
        public int CustomerStatusId { get; set; }
      //  public CustomerStatus CustomerStatus { get; set; }
         public int MaxPackage { get; set; }
         public int? PickupRadius { get; set; }
          public int? DeliverRadius { get; set; }

         public String DeliveryStreet { get; set; }
         public String DeliveryCity { get; set; }
        public String DeliveryState { get; set; }
        public String DeliveryCountry { get; set; }
           public String DeliveryZipCode { get; set; }


    }
}
