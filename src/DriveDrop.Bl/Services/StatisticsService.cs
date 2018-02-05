using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wangkanai.Detection;

namespace DriveDrop.Bl.Services
{
    public class StatisticsService
    {
        private readonly IDevice _device;

        public StatisticsService(IDeviceResolver deviceResolver)
        {
            _device = deviceResolver.Device;
        }

        public bool Mobile()
        {
            if (_device.Type == DeviceType.Mobile || _device.Type == DeviceType.Tablet)
            {
                //some logic
                return true;
            }
            return false;
        }
    }
}
