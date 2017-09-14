using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD.Mobile.Models.Senders
{
   public  class SenderEntry
    {
        public SenderEntry()
        { }
        //public IFormFile ImgeFoto { get; set; }

        public string UserEmail { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Phone { get; set; }
        public string PrimaryPhone { get; set; }

        public int CustomerTypeId { get; set; }
        public string PersonalPhotoUri { get; set; }
        public string IdentityGuid { get; set; }
        public string UserGuid { get; set; }


        
        public String PickupStreet { get; set; }
        
        public String PickupCity { get; set; }
        public String PickupState { get; set; }
        public String PickupCountry { get; set; }
      
        public String PickupZipCode { get; set; }

        public Double PickupLatitude { get; set; }
        public Double PickupLongitude { get; set; }
    }
}
