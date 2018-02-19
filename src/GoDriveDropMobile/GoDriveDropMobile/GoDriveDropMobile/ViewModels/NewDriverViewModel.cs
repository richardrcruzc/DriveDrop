using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GoDriveDrop.Core.Models;
using Xamarin.Forms;
using Plugin.Media;

namespace GoDriveDrop.Core.ViewModels
{
    public class NewDriverViewModel: BaseViewModel
    {
         

        public NewDriverViewModel()
        {
            Title = "Be a Driver";
            Icon = "serviceicon04";
        }

        private NewDriverModel _newDriverModel = new NewDriverModel();

        public NewDriverModel NewDriverModel
        {
            get { return _newDriverModel; }
            set
            {
                _newDriverModel = value;
                OnPropertyChanged("NewDriverModel"); // Add the OnPropertyChanged();
            }
        }

        #region [Commands]
        public ICommand SubmitCommand => new Command(async () => await SubmitAsync());

        public async Task SubmitAsync()
        {
            IsBusy = true;
            if (Password != ConfirmPassword)
            {
                await DialogService.ShowAlertAsync("Password and  Confirmation Password must be equal !", "User Name or Password Invalid!", "Ok");
                IsBusy = false;
                return;
            }
        }



        public Command CreateCommand // for ADD
        {
            get
            {
                return new Command(() => {
                    // for auto increment the id upon adding
                    //_newDriverModel.CustomerId = _getRealmInstance.All<NewDriverModel>().Count() + 1;
                    //_getRealmInstance.Write(() =>
                    //{
                    //    _getRealmInstance.Add(_customerDetails); // Add the whole set of details
                    //});
                });
            }
        }
        public Command UpdateCommand // For UPDATE
        {
            get
            {
                return new Command(() =>
                {
                    // instantiate to supply the new set of details
                    var customerDetailsUpdate = new NewDriverModel
                    {
                        CustomerId = _newDriverModel.CustomerId,
                        LastName = _newDriverModel.LastName,
                        FirstName = _newDriverModel.FirstName,                        
                        PrimaryPhone= _newDriverModel.PrimaryPhone,
                        Phone = _newDriverModel.Phone,
                    };

                    //_getRealmInstance.Write(() =>
                    //{
                    //    // when there's id match, the details will be replaced except by primary key
                    //    _getRealmInstance.Add(customerDetailsUpdate, update: true);
                    //})
                });
            }
        }
        public Command RemoveCommand
        {
            get
            {
                return new Command(() =>
                {
                    //// get the details with specific id
                    //var getAllCustomerDetailsById = _getRealmInstance.All<CustomerDetails>().First(x => x.CustomerId == _customerDetails.CustomerId);

                    //using (var transaction = _getRealmInstance.BeginWrite())
                    //{
                    //    // remove all details
                    //    _getRealmInstance.Remove(getAllCustomerDetailsById);
                    //    transaction.Commit();
                    //};
                });
            }
        }

        #endregion

        #region [Priver fields]

        string lastName = string.Empty;
        string LastName
        {
            get { return lastName; }
            set { lastName = value;
                OnPropertyChanged("LastName");
            }
        }
        string firstName = string.Empty;
        public string FirstName
        {
            get { return firstName; }
            set
            {
                firstName = value;
                OnPropertyChanged("FirstName");
            }
        }
        string primaryPhone = string.Empty;
        public string PrimaryPhone
        {
            get { return primaryPhone; }
            set
            {
                primaryPhone = value;
                OnPropertyChanged("PrimaryPhone");
            }
        }
        string phone = string.Empty;
        public string Phone
        {
            get { return phone; }
            set
            {
                phone = value;
                OnPropertyChanged("Phone");
            }
        }
        string maxPackage = string.Empty;
        public string MaxPackage
        {
            get { return maxPackage; }
            set
            {
                maxPackage = value;
                OnPropertyChanged("MaxPackage");
            }
        }

        string pickupRadius = string.Empty;
        public string PickupRadius
        {
            get { return pickupRadius; }
            set
            {
                pickupRadius = value;
                OnPropertyChanged("PickupRadius");
            }
        }

        string deliverRadius = string.Empty;
        public string DeliverRadius
        {
            get { return deliverRadius; }
            set
            {
                deliverRadius = value;
                OnPropertyChanged("DeliverRadius");
            }
        }

        string transportTypeId = string.Empty;
        public string TransportTypeId
        {
            get { return transportTypeId; }
            set
            {
                transportTypeId = value;
                OnPropertyChanged("TransportTypeId");
            }
        }

        string _vehicleMake = string.Empty;
        public  string VehicleMake
        {
            get { return _vehicleMake; }
            set
            {
                _vehicleMake = value;
                OnPropertyChanged("VehicleMake");
            }
        }
        string _vehicleModel = string.Empty;
        public  string VehicleModel
        {
            get { return _vehicleModel; }
            set
            {
                _vehicleModel = value;
                OnPropertyChanged("VehicleModel");
            }
        }
        string _vehicleColor = string.Empty;
        public  string VehicleColor
        {
            get { return _vehicleColor; }
            set
            {
                _vehicleColor = value;
                OnPropertyChanged("VehicleColor");
            }
        }

        string _vehicleYear = string.Empty;
        public  string  VehicleYear
        {
            get { return _vehicleYear; }
            set
            {
                _vehicleYear = value;
                OnPropertyChanged("VehicleYear");
            }
        }
        string _user = string.Empty;
        public  string UserEmail
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged("UserEmail");
}
        }
        string password = string.Empty;
        public  string  Password
        {
            get { return password; }
            set
            {
                password = value;
                OnPropertyChanged("Password");
            }
        }
        string _confirmPassword = string.Empty;
        public  string  ConfirmPassword
        {
            get { return _confirmPassword; }
            set
            {
                _confirmPassword = value;
                OnPropertyChanged("ConfirmPassword");
            }
        }
        #endregion

        #region Command and associated methods for SearchCommand
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

                OnPropertyChanged("Items");
            }
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (_searchText != value)
                {
                    _searchText = value ?? string.Empty;
                    OnPropertyChanged("SearchText");
                    // Perform the search
                    //if (SearchCommand.CanExecute(null))
                    //{
                    //    SearchCommand.Execute(null);
                    //}
                }
            }
        }
        public ICommand SearchCommand => new Command(async () => await DoSearchCommandAsync());
        private async Task DoSearchCommandAsync()
        {
            // Refresh the list, which will automatically apply the search text

            string keyword = SearchText;

            if (keyword.Length >= 3)
            {
                var authToken = string.Empty; // GloboSettings.AuthAccessToken;
                //IEnumerable<String> predictions = await _googleAddress.AutoComplete(keyword, authToken);
                //Items = new ObservableCollection<string>(predictions);
                //if (Items.Count > 0)
                //    IsListViewVisible = true;
            }
        }

        private string _selectedItem;
        public string SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;

                if (_selectedItem == null)
                    return;
                OnPropertyChanged("SelectedItem");
                //SomeCommand.Execute(_selectedItem);
                //  DialogService.ShowAlertAsync(_selectedItem, "Changes", "Ok");
                IsListViewVisible = false;
                IsDefaultAddressVisible = true;
                DefaultAddress = _selectedItem;
            }
        }
        public bool isListViewVisible;
        public bool IsListViewVisible
        {
            get
            {
                return isListViewVisible;
            }
            set
            {
                isListViewVisible = value;
                OnPropertyChanged("IsListViewVisible");
            }
        }

        private string _defaultAddress;
        public string DefaultAddress
        {
            get
            {
                return _defaultAddress;
            }
            set
            {
                _defaultAddress = value;
                OnPropertyChanged("DefaultAddress");
            }
        }

        public bool isDefaultAddressVisible;
        public bool IsDefaultAddressVisible
        {
            get
            {
                return isDefaultAddressVisible;
            }
            set
            {
                isDefaultAddressVisible = value;
                OnPropertyChanged("IsDefaultAddressVisible");
            }
        }
        #endregion


        #region [profilePhotoImage profilePhotoImage profilePhotoImage]

        #region [profilePhotoImage]
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

                OnPropertyChanged("ProfilePhotoImage");
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

        #region [LicensePhotoImage ]
        private Stream LicensePhotoImageS { set; get; }
        private ImageSource _licensePhotoImage = "Assets/profileicon.png";
        public ImageSource LicensePhotoImage
        {
            get
            {
                return _licensePhotoImage;
            }
            set
            {
                _licensePhotoImage = value;

                OnPropertyChanged("LicensePhotoImage");
            }
        }
        public ICommand TakePhotoLicenseCommand => new Command(async () => await TakePhotoLicenseAsync());
        private async Task TakePhotoLicenseAsync()
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DialogService.ShowAlertAsync("No Camera", ":( No camera avaialble.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                Directory = "godrivedrop",
                Name = "LicensePhotoImage.jpg"
            });

            if (file == null)
            {
                return;
            }
            LicensePhotoImage = file.Path;
            LicensePhotoImageS = file.GetStream();
            file.Dispose();
        }

        public ICommand PickPhotoLicenseCommand => new Command(async () => await PickPhotoLicenseAsync());
        private async Task PickPhotoLicenseAsync()
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

            LicensePhotoImage = file.Path;
            LicensePhotoImageS = file.GetStream();
            file.Dispose();

        }

        #endregion

        #region [VehiclePhotoImage]
        private Stream VehiclePhotoImages { set; get; }
        private ImageSource _vehiclePhotoImage = "Assets/profileicon.png";
        public ImageSource VehiclePhotoImage
        {
            get
            {
                return _vehiclePhotoImage;
            }
            set
            {
                _vehiclePhotoImage = value;

                OnPropertyChanged("VehiclePhotoImage");
            }
        }
        public ICommand TakePhotoVehicleCommand => new Command(async () => await TakePhotoVehicleAsync());
        private async Task TakePhotoVehicleAsync()
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
             //   await DialogService.ShowAlertAsync("No Camera", ":( No camera avaialble.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                Directory = "godrivedrop",
                Name = "VehiclePhotoImage.jpg"
            });

            if (file == null)
            {
                return;
            }
            VehiclePhotoImage = file.Path;
            VehiclePhotoImages = file.GetStream();
            file.Dispose();
        }

        public ICommand PickPhotoVehicleCommand => new Command(async () => await PickPhotoVehicleAsync());
        private async Task PickPhotoVehicleAsync()
        {
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                //await DialogService.ShowAlertAsync("Photos Not Supported", ":( Permission not granted to photos.", "OK");
                return;
            }
            var file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
            });


            if (file == null)
                return;

            VehiclePhotoImage = file.Path;
            VehiclePhotoImages = file.GetStream();
            file.Dispose();

        }

        #endregion

        #region [VehiclePhotoImage]
        private Stream ProofPhotoImages { set; get; }
        private ImageSource _proofPhotoImage = "Assets/profileicon.png";
        public ImageSource ProofPhotoImage
        {
            get
            {
                return _proofPhotoImage;
            }
            set
            {
                _proofPhotoImage = value;

                OnPropertyChanged("ProofPhotoImage");
            }
        }
        public ICommand TakePhotoProofCommand => new Command(async () => await TakePhotoProofAsync());
        private async Task TakePhotoProofAsync()
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
               // await DialogService.ShowAlertAsync("No Camera", ":( No camera avaialble.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                Directory = "godrivedrop",
                Name = "ProofPhotoImage.jpg"
            });

            if (file == null)
            {
                return;
            }
            ProofPhotoImage = file.Path;
            ProofPhotoImages = file.GetStream();
            file.Dispose();
        }

        public ICommand PickPhotoProofCommand => new Command(async () => await PickPhotoProofAsync());
        private async Task PickPhotoProofAsync()
        {
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
               // await DialogService.ShowAlertAsync("Photos Not Supported", ":( Permission not granted to photos.", "OK");
                return;
            }
            var file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
            });


            if (file == null)
                return;

            ProofPhotoImage = file.Path;
            ProofPhotoImages = file.GetStream();
            file.Dispose();

        }

        #endregion


        #endregion



    }
}
