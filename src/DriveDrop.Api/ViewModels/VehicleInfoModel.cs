﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Api.ViewModels
{
    public class VehicleInfoModel
    {
        public int DriverId { get; set; }

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
    }
}
