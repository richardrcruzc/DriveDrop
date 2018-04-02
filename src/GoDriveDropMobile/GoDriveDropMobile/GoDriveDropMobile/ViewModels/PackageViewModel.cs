using GoDriveDrop.Core.Models.Commons;
using GoDriveDrop.Core.Models.Shippments;
using GoDriveDrop.Core.Services.Address;
using GoDriveDrop.Core.Services.Navigation;
using GoDriveDrop.Core.Services.User;
using GoDriveDrop.Core.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace GoDriveDrop.Core.ViewModels
{
    public class PackageViewModel : BaseViewModel
    {      
        private ObservableCollection<ShipmentModel> _packageModel;
        private IUserService _userService;
        private ShipmentModel _selectedItem { get; set; }

        public PackageViewModel( IUserService userService)
        {
            Title = "My Packages";
             
            _userService = userService;

            if (PackagesItems == null)
                PackagesItems = new ObservableCollection<ShipmentModel>();

            foreach (var add in GlobalSetting.Instance.CurrentCustomerModel.ShipmentSenders)
                PackagesItems.Add(add);

            showList = true; 
            ShowSelected = false;
        }
        public ShipmentModel SelectedItem
        {

            get
            {
                return _selectedItem;
            }

            set
            {
                _selectedItem = value;
                RaisePropertyChanged(() => SelectedItem);

                // When your item is selected, you can open a new "PageDetail" and pass the value
                //Application.Current.MainPage.Navigation.PushAsync(new PageDetail(_selectedItem)); // If you are in a Navigation page
            }
        }

        public ICommand NewPackageCommand => new Command(async () => await NewPackage());
        private async Task NewPackage()
        {
            IsBusy = true;
            //var navigationService = ViewModelLocator.Resolve<INavigationService>();
            //await navigationService.NavigateToAsync<NewPackageViewModel>();
            //await navigationService.RemoveBackStackAsync();

            var mainPage = new NavigationPage(new NewPackageView()
            {
                Title = "New Package",
            })
            {

                // Set the NavigationBar TextColor and Background Color
                BarBackgroundColor = Color.FromHex("#223669"),
                BarTextColor = Color.White
            };
            Application.Current.MainPage = mainPage;

            IsBusy = false;
        }


        public ICommand BackToListCommand => new Command(async () => await BackToList());
        private async Task BackToList()
        {
            IsBusy = true;
            ShowList = true;
            ShowSelected = false;
            
            IsBusy = false;
        }
        public ICommand AddCommand => new Command<ShipmentModel>(async (item) => await AddItemAsync(item)); 
        private async Task AddItemAsync(ShipmentModel item)
        {
            IsBusy = true;
            //RaisePropertyChanged(() => PackagesItems);
            if (item != null)
            {
                //item.PickupPictureUri = GlobalSetting.Instance.PicBaseUrl.Replace("[0]", System.Net.WebUtility.UrlEncode(item.PickupPictureUri));
                //item.DeliveredPictureUri = GlobalSetting.Instance.PicBaseUrl.Replace("[0]", System.Net.WebUtility.UrlEncode(item.PickupPictureUri));                 

                ShowList = false;
                ShowSelected = true;
                SelectedItem = item;
                
            }
            IsBusy = false;
        } 

        private bool showList;
        public bool ShowList
        {
            get { return showList; }
            set
            {
                showList = value;
                RaisePropertyChanged(() => ShowList);
            }
        }
        private bool showSelected;
        public bool ShowSelected
        {
            get { return showSelected; }
            set
            {
                showSelected = value;
                RaisePropertyChanged(() => ShowSelected);
            }
        }
        public ObservableCollection<ShipmentModel> PackagesItems
        {
            get { return _packageModel; }
            set
            {
                _packageModel = value;
                RaisePropertyChanged(() => PackagesItems);
              
            }
        }
        public override async Task InitializeAsync(object navigationData)
        {
            if (PackagesItems == null)
                PackagesItems = new ObservableCollection<ShipmentModel>();

            
            var userInfo = await _userService.GetUserInfoAsync(GlobalSetting.Instance.AuthAccessToken);

            // Update Address
            var address = GlobalSetting.Instance.CurrentCustomerModel.ShipmentSenders;
            if (address != null && address.Count > 0)
            {
                address.Clear();
                foreach (var add in address)
                    PackagesItems.Add(add);
            }

            await base.InitializeAsync(navigationData);
        }
        }
}
