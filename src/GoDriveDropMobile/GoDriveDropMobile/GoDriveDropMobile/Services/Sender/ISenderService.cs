using GoDriveDrop.Core.Models;
using GoDriveDrop.Core.Models.Commons;
using GoDriveDrop.Core.Models.Shippments;
using GoDriveDrop.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace GoDriveDrop.Core.Services.Driver
{
    public interface ISenderService
    {
        Task<string> CreatePackageAsync(NewPackageModel p, string token);
        Task<string> CreateSenderAsync(NewSenderModel newSender);
        Task<string> UpdateSenderAsync(CustomerModel newSender);
        Task<ObservableCollection<NewSenderModel>> GetDriversAsync(string token);
        Task<NewSenderModel> GetSenderAsync(int driverId, string token);
        Task<NewSenderModel> GetSenderAsync(string userName, string token);
        //Task<bool> CancelDriverAsync(int DriverId, string token);
        // BasketCheckout MapDriverToBasket(NewDriver Driver);
    }
}
