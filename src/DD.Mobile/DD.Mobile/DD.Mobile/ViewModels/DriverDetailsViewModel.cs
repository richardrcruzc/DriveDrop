using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DD.Mobile.Models.Drivers;

namespace DD.Mobile.ViewModels
{
    public class DriverDetailsViewModel: BaseDriverViewModel
    {
        DriverEntry _driverEntry;

        public DriverEntry DriverEntry
        {
            get { return _driverEntry; }
            set
            {
                _driverEntry = value;
                OnPropertyChanged();

            }
        }

        public DriverDetailsViewModel(DriverEntry driverEntry)
        {
            DriverEntry = driverEntry;
        }
    }
}
