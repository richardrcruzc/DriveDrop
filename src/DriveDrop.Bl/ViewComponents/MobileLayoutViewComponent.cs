//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Wangkanai.Detection;

//namespace DriveDrop.Bl.ViewComponents
//{
//    public class MobileLayoutViewComponent:  ViewComponent
//    {

//        private readonly IDevice _device;

//        public MobileLayoutViewComponent(IDeviceResolver deviceResolver)
//        {
//            _device = deviceResolver.Device;
//        }

//        public   bool InvokeAsync()
//        {
//            if (_device.Type == DeviceType.Mobile || _device.Type == DeviceType.Tablet)
//            {
//                //some logic
//                return true;
//            }
//            return false;
//        }
//    }
//}
