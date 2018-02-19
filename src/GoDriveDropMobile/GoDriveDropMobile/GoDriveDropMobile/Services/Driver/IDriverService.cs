using GoDriveDrop.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace GoDriveDrop.Core.Services.Driver
{
    public interface IDriverService
    {
        Task<string> CreateDriverAsync(NewDriverModel newDriver);
        Task<ObservableCollection<NewDriverModel>> GetDriversAsync(string token);
        Task<NewDriverModel> GetDriverAsync(int driverId, string token);
        Task<NewDriverModel> GetDriverAsync(string userName, string token);
        //Task<bool> CancelDriverAsync(int DriverId, string token);
        // BasketCheckout MapDriverToBasket(NewDriver Driver);
    }
}
