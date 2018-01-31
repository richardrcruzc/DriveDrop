
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Bl.ViewModels
{
    public class AddressModel
    {

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
        public Double Latitude { get; set; }
        public Double Longitude { get; set; }
        public String TypeAddress { get; set; }
        public int CustomerId { get; set; }
        public int Id { get; set; }

    }
}
