using DriveDrop.Core.Models.Drivers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveDrop.Core.Services.Driver
{ 
    public interface IDriverService
    {
        Task<string> CreateDriverAsync(NewDriver newDriver );
        Task<ObservableCollection<NewDriver>> GetDriversAsync(string token);
        Task<NewDriver> GetDriverAsync(int driverId, string token);
        Task<NewDriver> GetDriverAsync(string userName, string token);
        //Task<bool> CancelDriverAsync(int DriverId, string token);
        // BasketCheckout MapDriverToBasket(NewDriver Driver);
    }
}
