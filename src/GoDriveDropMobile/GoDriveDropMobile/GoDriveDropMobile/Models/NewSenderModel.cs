using System;
using System.Collections.Generic;
using System.Text;

namespace GoDriveDrop.Core.Models
{ 
     public class NewSenderModel
    {
        public NewSenderModel()
        {
            CustomerTypeList = new List<GenericModel>();
            TransportTypeList = new List<GenericModel>();
            CustomerStatusList = new List<GenericModel>();
            PriorityTypeList = new List<GenericModel>();

            PackageSizeList = new List<GenericModel>();
        }
        
       public bool FromXamarin { get; set; }
        public decimal Distance { get; set; }
        public IEnumerable<GenericModel> CustomerTypeList { get; set; }
        public IEnumerable<GenericModel> TransportTypeList { get; set; }
        public IEnumerable<GenericModel> CustomerStatusList { get; set; }
        public IEnumerable<GenericModel> PriorityTypeList { get; set; }

        public IEnumerable<GenericModel> PackageSizeList { get; set; }

        public string PersonalPhotoUri { get; set; }
        public int CustomerId { get; set; } 

        public string PrimaryPhone { get; set; }
        public string Phone { get; set; }
        public string FilePath { get; set; }
   
        public string UserEmail { get; set; }
 
        public string Password { get; set; } 
        public string ConfirmPassword { get; set; } 
 
   
        public string IdentityGuid { get; set; }
        public string UserGuid { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public GenericModel CustomerType { get; set; }
        
        public int CustomerTypeId { get; set; } 
         
        public String PickupStreet { get; set; }
      
        public String PickupCity { get; set; }
        public String PickupState { get; set; }
        public String PickupCountry { get; set; }
       
        public String PickupZipCode { get; set; }

        public Double PickupLatitude { get; set; }
        public Double PickupLongitude { get; set; }
    }
}
