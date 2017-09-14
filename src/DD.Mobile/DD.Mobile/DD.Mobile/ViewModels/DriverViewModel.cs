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
        ObservableCollection<DriverEntry> _driverEntries;
        public ObservableCollection<DriverEntry> driverEntries
        {
            get { return _driverEntries; }
            set {
                _driverEntries = value;
             
            }
        }
        public DriverViewModel()
        {

            driverEntries= new ObservableCollection <DriverEntry>() {
                new DriverEntry{ FirstName="One", LastName="1wewe",longitude=-122.360348, Latitude=47.175708,  MaxPackage=1, PersonalPhotoUri="http://trailswa.com.au/media/cache/media/images/trails/_mid/FullSizeRender1_600_480_c1.jpg",},
                new DriverEntry{ FirstName="Two", LastName="2we wewe" ,longitude=-122.360348, Latitude=47.175708, MaxPackage=2, PersonalPhotoUri="http://trailswa.com.au/media/cache/media/images/trails/_mid/Ancient_Empire_534_480_c1.jpg", },
                new DriverEntry{ FirstName="Three", LastName="3 we wewe", longitude=-122.360348, Latitude=47.175708, MaxPackage=3, PersonalPhotoUri="http://trailswa.com.au/media/cache/media/images/trails/_mid/Ancient_Empire_534_480_c1.jpg", },
            };
        }
    }
}
