using GoDriveDrop.Core.Helpers;
using GoDriveDrop.Core.Models;
using GoDriveDrop.Core.Models.Commons;
using GoDriveDrop.Core.Services.Common;
using GoDriveDrop.Core.Services.Driver;
using GoDriveDrop.Core.Services.Navigation;
using GoDriveDrop.Core.Services.RequestProvider;
using GoDriveDrop.Core.Services.User;
using GoDriveDrop.Core.Validations;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace GoDriveDrop.Core.ViewModels
{
    public class NewSenderViewModel : BaseViewModel
    {
        private readonly IRequestProvider _requestProvider;

        private IUserService _userService;
        private NewSenderModel _driver;
        private ISenderService _senderService;
        private IGoogleAddress _googleAddress;
        private ICommons _commons;


        private ValidatableObject<string> _user;

        private ValidatableObject<string> _lastName;
        private ValidatableObject<string> _firstName;

        private EmailRule<string> _userEmail;
        private ValidatableObject<string> _password;
        private ValidatableObject<string> _confirmPassword; 
        private ValidatableObject<string> _primaryPhone;
        private ValidatableObject<string> _phone;

        public NewSenderViewModel(
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
            _userEmail = new EmailRule<string>();
            _password = new ValidatableObject<string>();
            _confirmPassword = new ValidatableObject<string>();

            _lastName = new ValidatableObject<string>();
            _firstName = new ValidatableObject<string>();
            _primaryPhone = new ValidatableObject<string>();
            _phone = new ValidatableObject<string>();

            _user = new ValidatableObject<string>();
            AddValidations();
        }

        #region image names
        private string PersonalPhotoUri { set; get; }
        #endregion

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

            if (keyword.Length >= 3)
            {
                var authToken = Settings.AuthAccessToken;
                IEnumerable<String> predictions = await _googleAddress.AutoComplete(keyword, authToken);
                Items = new ObservableCollection<string>(predictions);
                if (Items.Count > 0)
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

        public NewSenderModel NewSender
        {
            get { return _driver; }
            set
            {
                _driver = value;
                RaisePropertyChanged(() => NewSender);
            }
        }

        public override async Task InitializeAsync(object navigationData)
        {
            IsBusy = true;
            await Task.Delay(10);
            var authToken = Settings.AuthAccessToken;  

            IsBusy = false;

        }

        #region Fields
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
        public int CustomerId { get; set; }

        public string FilePath { get; set; }

        public string IdentityGuid { get; set; }
        public string UserGuid { get; set; }

        public int CustomerTypeId { get; set; }

        public String PickupStreet { get; set; }
        public String PickupCity { get; set; }
        public String PickupState { get; set; }
        public String PickupCountry { get; set; }
        public String PickupZipCode { get; set; }
        public Double PickupLatitude { get; set; }
        public Double PickupLongitude { get; set; }

        #endregion
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

        private bool ValidateUserEmail()
        {           

          return _userEmail.Check(UserEmail.Value);
          
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
              _userEmail.ValidationMessage = "User Name is required.";
            _password.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Password is required." });
            _confirmPassword.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Configrmation Password is required." });
 
        }

        public ICommand ValidateFirstNameCommand => new Command(() => ValidateFirstName());
        public ICommand ValidateLastNameCommand => new Command(() => ValidateLastName());
        public ICommand ValidatePrimaryPhoneCommand => new Command(() => ValidatePrimaryPhone());
        public ICommand ValidatePhoneCommand => new Command(() => ValidatePhone());

        public ICommand ValidateUserEmailCommand => new Command(() => ValidateUserEmail());
        public ICommand ValidatePasswordCommand => new Command(() => ValidatePassword());
        public ICommand ValidateConfirmPasswordCommand => new Command(() => ValidateConfirmPassword());

        public ICommand SubmitCommand => new Command(async () => await SubmitAsync());


        public async Task SubmitAsync()
        {
            IsBusy = true;

            bool isValid = Validate();
            if (isValid)
            {
                if (Password.Value != ConfirmPassword.Value)
                {
                    await DialogService.ShowAlertAsync("Password and  Confirmation Password must be equal !", "User Name or Password Invalid!", "Ok");
                    IsBusy = false;
                    return;
                }
                 

                var valid = await _commons.ValidateUserName(UserEmail.Value);
                if (!valid.Contains("true"))
                {

                    await DialogService.ShowAlertAsync("Invalid User Name or Password", "User Name Issue", "Ok");

                    IsBusy = false;
                    return;
                }


                // fix the address
                AddressModel address = await _googleAddress.CompleteAddress(DefaultAddress, "");

                var sender = new NewSenderModel
                { 
                    PickupStreet = address.Street,
                    PickupCity = address.City,
                    PickupState = address.State,
                    PickupZipCode = address.ZipCode,
                    PickupCountry = address.Country,
                    PickupLatitude = address.Latitude,
                    PickupLongitude = address.Longitude, 

                    FirstName = FirstName.Value,
                    LastName = LastName.Value,
                    PrimaryPhone = PrimaryPhone.Value,
                    Phone = Phone.Value,
                     
                    UserEmail = UserEmail.Value,
                    Password = Password.Value,
                    ConfirmPassword = ConfirmPassword.Value,
                    
                    
                };

                bool isvalidpi = ProfilePhotoImageMS != null ? true : false;
                if (!isvalidpi  )
                {
                    await DialogService.ShowAlertAsync("Need a profile photo", "Saving data ProfilePhotoImage", "Ok");
                    IsBusy = false;
                    return;
                }

                if (string.IsNullOrEmpty(PersonalPhotoUri) && ProfilePhotoImageMS.CanRead)
                    sender.PersonalPhotoUri = await _commons.UploadImage(ProfilePhotoImageMS, "sender");
                else
                    sender.PersonalPhotoUri = PersonalPhotoUri;

                PersonalPhotoUri = sender.PersonalPhotoUri;

                if (string.IsNullOrWhiteSpace(sender.PersonalPhotoUri)  )
                {
                    IsBusy = false;
                    await DialogService.ShowAlertAsync("Unable to Save Photos !", "Verify Informations", "Ok");
                    return;
                }


                var response = "Info updated";
                try
                {
                   response = await _senderService.CreateSenderAsync(sender);


                }
                catch (Exception ex)
                {
                    response = ex.Message;
                     
                }
                if (response == "Info updated")
                {
                    await DialogService.ShowAlertAsync(" Shortly, you will receive an email with a link. Follow the link to verify your email address.", "Driver Account has Been Created", "Ok");
                  

                    var navigationService = ViewModelLocator.Resolve<INavigationService>();
                    await navigationService.InitializeAsync();
                }
                else
                {
                    await DialogService.ShowAlertAsync(response, "Error", "Ok");
                  
                }

            } 
            else
            {
                await DialogService.ShowAlertAsync("Something Wrong !", "Verify Informations", "Ok");


         
            }
            IsBusy = false;
        }


        private bool Validate()
        {
            bool isValidFirstName = ValidateFirstName();
            bool isValidLastName = ValidateLastName();
            bool isValiPP = ValidatePrimaryPhone();
            bool isValiP = ValidatePhone();
               bool isValiE = ValidateUserEmail();
            bool isValiPass = ValidatePassword();
            bool isValiPC = ValidateConfirmPassword(); 

            return isValidFirstName &&
                    isValidLastName &&
                    isValiPP &&
                    isValiP &&
                    isValiE &&
                    isValiPass &&
                    isValiPC; 
        }
    }

}
