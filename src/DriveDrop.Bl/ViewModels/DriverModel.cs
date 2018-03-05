using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Bl.ViewModels
{
    public class GenericModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
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
    public class DriverModel
    {
        public DriverModel()
        {
            CustomerTypeList = new List<SelectListItem>();
            TransportTypeList = new List<SelectListItem>();
            CustomerStatusList = new List<SelectListItem>();
            PriorityTypeList = new List<SelectListItem>();
            PackageSizeList = new List<SelectListItem>();

        }

        public IEnumerable<SelectListItem> CustomerTypeList { get; set; }
        public IEnumerable<SelectListItem> TransportTypeList { get; set; }
        public IEnumerable<SelectListItem> CustomerStatusList { get; set; }
        public IEnumerable<SelectListItem> PriorityTypeList { get; set; }
        public IEnumerable<SelectListItem> PackageSizeList { get; set; }



        [Required]
        public List<IFormFile> Personalfiles { get; set; }

        [Required]
        public List<IFormFile> Licensefiles { get; set; }

        [Required]
        public List<IFormFile> Vehiclefiles { get; set; }

        [Required]
        public List<IFormFile> Insurancefiles { get; set; }



        public string DriverLincensePictureUri { get; set; }
        public string PersonalPhotoUri { get; set; }
        public string VehiclePhotoUri { get; set; }
        public string InsurancePhotoUri { get; set; }

         
        public int CustomerId { get; set; }
        public bool FromXamarin { get; set; }

        public string FilePath { get; set; }
        [Required(ErrorMessage = "Your must provide a  vehicle make")]
        public string VehicleMake { get; set; }
        [Required(ErrorMessage = "Your must provide a vehicle model")]
        public string VehicleModel { get; set; }
        [Required(ErrorMessage = "Your must provide a vehicle color")]
        public string VehicleColor { get; set; }
        [Required(ErrorMessage = "Your must provide a vehicle year")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "vehicle year must be a number")]
        [Range(1990, 9999)]
        public string VehicleYear { get; set; }


        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        [Remote(action:"ValidateUserName", controller:"Common")]
        public string UserEmail { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password", Description="Password must be 8 charater long")]
        //[Remote(action: "ValidatePassword", controller: "Common")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }


        [Required(ErrorMessage = "Your must provide a primary phone")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        [Display(Name = "Phone")]
        public string PrimaryPhone { get; set; }

        [Required(ErrorMessage = "Your must provide a phone number")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        [Display(Name = "Phone")]
        public string Phone { get; set; }
        public string IdentityGuid { get; set; }
        public string UserGuid { get; set; } 


        //[Required]
        //[EmailAddress]
        //[Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Your must provide a Last Name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Your must provide a First Name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        public int TransportTypeId { get; set; }
        public TransportTypeModel TransportType { get; set; }
        public int CustomerStatusId { get; set; }
        public CustomerStatusModel CustomerStatus { get; set; }
        [Required(ErrorMessage = "Your must provide a MaxPackage")]
        [Display(Name = "Maximum package to pickup")]
        public int MaxPackage { get; set; }
        [Display(Name = "Pick up Radius")]
        [Required(ErrorMessage = "Your must provide a Pickup Radius")]
        public int PickupRadius { get; set; }
        [Display(Name = "Deliver Radius")]
        [Required(ErrorMessage = "Your must provide a Deliver Radius")]
        public int DeliverRadius { get; set; }

        public Double DeliveryLatitude { get; set; }
        public Double DeliveryLongitude { get; set; }

        [Display(Name = "Street")]
        [Required(ErrorMessage = "Your must provide a delivery street")]
        public String DeliveryStreet { get; set; }
        [Display(Name = "City")]
        [Required(ErrorMessage = "Your must provide a delivery City")]
        public String DeliveryCity { get; set; }
        public String DeliveryState { get; set; }
        public String DeliveryCountry { get; set; }
        [Display(Name = "Postal Code")]
        [Required(ErrorMessage = "Your must provide a delivery Postal Code")]
        [DataType(DataType.PostalCode)]
        public String DeliveryZipCode { get; set; } 
         

    }

}
