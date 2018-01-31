using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Bl.ViewModels
{
    public class DriverInfoModel
    {
        [Required(ErrorMessage = "Your must provide a Last Name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Your must provide a First Name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        public string Email { get; set; }
        [Required(ErrorMessage = "Your must provide a PhoneNumber")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        [Display(Name = "Cell Number")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Your must provide a primary phone number")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid primary phone number")]
        [Display(Name = "Cell Number")]
        public string PrimaryPhone { get; set; } 
        public int StatusId { get; set; }
        public string CustomerStatus { get; set; }
        public int Id { get; set; }

        public Decimal Comission { get; set; }

        public string DriverLincensePictureUri { get; set; }
        public string PersonalPhotoUri { get; set; }
        public string VehiclePhotoUri { get; set; }
        public string InsurancePhotoUri { get; set; }

        public double PickupRadius { get; set; }
        public double DeliverRadius { get; set; }
        public string VerificationId { get; set; }
         
        public IFormFile PhotoUrl { get; set; }


    }
}
