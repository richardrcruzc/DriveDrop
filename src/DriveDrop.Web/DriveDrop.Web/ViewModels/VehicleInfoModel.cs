using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Web.ViewModels
{
    public class VehicleInfoModel
    {
        public VehicleInfoModel()
        {
            TransportTypeList = new List<SelectListItem>();
        }

        public IEnumerable<SelectListItem> TransportTypeList { get; set; }

        public int DriverId { get; set; }
        public int Id { get; set; }

        public String TypeAddress { get; set; }
        [Required(ErrorMessage = "Your must provide a  lincensePictureUri")]
        public string lincensePictureUri { get; set; }
        [Required(ErrorMessage = "Your must provide a  vehicle vehiclePhotoUri")]
        public string vehiclePhotoUri { get; set; }
        [Required(ErrorMessage = "Your must provide a  insurancePhotoUri")]
        public string insurancePhotoUri { get; set; }
        public int vehicleTypeId { get; set; }

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

        public string DriverLincensePictureUri { get; set; }

        public int? TransportTypeId { get; set; }
        public TransportType TransportType { get; set; }

    }
}
