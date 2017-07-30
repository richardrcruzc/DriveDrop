using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Api.ViewModels
{
    public class VehicleInfoModel
    {
        public String TypeAddress { get; set; }
        public string lincensePictureUri { get; set; }
        public string vehiclePhotoUri { get; set; }
        public string insurancePhotoUri { get; set; }
        public int vehicleTypeId { get; set; }
    }
}
