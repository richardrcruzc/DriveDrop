using GoDriveDrop.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace GoDriveDrop.Core.Services.Driver
{
    public interface ISenderService
    {
        Task<string> CreateSenderAsync(NewSenderModel newSender);
        Task<ObservableCollection<NewSenderModel>> GetDriversAsync(string token);
        Task<NewSenderModel> GetSenderAsync(int driverId, string token);
        Task<NewSenderModel> GetSenderAsync(string userName, string token);
        //Task<bool> CancelDriverAsync(int DriverId, string token);
        // BasketCheckout MapDriverToBasket(NewDriver Driver);
    }
}
