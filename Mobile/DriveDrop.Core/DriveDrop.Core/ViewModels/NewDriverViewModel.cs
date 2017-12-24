

using DriveDrop.Core.Models.Drivers;
using DriveDrop.Core.Services.Driver;
using DriveDrop.Core.Services.User;
using DriveDrop.Core.Validations;
using DriveDrop.Core.ViewModels.Base;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace DriveDrop.Core.ViewModels
{
    public class NewDriverViewModel : ViewModelBase    
    {

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
        private ValidatableObject<string> _userEmail;
        private ValidatableObject<string> _password;
        private ValidatableObject<string> _confirmPassword;

        private bool _isValid;

        private IUserService _userService;
        private NewDriver _driver;
        private IDriverService _driverService;
        
        public NewDriverViewModel(
            IDriverService driverService,
            IUserService userService)
        {
            _driverService = driverService;
            _userService = userService;


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
            _userEmail = new ValidatableObject<string>();
            _password = new ValidatableObject<string>();
            _confirmPassword = new ValidatableObject<string>(); 

            AddValidations();
        }


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
            await Task.Delay(100);
            IsBusy = false;
             await base.InitializeAsync(navigationData);
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
            get { return _userEmail; }
            set
            {
                _userEmail = value;
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
        public ICommand ValidateUserEmailCommand => new Command(() => ValidateUserEmail());
        public ICommand ValidatePasswordCommand => new Command(() => ValidatePassword());
        public ICommand ValidateConfirmPasswordCommand => new Command(() => ValidateConfirmPassword());

        
        public ICommand SubmitCommand => new Command(async () => await SubmitAsync());
        
        public  async Task SubmitAsync()
        {
            await Task.Delay(100);
            //    await DialogService.ShowAlertAsync("Saving data", "Data save", "Ok");
            IsBusy = false;

            bool isValid =   Validate();
            if (isValid)
            {
                IsValid = true;
            }
            else
            {
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
                    isValiPC; 
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
            return _maxPackage.Validate();
        }
        private bool ValidatePickupRadius()
        {
            return _pickupRadius.Validate();
        }
        private bool ValidateDeliverRadius()
        {
            return _deliverRadius.Validate();
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
            return _userEmail.Validate();
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
            _userEmail.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "User Name is required." });
            _password.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Password is required." });
            _confirmPassword.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Configrmation Password is required." });




        }

    }
}
