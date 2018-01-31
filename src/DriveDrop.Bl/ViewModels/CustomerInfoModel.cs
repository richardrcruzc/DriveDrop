using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Bl.ViewModels
{
    public class CustomerInfoModel
    {
        [Required(ErrorMessage = "Your must provide First Name")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Your must provide Last Name")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Your must provide a profile photo")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Your must provide a PhoneNumber")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Your must provide a PhoneNumber")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        public string PrimaryPhone { get; set; }
        [Required(ErrorMessage = "Your must provide a profile photo")]
        public int StatusId { get; set; }        
        public string CustomerStatus { get; set; }
        public int Id { get; set; }

        public string VerificationId { get; set; }

        public string DriverLincensePictureUri { get; set; }
        public string PersonalPhotoUri { get; set; }
        public string VehiclePhotoUri { get; set; }
        public string InsurancePhotoUri { get; set; }

        
        //[Required(ErrorMessage = "Your must provide a profile photo")]
        public IFormFile PhotoUrl { get; set; }

    }
}
