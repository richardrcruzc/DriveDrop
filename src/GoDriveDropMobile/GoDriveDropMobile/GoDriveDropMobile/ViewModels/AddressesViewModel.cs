using GoDriveDrop.Core.Models.Commons;
using GoDriveDrop.Core.Services.Address;
using GoDriveDrop.Core.Services.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace GoDriveDrop.Core.ViewModels
{
    public class AddressesViewModel : BaseViewModel
    {
        private IAddressService _addressService;
        private ObservableCollection<AddressModel> _addressModel;
        private IUserService _userService;

        public AddressesViewModel(IAddressService addressService, IUserService userService)
        {
            Title = "My Addresses";

            _addressService = addressService;
            _userService = userService;

            if (AddressesItem == null)
                AddressesItem = new ObservableCollection<AddressModel>();

            foreach (var add in GlobalSetting.Instance.CurrentCustomerModel.Addresses)
                AddressesItem.Add(add);
        }

        public ObservableCollection<AddressModel> AddressesItem
        {
            get { return _addressModel; }
            set
            {
                _addressModel = value;
                RaisePropertyChanged(() => AddressesItem);
            }
        }
        public override async Task InitializeAsync(object navigationData)
        {
            if (AddressesItem == null)
                AddressesItem = new ObservableCollection<AddressModel>();

            
            var userInfo = await _userService.GetUserInfoAsync(GlobalSetting.Instance.AuthAccessToken);

            // Update Address
            var address = GlobalSetting.Instance.CurrentCustomerModel.Addresses;
            if (address != null && address.Count>0)
            {
                address.Clear();
                foreach(var add in address)
                    AddressesItem.Add(add);
            }
            
            await base.InitializeAsync(navigationData);
        }
        }
}
