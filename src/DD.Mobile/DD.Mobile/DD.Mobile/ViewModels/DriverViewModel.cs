using DD.Mobile.Models.Drivers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD.Mobile.ViewModels
{
   
  public class DriverViewModel : BaseDriverViewModel
    {
        ObservableCollection<DriverEntry> _drivers;
        public ObservableCollection<DriverEntry> Drivers {
            get { return _drivers; }
            set {
                _drivers = value;
                OnPropertyChanged();
            }
        }
    }
}
