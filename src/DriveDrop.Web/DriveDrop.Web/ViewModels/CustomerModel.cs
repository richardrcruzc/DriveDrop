
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Web.ViewModels
{
    public class SenderRegisterModel
    {
        public SenderRegisterModel()
        {
            CustomerTypeList = new List<SelectListItem>();
            TransportTypeList = new List<SelectListItem>();
            CustomerStatusList = new List<SelectListItem>();
            PriorityTypeList = new List<SelectListItem>();

            PackageSizeList = new List<SelectListItem>(); 
        }
        //[FileExtensions(Extensions = ".jpg,.jpeg")]
        [Display(Name = "profile photo")]
        [Required(ErrorMessage = "Your must provide a profile photo")]       
        public IFormFile ImgeFoto { get; set; }

        public decimal Distance { get; set; }
        public IEnumerable<SelectListItem> CustomerTypeList { get; set; }
        public IEnumerable<SelectListItem> TransportTypeList { get; set; }
        public IEnumerable<SelectListItem> CustomerStatusList { get; set; }
        public IEnumerable<SelectListItem> PriorityTypeList { get; set; }

        public IEnumerable<SelectListItem> PackageSizeList { get; set; }

        public string PersonalPhotoUri { get; set; }
        public int CustomerId { get; set; } 

        public IFormFile file { get; set; }

        public string PrimaryPhone { get; set; }
        public string FilePath { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        [Remote("ValidateUserName", "Sender", ErrorMessage = "Username is not available.")]
        public string UserEmail { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }


        [Required(ErrorMessage = "Your must provide a PhoneNumber")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        [Display(Name = "Cell Number")]
        public string Phone { get; set; }
        public string IdentityGuid { get; set; }
        public string UserGuid { get; set; }




        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Your must provide a Last Name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Your must provide a First Name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        public CustomerType CustomerType { get; set; }
        [Display(Name = "Customer Type")]
        [Required(ErrorMessage = "Customer Type required")]
        [Range(1, 999, ErrorMessage = "Select a valid Customer type")]
        public int CustomerTypeId { get; set; }

        

       
        [Display(Name = "Street")]
        [Required(ErrorMessage = "Your must provide a pickup street")]
        public String PickupStreet { get; set; }
        [Display(Name = "City")]
        [Required(ErrorMessage = "Your must provide a pickup city")]
        public String PickupCity { get; set; }
        public String PickupState { get; set; }
        public String PickupCountry { get; set; }
        [Display(Name = "Postal code")]
        [Required(ErrorMessage = "Your must provide a pickup Postal Code")]
        [DataType(DataType.PostalCode)]
        public String PickupZipCode { get; set; }  

    }

    public class CustomerModelComplete
    {
        public CustomerModelComplete()
        {
            CustomerTypeList = new List<SelectListItem>();
            TransportTypeList = new List<SelectListItem>();
            CustomerStatusList = new List<SelectListItem>();
            PriorityTypeList = new List<SelectListItem>();

            PackageSizeList = new List<SelectListItem>();
            Addresses = new List<AddressModel>();
            }


        public decimal Distance { get; set; }
        public IEnumerable<SelectListItem> CustomerTypeList { get; set; }
        public IEnumerable<SelectListItem> TransportTypeList { get; set; }
        public IEnumerable<SelectListItem> CustomerStatusList { get; set; }
        public IEnumerable<SelectListItem> PriorityTypeList { get; set; }

        public IEnumerable<SelectListItem> PackageSizeList { get; set; }

        public string PersonalPhotoUri { get; set; }
        public int CustomerId { get; set; }
        public int PackageSizeId { get; set; }

        public IFormFile file { get; set; }
        [Required(ErrorMessage = "Your must provide a primary phone number")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid primary phone number")]
        [Display(Name = "Cell Number")]
        public string PrimaryPhone { get; set; }
        public string FilePath { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        [Remote("ValidateUserName", "Sender", ErrorMessage = "Username is not available.")]
        public string UserEmail { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }


        [Required(ErrorMessage = "Your must provide a PhoneNumber")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        [Display(Name = "Cell Number")]
        public string Phone { get; set; }
        public string IdentityGuid { get; set; }
        public string UserGuid { get; set; }




        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Your must provide a Last Name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Your must provide a First Name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        public CustomerType CustomerType { get; set; }
        [Display(Name = "Customer Type")]
        [Required(ErrorMessage = "Customer Type required")]
        [Range(1, 999, ErrorMessage = "Select a valid Customer type")]
        public int CustomerTypeId { get; set; }

        //[Display(Name = "Transport Type")]
        //[Required(ErrorMessage = "Transport Type required")]
        //[Range(1, 999, ErrorMessage = "Select a valid transport type")]
        //public int? TransportTypeId { get; set; }
        public TransportType TransportType { get; set; }
        public int CustomerStatusId { get; set; }
        public CustomerStatus CustomerStatus { get; set; }
        public int? MaxPackage { get; set; }
        public int? PickupRadius { get; set; }
        public int? DeliverRadius { get; set; }

        //[Display(Name = "Quantity")]
        //[Required(ErrorMessage = "required")]
        //public int? Quantity { get; set; }
        [Display(Name = "Weight")]
        [Required(ErrorMessage = "required")]
        public decimal? ShippingWeight { get; set; }

        [Display(Name = "Vehicle Info")]
        [Required(ErrorMessage = "provide your vehicle info")]
        public string VehicleInfo { get; set; }

        public List<AddressModel> Addresses { get; set; }


        public int AddressTypeId { get; set; }

        [Required(ErrorMessage = "Your must provide a PhoneNumber")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        [Display(Name = "Phone")]
        public String PickupPhone { get; set; }
        [Display(Name = "Contact person")]
        public String PickupContact { get; set; }
        [Display(Name = "Street")]
        [Required(ErrorMessage = "Your must provide a pickup street")]
        public String PickupStreet { get; set; }
        [Display(Name = "City")]
        [Required(ErrorMessage = "Your must provide a pickup city")]
        public String PickupCity { get; set; }
        public String PickupState { get; set; }
        public String PickupCountry { get; set; }
        [Display(Name = "Postal code")]
        [Required(ErrorMessage = "Your must provide a pickup Postal Code")]
        [DataType(DataType.PostalCode)]
        public String PickupZipCode { get; set; }
        //public Double PickupLatitude { get;  set; }
        //public Double PickupLongitude { get;  set; }



        [Required(ErrorMessage = "Your must provide a PhoneNumber")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        [Display(Name = "Phone")]
        public String DeliveryPhone { get; set; }
        [Display(Name = "Contact person")]
        public String DeliveryContact { get; set; }

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
        //public Double DeliveryLatitude { get;  set; }
        //public Double DeliveryLongitude { get;  set; }




        /// <summary>
        /// Shipment
        /// </summary>
        public string IdentityCode { get; set; }
        public PriorityType PriorityType { get; set; }
        public int PriorityTypeLevel { get; set; }

        public int TransportTypeIdS { get; set; }
        public TransportType TransportTypeS { get; set; }


        [Display(Name = "Estimate shipping value")]
        [Required(ErrorMessage = "required")]
        public Decimal? Amount { get; set; }
        public Decimal Tax { get; set; }
        public Decimal Discount { get; set; }
        public string PromoCode { get; set; }

        [Display(Name = "Priority Type")]
        [Required(ErrorMessage = "Priority Type required")]
        [Range(1, 999, ErrorMessage = "Select a valid Priority Type")]
        public int PriorityTypeId { get; set; }

        [DataType(DataType.MultilineText)]
        public string Note { get; set; }


        ////[CreditCard]
        //[DataType(DataType.CreditCard)]
        //[Display(Name = "Credit Card Number")]
        //[Required(ErrorMessage = "required")]
        ////[Range(100000000000, 9999999999999999999, ErrorMessage = "must be between 12 and 19 digits")]
        //public String CardNumber { get; set; }
        //[Required]
        //[Display(Name = "Security Number")]
        //public int SecurityNumber { get; set; }
        //[Required]
        //[Display(Name = "Card Holder Name")]
        //public String CardHolderName { get; set; }
        //[Required]
        //[Display(Name = "Expiration")]
        //public String Expiration { get; set; }





    }

    public class NewSenderResult
    {
        public string UserName { get; set; }
        public string Amount { get; set; }
        public string Message { get; set; }
        

    }

 
    public class NewShipment
    {
        public NewShipment()
        {
            CustomerTypeList = new List<SelectListItem>();
            TransportTypeList = new List<SelectListItem>();
            CustomerStatusList = new List<SelectListItem>();
            PriorityTypeList = new List<SelectListItem>();
            PickupAddresses = new List<SelectListItem>();
            DropAddresses = new List<SelectListItem>();

            PackageSizeList = new List<SelectListItem>();

        }
        public int PickupAddressId { get; set; }
        public int DropAddressId { get; set; } 

        public IEnumerable<SelectListItem> PickupAddresses { get; set; }
        public IEnumerable<SelectListItem> DropAddresses { get; set; }


        public IEnumerable<SelectListItem> CustomerTypeList { get; set; }
        public IEnumerable<SelectListItem> TransportTypeList { get; set; }
        public IEnumerable<SelectListItem> CustomerStatusList { get; set; }
        public IEnumerable<SelectListItem> PriorityTypeList { get; set; }


        public IEnumerable<SelectListItem> PackageSizeList { get; set; }

        public decimal TotalCharge { get; set; }

        public decimal Distance { get; set; }
        public int CustomerId { get; set; }

        public int PackageSizeId { get; set; }


        public IFormFile file { get; set; }

        public string PickupPictureUri { get; set; }

        [Required(ErrorMessage = "Your must provide a Pickup PhoneNumber")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        [Display(Name = "Phone")]
        public String PickupPhone { get; set; }

        [Display(Name = "Contact person")]
        public String PickupContact { get; set; }

        [Display(Name = "Street")]
        [Required(ErrorMessage = "Your must provide a pickup street")]
        public String PickupStreet { get; set; }
        [Display(Name = "City")]
        [Required(ErrorMessage = "Your must provide a pickup city")]
        public String PickupCity { get; set; }
        public String PickupState { get; set; }
        public String PickupCountry { get; set; }
        [Display(Name = "Postal code")]
        [Required(ErrorMessage = "Your must provide a pickup Postal Code")]
        [DataType(DataType.PostalCode)]
        public String PickupZipCode { get; set; } 

        public decimal? Weight { get; set; }

        [Required(ErrorMessage = "Your must provide a drop phone,Number")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        [Display(Name = "Phone")]
        public String DeliveryPhone { get; set; }
        [Display(Name = "Contact person")]
        public String DeliveryContact { get; set; }

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




        /// <summary>
        /// Shipment
        /// </summary>
        public string IdentityCode { get; set; }
        public PriorityType PriorityType { get; set; }
        public int PriorityTypeLevel { get; set; }
         

        [Required(ErrorMessage = "Your must provide a Estimate shipping value")]
        public Decimal? Amount { get; set; }
        public Decimal? ShippingWeight { get; set; } 
        public string PromoCode { get; set; }

        public int PriorityTypeId { get; set; }

        [DataType(DataType.MultilineText)]
        public string Note { get; set; }

         




    }
    public class AddressModel {

        [Required(ErrorMessage = "Your must provide a PhoneNumber")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        [Display(Name = "Phone")]
        public String Phone { get; set; }
        [Display(Name = "Contact person")]
        public String Contact { get; set; }
        [Display(Name = "Street")]
        [Required(ErrorMessage = "Your must provide a pickup street")]
        public String Street { get; set; }
        [Display(Name = "City")]
        [Required(ErrorMessage = "Your must provide a pickup city")]
        public String City { get; set; }
        public String State { get; set; }
        public String Country { get; set; }
        [Display(Name = "Postal code")]
        [Required(ErrorMessage = "Your must provide a pickup Postal Code")]
        [DataType(DataType.PostalCode)]
        public String ZipCode { get; set; }
        public Double Latitude { get;  set; }
        public Double Longitude { get;  set; }
        public String TypeAddress { get; set; }
        public int CustomerId { get; set; }
        public int Id { get; set; }
        
    }
}
