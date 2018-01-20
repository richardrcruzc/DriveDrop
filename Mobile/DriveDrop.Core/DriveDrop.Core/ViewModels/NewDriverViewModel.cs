
using Plugin.Media;
using DriveDrop.Core.Models.Drivers;
using DriveDrop.Core.Services.Driver;
using DriveDrop.Core.Services.User;
using DriveDrop.Core.Validations;
using DriveDrop.Core.ViewModels.Base;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using DriveDrop.Core.Models.Commons;
using System.Collections.Generic; 
using DriveDrop.Core.Services.RequestProvider;
using System;
using DriveDrop.Core.Services.Common;
using DriveDrop.Core.Helpers; 
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using PCLStorage;

namespace DriveDrop.Core.ViewModels
{
    public class NewDriverViewModel : ViewModelBase    
    {
        const string emailRegex = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
        @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";

        private readonly IRequestProvider _requestProvider;
        //  private Geocoder geoCoder;

        private ValidatableObject<string> _user;
        
        private ValidatableObject<string> _lastName;
        private ValidatableObject<string> _firstName;
        private ValidatableObject<string> _primaryPhone;
        private ValidatableObject<string> _phone;
        private ValidatableObject<int> _transportTypeId;
        private ValidatableObject<int> _maxPackage;
        private ValidatableObject<int> _pickupRadius;
        private ValidatableObject<int> _deliverRadius;
        private ValidatableObject<string> _vehicleMake;
        private ValidatableObject<string> _vehicleModel;
        private ValidatableObject<string> _vehicleColor;
        private ValidatableObject<string> _vehicleYear;
        private EmailRule<string> _userEmail;
        private ValidatableObject<string> _password;
        private ValidatableObject<string> _confirmPassword;
                
        private bool _isValid;

        private IUserService _userService;
        private NewDriver _driver;
        private IDriverService _driverService;
        private IGoogleAddress _googleAddress;
        private ICommons _commons;
        

        public NewDriverViewModel(
            ICommons commons,
            IGoogleAddress googleAddress,
            IRequestProvider requestProvider,
            IDriverService driverService,
            IUserService userService)
        {
            _commons = commons;
             _googleAddress = googleAddress;
            _driverService = driverService;
            _userService = userService;

            _requestProvider = requestProvider;

            
            _lastName = new ValidatableObject<string>();
            _firstName = new ValidatableObject<string>();

            _primaryPhone = new ValidatableObject<string>();
            _phone = new ValidatableObject<string>();
            _transportTypeId = new ValidatableObject<int>();
            _maxPackage = new ValidatableObject<int>();
            _pickupRadius = new ValidatableObject<int>();
            _deliverRadius = new ValidatableObject<int>();
            _vehicleMake = new ValidatableObject<string>();
            _vehicleModel = new ValidatableObject<string>();
            _vehicleColor = new ValidatableObject<string>();
            _vehicleYear = new ValidatableObject<string>();
            _userEmail = new EmailRule<string>();
            _password = new ValidatableObject<string>();
            _confirmPassword = new ValidatableObject<string>();


            _user = new ValidatableObject<string>();
            AddValidations();


           

        }
         
        #region Pickers
        private int selectedIndex;
        public int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
            set
            {
                selectedIndex = value;

                RaisePropertyChanged(() => selectedIndex);
            }
        }
        private IEnumerable<Generic> _vehicleTypes;
        public IEnumerable<Generic> VehicleTypesList
        {
            get
            {
                return _vehicleTypes;
            }
            set
            {
                _vehicleTypes = value;

                RaisePropertyChanged(() => VehicleTypesList);
            }
        }
        #endregion

        #region [profilePhotoImage profilePhotoImage profilePhotoImage]

        #region [profilePhotoImage]
        private Stream ProfilePhotoImageMS { set; get; }
        private ImageSource _profilePhotoImage = "Assets/noimage.png";
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

        #region [LicensePhotoImage ]
        private Stream LicensePhotoImageS { set; get; } 
        private ImageSource _licensePhotoImage = "Assets/noimage.png";
        public ImageSource LicensePhotoImage
        {
            get
            {
                return _licensePhotoImage;
            }
            set
            {
                _licensePhotoImage = value;

                RaisePropertyChanged(() => LicensePhotoImage);
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
        private ImageSource _vehiclePhotoImage = "Assets/noimage.png";
        public ImageSource VehiclePhotoImage
        {
            get
            {
                return _vehiclePhotoImage;
            }
            set
            {
                _vehiclePhotoImage = value;

                RaisePropertyChanged(() => VehiclePhotoImage);
            }
        }
        public ICommand TakePhotoVehicleCommand => new Command(async () => await TakePhotoVehicleAsync());
        private async Task TakePhotoVehicleAsync()
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
                await DialogService.ShowAlertAsync("Photos Not Supported", ":( Permission not granted to photos.", "OK");
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
        private ImageSource _proofPhotoImage = "Assets/noimage.png";
        public ImageSource ProofPhotoImage
        {
            get
            {
                return _proofPhotoImage;
            }
            set
            {
                _proofPhotoImage = value;

                RaisePropertyChanged(() => ProofPhotoImage);
            }
        }
        public ICommand TakePhotoProofCommand => new Command(async () => await TakePhotoProofAsync());
        private async Task TakePhotoProofAsync()
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
                await DialogService.ShowAlertAsync("Photos Not Supported", ":( Permission not granted to photos.", "OK");
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

                RaisePropertyChanged(() => Items);
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
                      RaisePropertyChanged(() => SearchText);
                    // Perform the search
                    if (SearchCommand.CanExecute(null))
                    {
                        SearchCommand.Execute(null);
                    }
                }
            }
        }
       
       public ICommand SearchCommand => new Command(async () => await DoSearchCommandAsync()); 
        private async Task DoSearchCommandAsync()
        {
            // Refresh the list, which will automatically apply the search text
 
            string keyword = SearchText;
           
              if (keyword.Length >= 3){
                var authToken = Settings.AuthAccessToken;
                IEnumerable<String> predictions = await _googleAddress.AutoComplete(keyword, authToken);
                Items = new ObservableCollection<string>(predictions);
                if(Items.Count>0)
                    IsListViewVisible = true;
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
                RaisePropertyChanged(() => SelectedItem);
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
                RaisePropertyChanged(() => IsListViewVisible);
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
                RaisePropertyChanged(() => DefaultAddress);
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
                RaisePropertyChanged(() => IsDefaultAddressVisible);
            }
        }
        #endregion


        public NewDriver NewDriver
        {
            get { return _driver; }
            set
            {
                _driver = value;
                RaisePropertyChanged(() => NewDriver);
            }
        }


        public override async Task InitializeAsync(object navigationData)
        {
            IsBusy = false;

            var authToken = Settings.AuthAccessToken;
            VehicleTypesList =await  _commons.VehicleTypes( token: authToken);



            IsBusy = false;
            
        }
        
        public ValidatableObject<string> LastName
        {
           
            get { return _lastName; }
            set
            {

                _lastName = value;
                RaisePropertyChanged(() => LastName);
            }
        }
       
        public ValidatableObject<string> FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                RaisePropertyChanged(() => FirstName);
            }
        }
         
        public ValidatableObject<string> PrimaryPhone
        {
            get { return _primaryPhone; }
            set
            {
                _primaryPhone = value;
                RaisePropertyChanged(() => PrimaryPhone);
            }
        }

        public ValidatableObject<string> Phone
        {
            get { return _phone; }
            set
            {
                _phone = value;
                RaisePropertyChanged(() => Phone);
            }
        }

        public string DeliveryStreet { get; set; }
        public string DeliveryCity { get; set; }
        public string DeliveryState { get; set; }
        public string DeliveryCountry { get; set; }
        public string DeliveryZipCode { get; set; }
         
        public ValidatableObject<int> MaxPackage  
        {
            get { return _maxPackage; }
            set
            {
                _maxPackage = value;
                RaisePropertyChanged(() => MaxPackage);
            }
        }
         
        public ValidatableObject<int> PickupRadius
        {
            get { return _pickupRadius; }
            set
            {
                _pickupRadius = value;
                RaisePropertyChanged(() => PickupRadius);
            }
        }
         
        public ValidatableObject<int> DeliverRadius
        {
            get { return _deliverRadius; }
            set
            {
               _deliverRadius = value;
                RaisePropertyChanged(() => DeliverRadius);
            }
        } 
        public ValidatableObject<int> TransportTypeId
        {
            get { return _transportTypeId; }
            set
            {
                _transportTypeId = value;
                RaisePropertyChanged(() => TransportTypeId);
            }
        } 
        public ValidatableObject<string> VehicleMake
        {
            get { return _vehicleMake; }
            set
            {
                _vehicleMake = value;
                RaisePropertyChanged(() => VehicleMake);
            }
        } 
        public ValidatableObject<string> VehicleModel
        {
            get { return _vehicleModel; }
            set
            {
                _vehicleModel = value;
                RaisePropertyChanged(() => VehicleModel);
            }
        } 
        public ValidatableObject<string> VehicleColor
        {
            get { return _vehicleColor; }
            set
            {
                _vehicleColor = value;
                RaisePropertyChanged(() => VehicleColor);
            }
        } 
        public ValidatableObject<string> VehicleYear
        {
            get { return _vehicleYear; }
            set
            {
                _vehicleYear = value;
                RaisePropertyChanged(() => VehicleYear);
            }
        }
        
        public ValidatableObject<string> UserEmail
        {
            get { return _user; }
            set
            {
                _user = value;
                RaisePropertyChanged(() => UserEmail);
            }
        }
        public ValidatableObject<string> Password
        {
            get { return _password; }
            set
            {
                _password = value;
                RaisePropertyChanged(() => Password);
            }
        }
        public ValidatableObject<string> ConfirmPassword
        {
            get { return _confirmPassword; }
            set
            {
                _confirmPassword = value;
                RaisePropertyChanged(() => ConfirmPassword);
            }
        }


        public bool IsValid
        {
            get
            {
                return _isValid;
            }
            set
            {
                _isValid = value;
                RaisePropertyChanged(() => IsValid);
            }
        }

        public ICommand ValidateFirstNameCommand => new Command(() => ValidateFirstName());
        public ICommand ValidateLastNameCommand => new Command(() => ValidateLastName());
        public ICommand ValidatePrimaryPhoneCommand => new Command(() => ValidatePrimaryPhone());
        public ICommand ValidatePhoneCommand => new Command(() => ValidatePhone());

        public ICommand ValidateMaxPackageCommand => new Command(() => ValidateMaxPackage());
        public ICommand ValidatePickupRadiusCommand => new Command(() => ValidatePickupRadius());
        public ICommand ValidateDeliverRadiusCommand => new Command(() => ValidateDeliverRadius());
        public ICommand ValidateTransportTypeIdCommand => new Command(() => ValidateTransportTypeId());
        public ICommand ValidateVehicleMakeCommand => new Command(() => ValidateVehicleMake());
        

        public ICommand ValidateUserEmailCommand => new Command(() => ValidateUserEmail());
        public ICommand ValidatePasswordCommand => new Command(() => ValidatePassword());
        public ICommand ValidateConfirmPasswordCommand => new Command(() => ValidateConfirmPassword());


        public ICommand SubmitCommand => new Command(async () => await SubmitAsync());

        public ICommand OnSearchTextChangedCommand => new Command(async () => await DialogService.ShowAlertAsync("Chamges", "Changes", "Ok"));

        //public ICommand takePhotoCommand => new Command(async () => await DialogService.ShowAlertAsync("Phonto", "Phonto", "Ok"));



      

        public async Task SubmitAsync()
        {
          // await _driverService.CreateDriverAsync(new NewDriver());

            IsBusy = true;

            bool isValid =   Validate();
            if (isValid)
            {
                if (Password.Value != ConfirmPassword.Value)
                {
                    await DialogService.ShowAlertAsync("Password and  Confirmation Password must be equal !", "User Name or Password Invalid!", "Ok");
                    IsBusy = false;
                    return;
                }


                bool isvalidpi = ProfilePhotoImageMS != null ? true : false;
                bool isvalidlpi = LicensePhotoImageS != null ? true : false;
                bool isvalidvpi = VehiclePhotoImages != null ? true : false;
                bool isvalidppi = ProofPhotoImages != null ? true : false;

                if (!isvalidpi || !isvalidlpi || !isvalidvpi || !isvalidppi)
                {
                    await DialogService.ShowAlertAsync("Need a profile or driver license or vehicle or proof of insurance photo", "Saving data ProfilePhotoImage", "Ok");
                    IsBusy = false;
                    return;
                }

                var vTypes = VehicleTypesList.ToList();
                 

                var valid =await _commons.ValidateUserName(UserEmail.Value );
                if (valid.Contains("duplicate"))
                {

                    await DialogService.ShowAlertAsync("Invalid User Name or Password", "User Name Issue", "Ok");

                    IsBusy = false;
                    return;
                }


                var personalPhotoUri = await _commons.UploadImage(ProfilePhotoImageMS, "driver" );
                var vehiclePhotoUri = await _commons.UploadImage(VehiclePhotoImages, "driver" );
                var driverLincensePictureUri = await _commons.UploadImage(LicensePhotoImageS, "driver" );
                var insurancePhotoUri = await _commons.UploadImage(ProofPhotoImages, "driver" );


                if (string.IsNullOrWhiteSpace(personalPhotoUri) ||
                    string.IsNullOrWhiteSpace(vehiclePhotoUri) ||
                    string.IsNullOrWhiteSpace(driverLincensePictureUri) ||
                    string.IsNullOrWhiteSpace(insurancePhotoUri))
                {
                    IsBusy = false;
                    await DialogService.ShowAlertAsync("Unable to Save Photos !", "Verify Informations", "Ok");
                    return;
                }

                // fix the address
                AddressModel address = await _googleAddress.CompleteAddress(DefaultAddress, "");

                var driver = new NewDriver
                {
                    DeliveryStreet =address.Street,
                    DeliveryCity= address.City,
                    DeliveryState = address.State,
                    DeliveryZipCode = address.ZipCode,
                    DeliveryCountry = address.Country,
                    DeliveryLatitude = address.Latitude,
                    DeliveryLongitude = address.Longitude,


                    FirstName =  FirstName.Value,
                    LastName =LastName.Value,
                    PrimaryPhone = PrimaryPhone.Value,
                    Phone = Phone.Value,
                    
                    MaxPackage = MaxPackage.Value ,
                    DeliverRadius =DeliverRadius.Value,
                    PickupRadius = PickupRadius.Value,
                    TransportTypeId = vTypes[SelectedIndex].Id,
                    VehicleMake = VehicleMake.Value,
                    VehicleModel =  VehicleModel.Value,
                    VehicleColor = VehicleColor.Value,
                    VehicleYear = VehicleYear.Value,

                    PersonalPhotoUri = personalPhotoUri,
                    VehiclePhotoUri = vehiclePhotoUri,
                    DriverLincensePictureUri = driverLincensePictureUri,
                    InsurancePhotoUri = insurancePhotoUri,

                    UserEmail = UserEmail.Value,
                    Password = Password.Value, 
                    ConfirmPassword= ConfirmPassword.Value,
                    Email = UserEmail.Value,
                };
                var response =  "Info updated";
                try
                {
                      response = await _driverService.CreateDriverAsync(driver);

                    
                }
                catch (Exception ex)
                {
                    response = ex.Message;

                    IsValid = false;
                }
                if (response == "Info updated")
                {
                    await DialogService.ShowAlertAsync(" Shortly, you will receive an email with a link. Follow the link to verify your email address.", "Driver Account has Been Created", "Ok");
                    IsValid = true;
                }
                else
                {
                    await DialogService.ShowAlertAsync(response, "Error", "Ok");
                    IsValid = false;
                }


            }
            else
            {
                await DialogService.ShowAlertAsync("Something Wrong !", "Verify Informations", "Ok");


              
                IsValid = false;
            }
            IsBusy = false;
            
        }


        private bool Validate()
        {
            bool isValidFirstName = ValidateFirstName();
            bool isValidLastName = ValidateLastName();
            bool isValiPP = ValidatePrimaryPhone();
            bool isValiP = ValidatePhone();
            bool isValiVM = ValidateVehicleMake();
            bool isValiVEM = ValidateVehicleModel();
            bool isValiVC= ValidateVehicleColor();
            bool isValiVY = ValidateVehicleYear();
            bool isValiE = ValidateUserEmail();
            bool isValiPass = ValidatePassword();
            bool isValiPC = ValidateConfirmPassword();

            bool isvalidpi = ValidateProfilePhotoImage();
            bool isvalidlpi = ValidatelicensePhotoImage();
            bool isvalidvpi = ValidatevehiclePhotoImage();
            bool isvalidppi = ValidateProofPhotoImage();


            return isValidFirstName &&
                    isValidLastName &&
                    isValiPP &&
                    isValiP &&
                    isValiVM &&
                    isValiVEM &&
                    isValiVC &&
                    isValiVC &&
                    isValiVY &&
                    isValiE &&
                    isValiPass &&
                    isValiPC &&
                    isvalidpi &&
                    isvalidlpi &&
                    isvalidvpi &&
                    isvalidppi; 
        }
        private bool ValidateProfilePhotoImage()
        {
            if (string.IsNullOrEmpty(ProfilePhotoImage.ToString()) || ProfilePhotoImage.ToString().Trim() == "Assets/noimage.png")
                return false;
            return true;
        }
        private bool ValidatelicensePhotoImage()
        {
            if (string.IsNullOrEmpty(LicensePhotoImage.ToString()) || LicensePhotoImage.ToString().Trim() == "Assets/noimage.png")
                return false;
            return true;
        }
        private bool ValidatevehiclePhotoImage()
        {
            if (string.IsNullOrEmpty(VehiclePhotoImage.ToString()) || VehiclePhotoImage.ToString().Trim() == "Assets/noimage.png")
                return false;
            return true;
        }
        private bool ValidateProofPhotoImage()
        {
            if (string.IsNullOrEmpty(ProofPhotoImage.ToString()) || ProofPhotoImage.ToString().Trim() == "Assets/noimage.png")
                return false;
            return true;
        }
        private bool ValidateFirstName()
        {
            return _firstName.Validate();
        }
        private bool ValidateLastName()
        {
            return _lastName.Validate();
        } 

        private bool ValidatePrimaryPhone()
        {
            return _primaryPhone.Validate();
        }
        private bool ValidatePhone()
        {
            return _phone.Validate();
        }

        private bool ValidateMaxPackage()
        {
            return _maxPackage.Value > 0;  // _maxPackage.Validate();
        }
        private bool ValidatePickupRadius()
        {
            return _pickupRadius.Value > 0; // _pickupRadius.Validate();
        }
        private bool ValidateDeliverRadius()
        {
            return _deliverRadius.Value > 0; // _deliverRadius.Validate();
        }
        private bool ValidateTransportTypeId()
        {
            return _transportTypeId.Value>0; // _transportTypeId.Validate();
        }
        
        private bool ValidateVehicleMake()
        {
            return _vehicleMake.Validate();
        }
        private bool ValidateVehicleModel()
        {
            return _vehicleModel.Validate();
        }
        private bool ValidateVehicleColor()
        {
            return _vehicleColor.Validate();
        }
        private bool ValidateVehicleYear()
        {
            return _vehicleYear.Validate();
        }
        private bool ValidateUserEmail()
        {
            return true;

          //  return _userEmail.Check(UserEmail);
        }
        private bool ValidatePassword()
        {
            return _password.Validate();
        }
        private bool ValidateConfirmPassword()
        {
            return _confirmPassword.Validate();
        }


        private void AddValidations()
        {
            _firstName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "First Name is required." });
            _lastName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Last Name is required." });

            _primaryPhone.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Contact Phone is required." });
            _phone.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Cell Phone is required." });
            _transportTypeId.Validations.Add(new IsNotNullOrEmptyRule<int> { ValidationMessage = "Transportation is required." });
            _maxPackage.Validations.Add(new  IsNotNullOrEmptyRule<int> { ValidationMessage = "Maximum Package to Pickup is required." });
            _pickupRadius.Validations.Add(new IsNotNullOrEmptyRule<int> { ValidationMessage = "Maximum Drive Pickup Radius in Miles is required." });
            _deliverRadius.Validations.Add(new IsNotNullOrEmptyRule<int> { ValidationMessage = "Maximum Drive Deliver Package Radius  in Miles is required." });
            _vehicleMake.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Vehicle Make is required." });
            _vehicleModel.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Vehicle Model is required." });
            _vehicleColor.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Vehicle Color is required." });
            _vehicleYear.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Vehicle Year is required." });
            _userEmail.ValidationMessage= "User Name is required." ;
            _password.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Password is required." });
            _confirmPassword.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Configrmation Password is required." });




        }

    }
}
