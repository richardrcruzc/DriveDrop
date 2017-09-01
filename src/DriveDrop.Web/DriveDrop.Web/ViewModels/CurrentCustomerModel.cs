using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Web.ViewModels
{
    public class CurrentCustomerModel
    {
        public bool CanBeUnImpersonate { get; set; }
        public string UserNameToImpersonate { get; set; }
        public int Id { get; set; }
        public string IdentityGuid { get; set; }
        public string UserGuid { get; set; }
        public string UserName { get; set; }
         
       // [EmailAddress]
        [Display(Name = "Verification ID")]
        [Required(ErrorMessage = "Your must provide Verification ID")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
        public string VerificationId { get; set; }
        [EmailAddress]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Your must provide Email")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Your must provide a PhoneNumber")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        [Display(Name = "Cell Phone Number")]
        public string Phone { get; set; } 
        [Required(ErrorMessage = "Your must provide a Primary Phone")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Primary Phoner")]
        [Display(Name = "Primary Phone Number")]
        public string PrimaryPhone { get; set; }
        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Your must provide Last Name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
        public string LastName { get; set; }
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "Your must provide First Name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
        public string FirstName { get; set; }
        public string CustomerType { get; set; }
        public int? CustomerTypeId { get; set; }
        public int? TransportTypeId { get; set; }
        public string TransportType { get; set; }
        public int? CustomerStatusId { get; set; }
        public string CustomerStatus { get; set; }
        public int? MaxPackage { get; set; }
        public int? PickupRadius { get; set; }
        public int? DeliverRadius { get; set; }


        public string VehicleInfo { get; set; }

        public decimal Commission { get; set; }

        public ICollection<AddressModel> Addresses { get; set; }
        public virtual ICollection<Shipment> ShipmentDrivers { get; set; }
        public virtual ICollection<Shipment> ShipmentSenders { get; set; }

        public string DriverLincensePictureUri { get; set; }

        public bool IsAdmin { get; set; }
        public bool IsValid { get; set; }

        public AddressModel DefaultAddress { get; set; }

        public string PaymentMethodId { get; set; }

        public string PersonalPhotoUri { get; set; }
        public string VehiclePhotoUri { get; set; }
        public string InsurancePhotoUri { get; set; }



        public string VehicleMake { get; set; }
        public string VehicleModel { get; set; }
        public string VehicleColor { get; set; }
        public string VehicleYear { get; set; }


        public string FullName
        {
            get
            {
                return LastName + ", " + FirstName;
            }
        }

        public bool IsImpersonated 
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.UserNameToImpersonate);
            }
        }

    }
}
