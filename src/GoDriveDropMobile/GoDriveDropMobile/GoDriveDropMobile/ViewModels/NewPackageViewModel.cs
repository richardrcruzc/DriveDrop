using System;
using System.Collections.Generic; 
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input; 
using Xamarin.Forms;
using Plugin.Media; 
using GoDriveDrop.Core.Services.Driver;
using GoDriveDrop.Core.Services.Common;
using GoDriveDrop.Core.Services.User;
using GoDriveDrop.Core.Services.RequestProvider;
using GoDriveDrop.Core.Models.Commons; 
using System.Linq;
using GoDriveDrop.Core.Models.Shippments;
using GoDriveDrop.Core.Models;
using System.Collections.ObjectModel;
using GoDriveDrop.Core.Helpers;

namespace GoDriveDrop.Core.ViewModels
{
    public class NewPackageViewModel : BaseViewModel
    {
        private readonly IRequestProvider _requestProvider;

        private IUserService _userService;
        private NewPackageModel _package;
        private ISenderService _senderService;
        private IGoogleAddress _googleAddress;
        
        private ICommons _commons; 

        public NewPackageViewModel(
              ICommons commons,
          IGoogleAddress googleAddress,
          IRequestProvider requestProvider,
          ISenderService senderService,
          IUserService userService
            )
        {
            _commons = commons;
            _googleAddress = googleAddress;
            _senderService = senderService;
            _userService = userService;

            _requestProvider = requestProvider;

            Icon = "iconlogo";
            Title = "Send a Package"; 
        }


        #region Command and associated methods for Search pickup address Command
        private ObservableCollection<string> _list;
        public ObservableCollection<string> Items
        {
            get
            {
                return _list;
            }
            set
            {
                _list = value;

                RaisePropertyChanged(() => Items);
            }
        }

        private string searchTextForPickupAddress = string.Empty;
        public string SearchTextForPickupAddress
        {
            get { return searchTextForPickupAddress; }
            set
            {
                if (searchTextForPickupAddress != value)
                {
                    searchTextForPickupAddress = value ?? string.Empty;
                    RaisePropertyChanged(() => SearchTextForPickupAddress);
                    // Perform the search
                    if (SearchPickupAddressCommand.CanExecute(null))
                    {
                        SearchPickupAddressCommand.Execute(null);
                    }
                }
            }
        }

        public ICommand SearchPickupAddressCommand => new Command(async () => await DoSearchCommandAsync()); 
        private async Task DoSearchCommandAsync()
        {
            // Refresh the list, which will automatically apply the search text
            string keyword = string.Empty;
            
                 keyword = SearchTextForPickupAddress;
           

            if (keyword.Length >= 3)
            {
                var authToken = GlobalSetting.Instance.AuthAccessToken;
                IEnumerable<String> predictions = await _googleAddress.AutoComplete(keyword, authToken);

                if (predictions.ToList().Count > 0) 
                    {
                        Items = new ObservableCollection<string>(predictions);
                        IsListViewVisibleNewPickupAddress = true;
                    }
                    

            }
        }

        private string _selectedItemNewPickupAddress;
        public string SelectedItemNewPickupAddress
        {
            get
            {
                return _selectedItemNewPickupAddress;
            }
            set
            {
                _selectedItemNewPickupAddress = value;

                if (_selectedItemNewPickupAddress == null)
                    return;
                RaisePropertyChanged(() => SelectedItemNewPickupAddress);
                //SomeCommand.Execute(_selectedItemNewPickupAddress);
                //  DialogService.ShowAlertAsync(_selectedItemNewPickupAddress, "Changes", "Ok");
                if (SelectedItemNewPickupAddress.Trim().Count() > 0)
                {
                    IsListViewVisibleNewPickupAddress = false;
                    IsPickupAddressVisible = true;
                    PickupAddress = SelectedItemNewPickupAddress;
                    SearchTextForPickupAddress = string.Empty;
                    //Items = null;
                }
            }
        }
        public bool sListViewVisibleNewPickupAddress;
        public bool IsListViewVisibleNewPickupAddress
        {
            get
            {
                return sListViewVisibleNewPickupAddress;
            }
            set
            {
                sListViewVisibleNewPickupAddress = value;
                RaisePropertyChanged(() => IsListViewVisibleNewPickupAddress);
            }
        }
        private Generic _selectedPickupAddress;
        public Generic SelectedPickupAddress
        {
            get
            {
                return _selectedPickupAddress;
            }
            set
            {
                _selectedPickupAddress = value;
                RaisePropertyChanged(() => SelectedPickupAddress);
            }
        }
        private string _pickupAddress;
        public string PickupAddress
        {
            get
            {
                return _pickupAddress;
            }
            set
            {
                _pickupAddress = value;
                RaisePropertyChanged(() => PickupAddress);
            }
        }

        public bool isPickupAddressVisible;
        public bool IsPickupAddressVisible
        {
            get
            {
                return isPickupAddressVisible;
            }
            set
            {
                isPickupAddressVisible = value;
                RaisePropertyChanged(() => IsPickupAddressVisible);
            }
        }
        #endregion

        #region Command and associated methods for Search drop address Command
        private ObservableCollection<string> _itemsDropAddress;
        public ObservableCollection<string> ItemsDropAddress
        {
            get
            {
                return _itemsDropAddress;
            }
            set
            {
                _itemsDropAddress = value;

                RaisePropertyChanged(() => ItemsDropAddress);
            }
        }

        private string searchTextForDropAddress = string.Empty;
        public string SearchTextForDropAddress
        {
            get { return searchTextForDropAddress; }
            set
            {
                if (searchTextForDropAddress != value)
                {
                    searchTextForDropAddress = value ?? string.Empty;
                    RaisePropertyChanged(() => SearchTextForDropAddress);
                    // Perform the search
                    if (SearchDropAddressCommand.CanExecute(null))
                    {
                        SearchDropAddressCommand.Execute(null);
                    }
                }
            }
        }

        public ICommand SearchDropAddressCommand => new Command(async () => await DoSearchDropCommandAsync());
        private async Task DoSearchDropCommandAsync()
        {
            // Refresh the list, which will automatically apply the search text
            string keyword = string.Empty;
           
                keyword = SearchTextForDropAddress;

            if (keyword.Length >= 3)
            {
                var authToken = GlobalSetting.Instance.AuthAccessToken;
                IEnumerable<String> predictions = await _googleAddress.AutoComplete(keyword, authToken);

                if (predictions.ToList().Count > 0)
                { 
                        ItemsDropAddress = new ObservableCollection<string>(predictions);
                        IsListViewVisibleNewDropAddress = true;
                    }

            }
        }

        private string _selectedItemNewDropAddress;
        public string SelectedItemNewDropAddress
        {
            get
            {
                return _selectedItemNewDropAddress;
            }
            set
            {
                _selectedItemNewDropAddress = value;

                if (_selectedItemNewDropAddress == null)
                    return;
                RaisePropertyChanged(() => SelectedItemNewDropAddress);
                //SomeCommand.Execute(_selectedItemNewDropAddress);
                //  DialogService.ShowAlertAsync(_selectedItemNewDropAddress, "Changes", "Ok");
                if (SelectedItemNewDropAddress.Trim().Count() > 0)
                {
                    IsListViewVisibleNewDropAddress = false;
                    IsDropAddressVisible = true;
                   DropAddress = SelectedItemNewDropAddress;
                    SearchTextForDropAddress = string.Empty;
                }
            }
        }
        public bool sListViewVisibleNewDropAddress;
        public bool IsListViewVisibleNewDropAddress
        {
            get
            {
                return sListViewVisibleNewDropAddress;
            }
            set
            {
                sListViewVisibleNewDropAddress = value;
                RaisePropertyChanged(() => IsListViewVisibleNewDropAddress);
            }
        }
        
        private Generic _selectedDropAddress;
        public Generic SelectedDropAddress
        {
            get
            {
                return _selectedDropAddress;
            }
            set
            {
                _selectedDropAddress = value;
                RaisePropertyChanged(() => SelectedDropAddress);
            }
        }
        private string _dropAddress;
        public string DropAddress
        {
            get
            {
                return _dropAddress;
            }
            set
            {
                _dropAddress = value;
                RaisePropertyChanged(() => DropAddress);
            }
        }
        public bool isDropAddressVisible;
        public bool IsDropAddressVisible
        {
            get
            {
                return isDropAddressVisible;
            }
            set
            {
                isDropAddressVisible = value;
                RaisePropertyChanged(() => IsDropAddressVisible);
            }
        }
        #endregion 
        #region [Show drop address]
        private bool showNewDropAddress;
        public bool ShowNewDropAddress
        {
            get
            {
                return showNewDropAddress;
            }
            set
            {
                showNewDropAddress = value;
                RaisePropertyChanged(() => ShowNewDropAddress);
            }

        }

        private int dropAddressId;
        public int DropAddressId
        {
            get
            {
                return dropAddressId;
            }
            set
            {
                dropAddressId = value;
                RaisePropertyChanged(() => DropAddressId);
                if (DropAddressesList[DropAddressId].Id == 0)
                    ShowNewDropAddress = true;
                else
                    ShowNewDropAddress = false;

            }

        }
        #endregion

        #region [Show pickup address]

        private bool showNewPickupAddress;
        public bool ShowNewPickupAddress
        {
            get
            {
                return showNewPickupAddress;
            }
            set
            {
                showNewPickupAddress = value;
                RaisePropertyChanged(() => ShowNewPickupAddress);
            }

        }

        private int pickupAddressId;
        public int PickupAddressId
        {
            get
            {
                return pickupAddressId;
            }
            set
            {
                pickupAddressId = value;
                RaisePropertyChanged(() => PickupAddressId);
                if(PickupAddressesList[pickupAddressId].Id == 0)
                     ShowNewPickupAddress = true;
                else
                    ShowNewPickupAddress = false;

            }

        }
        #endregion

        private bool scrolledView;
        public bool ScrolledView
        {
            get
            {
                return scrolledView;
            }
            set
            {
                scrolledView = value;

                RaisePropertyChanged(() => ScrolledView);
            }
        }

        #region [Lists]
        private List<Generic> _priorityTypesList;
        public List<Generic> PriorityTypesList
        {
            get
            {
                return _priorityTypesList;
            }
            set
            {
                _priorityTypesList = value;

                RaisePropertyChanged(() => PriorityTypesList);
            }
        }

        private List<Generic> _packageSizesList;
        public List<Generic> PackageSizesList
        {
            get
            {
                return _packageSizesList;
            }
            set
            {
                _packageSizesList = value;

                RaisePropertyChanged(() => PackageSizesList);
            }
        }
        
        private List<Generic> _dropAddressesList;
        public List<Generic> DropAddressesList
        {
            get
            {
                return _dropAddressesList;
            }
            set
            {
                _dropAddressesList = value;

                RaisePropertyChanged(() => DropAddressesList);
            }
        }
        private List<Generic> _pickupAddressesList;
        public List<Generic> PickupAddressesList
        {
            get
            {
                return _pickupAddressesList;
            }
            set
            {
                _pickupAddressesList = value;

                RaisePropertyChanged(() => PickupAddressesList);
            }
        }
        #endregion

        private bool isEnabledScrollView;
        public bool IsEnabledScrollView
        {
            get { return isEnabledScrollView; }
            set
            {
                isEnabledScrollView = value;
                RaisePropertyChanged(() => IsEnabledScrollView);
            }
        }


        private CalculatedChargeModel calculatedTotal;
             public CalculatedChargeModel CalculatedTotal
        {
            get { return calculatedTotal; }
            set
            {
                calculatedTotal = value;
                RaisePropertyChanged(() => CalculatedTotal);
            }
        }
        public NewPackageModel PackageInfo
        {
            get { return _package; }
            set
            {
                _package = value;
                RaisePropertyChanged(() => PackageInfo);
            }
        }
        
        private bool buttomGoFocus;
        public bool ButtomGoFocus
        {
            get { return buttomGoFocus; }
            set
            {
                buttomGoFocus = value;
                RaisePropertyChanged(() => ButtomGoFocus);
            }
        }
        private bool showCalculatedTotal;
        public bool ShowCalculatedTotal
        {
            get { return showCalculatedTotal; }
            set
            {
                showCalculatedTotal = value;
                RaisePropertyChanged(() => ShowCalculatedTotal);
 
            }
        }
        private AddressModel fixedPickupAddress;
        public AddressModel FixedPickupAddress
        {
            get { return fixedPickupAddress; }
            set
            {
                fixedPickupAddress = value;
                RaisePropertyChanged(() => FixedPickupAddress);

            }
        }
        private AddressModel fixedDropAddress;
        public AddressModel FixedDropAddress
        {
            get { return fixedDropAddress; }
            set
            {
                fixedDropAddress = value;
                RaisePropertyChanged(() => FixedDropAddress);

            }
        }

        private async Task<bool> FixedAddress()
        {
            if (PickupAddressesList[pickupAddressId].Id == 0)
                FixedPickupAddress = await _googleAddress.CompleteAddress(PickupAddress, "");
            else
                FixedPickupAddress = GlobalSetting.Instance.CurrentCustomerModel.Addresses.Where(x => x.Id == PickupAddressesList[pickupAddressId].Id).FirstOrDefault();

            if (DropAddressesList[dropAddressId].Id == 0)
                FixedDropAddress = await _googleAddress.CompleteAddress(DropAddress, "");
            else
                FixedDropAddress = GlobalSetting.Instance.CurrentCustomerModel.Addresses.Where(x => x.Id == DropAddressesList[dropAddressId].Id).FirstOrDefault();
            
            FixedPickupAddress.Phone = PackageInfo.PickupPhone;
            FixedPickupAddress.Contact = PackageInfo.PickupContact;
            FixedDropAddress.Phone = PackageInfo.DeliveryPhone;
            FixedDropAddress.Contact = PackageInfo.DeliveryContact;


            if (FixedPickupAddress.Country  != "US" && FixedPickupAddress.Country != "United States")
            {
                await DialogService.ShowAlertAsync("Looks Like The Pickup Address Is Invalid", "Something Went Wrong !", "OK");
                return false;
            }

            if (FixedDropAddress.Country  != "US" && FixedDropAddress.Country  != "United States")
            {
                await DialogService.ShowAlertAsync("Looks Like The Drop Address Is Invalid", "Something Went Wrong !", "OK");
                return false;
            }


            return true;
        }
            private async Task<bool> ValidateInput()
        {
            IsBusy = true;
            var errorMsg = string.Empty;
                        

            if (SelectedPickupAddress != null)
            {
                if (PickupAddressesList[PickupAddressId].Id == 0)
                    if (string.IsNullOrWhiteSpace(PickupAddress))
                        errorMsg = "Must Have a PickUp Address !\n";
            }
            else
                errorMsg = "Must Have a PickUp Address !\n";


            if (SelectedDropAddress != null)
            {
                if (DropAddressesList[DropAddressId].Id == 0)
                    if (string.IsNullOrWhiteSpace(DropAddress))
                        errorMsg += "Must Have a Drop Address !\n";
            }              
            else
                errorMsg += "Must Have a Drop Address !\n"; 

            bool isvalidpi = ProfilePhotoImageMS != null ? true : false;
            if (!isvalidpi)
                errorMsg += "Need a Package Photo !\n"; 

            if (PackageInfo.PackageSizeId == 0)
                errorMsg += "Need a Package Size !\n"; 

            if (PackageInfo.PriorityTypeId == 0)
                errorMsg += "Need a Package Priority !\n";

            if (PackageInfo.Amount == 0)
                errorMsg += "Need Estimate Shipping Value !\n";

            if (PackageInfo.PriorityTypeId == 0)
                errorMsg += "Need Estimate Aproximate Weight !\n";

            if (!string.IsNullOrWhiteSpace(PickupAddress))
            {
                if (!ValidPhone.IsValidPhone(PackageInfo.PickupPhone))
                    errorMsg += "Need Pickup Phone !\n";

                if (string.IsNullOrWhiteSpace(PackageInfo.PickupContact))
                    errorMsg += "Need Pickup Contact !\n";
            }
            if (!string.IsNullOrWhiteSpace(DropAddress))
            {
                if (!ValidPhone.IsValidPhone(PackageInfo.DeliveryPhone))
                    errorMsg += "Need Delivery Phone !\n";

                if (string.IsNullOrWhiteSpace(PackageInfo.DeliveryContact))
                    errorMsg += "Need Delivery Contact !\n";
            }

            IsBusy = false;

            if (!string.IsNullOrEmpty(errorMsg)) 
                await DialogService.ShowAlertAsync(errorMsg, "Something Went Wrong !", "OK");  
        

            if (!string.IsNullOrEmpty(errorMsg))
                return false;

          return  await FixedAddress();

            //return true;
        }
        public ICommand CalcTotalCommand => new Command<Object>(async (key) => await CalcTotalFunction(key));
        private async Task CalcTotalFunction(Object obj)
        {
            if (await ValidateInput() == false)
                return;

            IsBusy = true;
            //calc distnace in miles             
            var distance = new Coordinates(FixedPickupAddress.Latitude, FixedPickupAddress.Longitude)
                   .DistanceTo(
                       new Coordinates(FixedDropAddress.Latitude, FixedDropAddress.Longitude),
                       UnitOfLength.Miles
                   );

            var authToken = GlobalSetting.Instance.AuthToken;
            CalculatedTotal = await _commons.CalcTotal(authToken, distance, PackageInfo.ShippingWeight, PackageInfo.PriorityTypeId, PackageInfo.PackageSizeId, PackageInfo.PromoCode, PackageInfo.ExtraCharge, PackageInfo.ExtraChargeNote);
  
            RaisePropertyChanged(() => CalculatedTotal);

            if (CalculatedTotal.AmountToCharge > 0)
            { 
                ShowCalculatedTotal = true; 
             
              //  PackageInfo.ExtraChargeNote = "after been seing...";

                ScrollView paypalShow = (ScrollView)obj;
                StackLayout messagesContent = (paypalShow.Content as StackLayout);
                var frame = messagesContent.Children.LastOrDefault();
                if (frame != null)
                    await paypalShow.ScrollToAsync(0, paypalShow.ContentSize.Height+50, true);

            }
            else
            {

                 await DialogService.ShowAlertAsync($"Something wrong !", "Total", "OK");
            }
            IsBusy = false;
        } 

        public ICommand SubmitCommand => new Command(async () => await SendFunction());
        private async Task SendFunction()
        {
            if (await ValidateInput() == false)
                return;

            // fix the addresses
          //  AddressModel FixedPickupAddress = new AddressModel();
         //   AddressModel FixedDropAddress = new AddressModel();

           


            var pickupPictureUri = string.Empty;


            var newPkg = new NewPackageModel
            {
                PickupStreet = FixedPickupAddress.Street,
                PickupCity = FixedPickupAddress.City,
                PickupState = FixedPickupAddress.State,
                PickupCountry = FixedPickupAddress.Country,
                PickupZipCode = FixedPickupAddress.ZipCode,
                PickupPhone = FixedPickupAddress.Phone,
                PickupContact = FixedPickupAddress.Contact,
                PickupLatitude = FixedPickupAddress.Latitude,
                PickupLongitude = FixedPickupAddress.Longitude,
                PickupAddressId = FixedPickupAddress.Id,

                DeliveryStreet = FixedDropAddress.Street,
                DeliveryCity = FixedDropAddress.City,
                DeliveryState = FixedDropAddress.State,
                DeliveryCountry = FixedDropAddress.Country,
                DeliveryZipCode = FixedDropAddress.ZipCode,
                DeliveryPhone = FixedDropAddress.Phone,
                DeliveryContact = FixedDropAddress.Contact,
                DeliveryLatitude = FixedDropAddress.Latitude,
                DeliveryLongitude = FixedDropAddress.Longitude,
                DropAddressId = FixedDropAddress.Id,

                PickupPictureUri = pickupPictureUri,
                NeedVanOrPickUp = PackageInfo.NeedVanOrPickUp,
                PackageSizeId= PackageInfo.PackageSizeId,
                PriorityTypeId= PackageInfo.PriorityTypeId,
                Amount= PackageInfo.Amount,
                ShippingWeight= PackageInfo.ShippingWeight,
                Weight = PackageInfo.ShippingWeight,
                Note= PackageInfo.Note,
                ExtraCharge= PackageInfo.ExtraCharge,
                ExtraChargeNote = PackageInfo.ExtraChargeNote,

                TotalCharge = (double)CalculatedTotal.AmountToCharge,
                Distance= CalculatedTotal.Distance,
                CustomerId =  GlobalSetting.Instance.CurrentCustomerModel.Id,


            };
            var authToken = GlobalSetting.Instance.AuthToken;
           var result= await _senderService.CreatePackageAsync(newPkg, authToken);
            if (!result.Contains("Info updated"))
                await DialogService.ShowAlertAsync(":( Something Wrong !", "Submited", "OK");
            else
            {
                await DialogService.ShowAlertAsync("Your Request for DriveDrop Have Been Submited, One Of Our Driver Will Contact You ASAP.", "Submited", "OK");
                IsEnabledScrollView = false;
            }
            IsBusy = false;
        }

        #region [profilePhotoImage]
        private string PersonalPhotoUri { set; get; }

        private Stream ProfilePhotoImageMS { set; get; }
        private ImageSource _profilePhotoImage = "Assets/profileicon.png";
        public ImageSource ProfilePhotoImage
        {
            get
            {
                return _profilePhotoImage;
            }
            set
            {
                _profilePhotoImage = value;

                RaisePropertyChanged(() => ProfilePhotoImage);
            }
        }

        private bool _loaded = false;
        public ICommand TakePhotoProfileCommand => new Command(async () => await TakePhotoProfileAsync());
        private async Task TakePhotoProfileAsync()
        {
            if (_loaded)
                return;
            _loaded = true;

            await CrossMedia.Current.Initialize();


            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DialogService.ShowAlertAsync("No Camera", ":( No camera avaialble.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                Directory = "godrivedrop",
                Name = "profilePhotoImage.jpg"
            });

            if (file == null)
            {
                return;
            }
            ProfilePhotoImage = file.Path;

            ProfilePhotoImageMS = file.GetStream();
            file.Dispose();
            _loaded = false;
        }

        public ICommand PickPhotoProfileCommand => new Command(async () => await PickPhotoProfileAsync());
        private async Task PickPhotoProfileAsync()
        {
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DialogService.ShowAlertAsync("Photos Not Supported", ":( Permission not granted to photos.", "OK");
                return;
            }
            var file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
            });


            if (file == null)
                return;

            ProfilePhotoImage = file.Path;

            ProfilePhotoImageMS = file.GetStream();

            file.Dispose();

        }



        #endregion


        public override async Task InitializeAsync(object navigationData)
        {
            IsBusy = true;


            Generic generic = new Generic { Id = 0, Name = "New Address" };

            PackageInfo = new NewPackageModel
            {
                PickupAddresses = GlobalSetting.Instance.CurrentCustomerModel.Addresses
            };

            PackageInfo.PickupAddresses = GlobalSetting.Instance.CurrentCustomerModel.Addresses.Where(x => x.TypeAddress == "home" || x.TypeAddress == "pickup").ToList();

            PackageInfo.DropAddresses = GlobalSetting.Instance.CurrentCustomerModel.Addresses.Where(x => x.TypeAddress == "drop").ToList();


            PickupAddressesList = GlobalSetting.Instance.CurrentCustomerModel.Addresses.Where(x => x.TypeAddress == "home" || x.TypeAddress == "pickup").Select(x => new Generic { Id = x.Id, Name = x.ToString() }).ToList();

            DropAddressesList = GlobalSetting.Instance.CurrentCustomerModel.Addresses.Where(x => x.TypeAddress == "drop").Select(x => new Generic { Id = x.Id, Name = x.ToString() }).ToList();

            PickupAddressesList.Add(generic);
            DropAddressesList.Add(generic);
 
            var authToken = GlobalSetting.Instance.AuthToken;
           
            PackageSizesList = await _commons.PackageSizes(token: authToken);
            PriorityTypesList = await _commons.PriorityTypes(token: authToken);

            //PackageInfo.ExtraChargeNote = "Note to the Driver, Pickup window, Special instructions, dimensions, if you're not inuding a picture, plase be descriptive.";
            //PackageInfo.Note= "Extra Charge Note.";

            ScrolledView = true;
            IsEnabledScrollView = true;

             IsBusy = false;

             

        }
    }
}
