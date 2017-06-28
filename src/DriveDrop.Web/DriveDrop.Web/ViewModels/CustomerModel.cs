
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
    public class CustomerModel
    {
        public CustomerModel()
        {
            CustomerTypeList = new List<SelectListItem>();
            TransportTypeList = new List<SelectListItem>();
            CustomerStatusList = new List<SelectListItem>();
            PriorityTypeList = new List<SelectListItem>();

        }

        public IEnumerable<SelectListItem> CustomerTypeList { get; set; }
        public IEnumerable<SelectListItem> TransportTypeList { get; set; }
        public IEnumerable<SelectListItem> CustomerStatusList { get; set; }
        public IEnumerable<SelectListItem> PriorityTypeList { get; set; }

        public int CustomerId { get; set; }

        public IFormFile file { get; set; }


        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
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
        [Display(Name = "Phone")]
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
        public int CustomerTypeId { get; set; }
        public int? TransportTypeId { get; set; }
        public TransportType TransportType { get; set; }
        public int CustomerStatusId { get; set; }
        public CustomerStatus CustomerStatus { get; set; }
        public int? MaxPackage { get; set; }
        public int? PickupRadius { get; set; }
        public int? DeliverRadius { get; set; }




        [Required(ErrorMessage = "Your must provide a PhoneNumber")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        [Display(Name = "Phone")] 
        public String PickupPhone { get;  set; }

        [Display(Name = "Contact person")]
        public String PickupContact { get;  set; }

        [Display(Name = "Street")]
        [Required(ErrorMessage = "Your must provide a pickup street")]
        public String PickupStreet { get;  set; }
        [Display(Name = "City")]
        [Required(ErrorMessage = "Your must provide a pickup city")]
        public String PickupCity { get;  set; }
        public String PickupState { get;  set; }
        public String PickupCountry { get;  set; }
        [Display(Name = "Postal code")]
        [Required(ErrorMessage = "Your must provide a pickup Postal Code")]
        [DataType(DataType.PostalCode)]
        public String PickupZipCode { get;  set; }
        //public Double PickupLatitude { get;  set; }
        //public Double PickupLongitude { get;  set; }



        [Required(ErrorMessage = "Your must provide a PhoneNumber")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        [Display(Name = "Phone")]
        public String DeliveryPhone { get;  set; }
        [Display(Name = "Contact person")]
        public String DeliveryContact { get;  set; }

        [Display(Name = "Street")]
        [Required(ErrorMessage = "Your must provide a delivery street")]
        public String DeliveryStreet { get;  set; }
        [Display(Name = "City")]
        [Required(ErrorMessage = "Your must provide a delivery City")]
        public String DeliveryCity { get;  set; }
        public String DeliveryState { get;  set; }
        public String DeliveryCountry { get;  set; }
        [Display(Name = "Postal Code")]
        [Required(ErrorMessage = "Your must provide a delivery Postal Code")]
        [DataType(DataType.PostalCode)]
        public String DeliveryZipCode { get;  set; }
        //public Double DeliveryLatitude { get;  set; }
        //public Double DeliveryLongitude { get;  set; }




        /// <summary>
        /// Shipment
        /// </summary>
        public string IdentityCode { get;  set; }
        public PriorityType PriorityType { get;  set; }
        public int PriorityTypeLevel { get;  set; }

        public int TransportTypeIdS { get;  set; }
        public TransportType TransportTypeS { get;  set; }



        public Decimal Amount { get;  set; }
        public Decimal Tax { get;  set; }
        public Decimal Discount { get;  set; }
        public string PromoCode { get;  set; }

        public int PriorityTypeId { get;  set; }

        [DataType(DataType.MultilineText)]
        public string Note { get; set; }

         
        //[CreditCard]
        [DataType(DataType.CreditCard)]
        [Display(Name = "Credit Card Number")]
        [Required(ErrorMessage = "required")]
        //[Range(100000000000, 9999999999999999999, ErrorMessage = "must be between 12 and 19 digits")]
        public String CardNumber { get; set; }
        [Required]        
        [Display(Name = "Security Number")]
        public int SecurityNumber { get; set; }
        [Required]
        [Display(Name = "Card Holder Name")]
        public String CardHolderName { get; set; }
        [Required]
        [Display(Name = "Expiration")]
        public String Expiration { get; set; }
        
        



    }


    public class DriverModel
    {
        public DriverModel()
        {
            CustomerTypeList = new List<SelectListItem>();
            TransportTypeList = new List<SelectListItem>();
            CustomerStatusList = new List<SelectListItem>();
            PriorityTypeList = new List<SelectListItem>();

        }

        public IEnumerable<SelectListItem> CustomerTypeList { get; set; }
        public IEnumerable<SelectListItem> TransportTypeList { get; set; }
        public IEnumerable<SelectListItem> CustomerStatusList { get; set; }
        public IEnumerable<SelectListItem> PriorityTypeList { get; set; }

        public int CustomerId { get; set; }

        [Display(Name = "Images")]
       // [FileExtensions(Extensions = "JPG,TIF,PNG,GIF")]
        [DataType(DataType.Upload)]
        public IEnumerable<IFormFile> Files { get; set; }


        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        [Remote("ValidateUserName", "Driver", ErrorMessage = "Username is not available.")]
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
        [Display(Name = "Phone")]
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
        public int CustomerTypeId { get; set; }
        public int? TransportTypeId { get; set; }
        public TransportType TransportType { get; set; }
        public int CustomerStatusId { get; set; }
        public CustomerStatus CustomerStatus { get; set; }
        public int? MaxPackage { get; set; }
        public int? PickupRadius { get; set; }
        public int? DeliverRadius { get; set; }





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



        public Decimal Amount { get; set; }
        public Decimal Tax { get; set; }
        public Decimal Discount { get; set; }
        public string PromoCode { get; set; }

        public int PriorityTypeId { get; set; }

        [DataType(DataType.MultilineText)]
        public string Note { get; set; }


        //[CreditCard]
        [DataType(DataType.CreditCard)]
        [Display(Name = "Credit Card Number")]
        [Required(ErrorMessage = "required")]
        //[Range(100000000000, 9999999999999999999, ErrorMessage = "must be between 12 and 19 digits")]
        public String CardNumber { get; set; }
        [Required]
        [Display(Name = "Security Number")]
        public int SecurityNumber { get; set; }
        [Required]
        [Display(Name = "Card Holder Name")]
        public String CardHolderName { get; set; }
        [Required]
        [Display(Name = "Expiration")]
        public String Expiration { get; set; }





    }

    public class NewShipment
    {
        public NewShipment()
        {
            CustomerTypeList = new List<SelectListItem>();
            TransportTypeList = new List<SelectListItem>();
            CustomerStatusList = new List<SelectListItem>();
            PriorityTypeList = new List<SelectListItem>();

        }

        public IEnumerable<SelectListItem> CustomerTypeList { get; set; }
        public IEnumerable<SelectListItem> TransportTypeList { get; set; }
        public IEnumerable<SelectListItem> CustomerStatusList { get; set; }
        public IEnumerable<SelectListItem> PriorityTypeList { get; set; }

        public int CustomerId { get; set; }

        public IFormFile file { get; set; }


         


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

        public int TransportTypeId { get; set; }
        public TransportType TransportType { get; set; }



        public Decimal Amount { get; set; }
        public Decimal Tax { get; set; }
        public Decimal Discount { get; set; }
        public string PromoCode { get; set; }

        public int PriorityTypeId { get; set; }

        [DataType(DataType.MultilineText)]
        public string Note { get; set; }


        //[CreditCard]
        [DataType(DataType.CreditCard)]
        [Display(Name = "Credit Card Number")]
        [Required(ErrorMessage = "required")]
        //[Range(100000000000, 9999999999999999999, ErrorMessage = "must be between 12 and 19 digits")]
        public String CardNumber { get; set; }
        [Required]
        [Display(Name = "Security Number")]
        public int SecurityNumber { get; set; }
        [Required]
        [Display(Name = "Card Holder Name")]
        public String CardHolderName { get; set; }
        [Required]
        [Display(Name = "Expiration")]
        public String Expiration { get; set; }





    }
}
