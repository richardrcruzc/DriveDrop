using GoDriveDrop.Core.Models.Commons;
using GoDriveDrop.Core.Services.Common;
using GoDriveDrop.Core.Services.Driver;
using GoDriveDrop.Core.Validations;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace GoDriveDrop.Core.ViewModels
{
    public class PersonalInfoViewModel : BaseViewModel
    {
        private ISenderService _senderService;
        private ValidatableObject<string> _lastName;
        private ValidatableObject<string> _firstName;

        private EmailRule<string> _userEmail; 
        private ValidatableObject<string> _primaryPhone;
        private ValidatableObject<string> _phone;
        private ValidatableObject<string> _verificationId;
        private ValidatableObject<string> _user;
        private ICommons _commons;

        public PersonalInfoViewModel(ICommons commons)
        {
            _commons = commons;

            _lastName = new ValidatableObject<string>();
            _firstName = new ValidatableObject<string>();
            _primaryPhone = new ValidatableObject<string>();
            _phone = new ValidatableObject<string>();

            _userEmail = new EmailRule<string>();
            _user = new ValidatableObject<string>();
            _verificationId = new ValidatableObject<string>();

            _lastName.Value = GlobalSetting.Instance.CurrentCustomerModel.LastName;
            _firstName.Value = GlobalSetting.Instance.CurrentCustomerModel.FirstName;
            _primaryPhone.Value = GlobalSetting.Instance.CurrentCustomerModel.PrimaryPhone.CleanPhone();
            _phone.Value = GlobalSetting.Instance.CurrentCustomerModel.Phone.CleanPhone();
            _verificationId.Value = GlobalSetting.Instance.CurrentCustomerModel.VerificationId;
            _user.Value = GlobalSetting.Instance.CurrentCustomerModel.Email; 

            AddValidations();

            Title = "My Personal Information";
            _profilePhotoImage = GlobalSetting.Instance.CurrentCustomerModel.PersonalPhotoUrl;
        }


        #region [profilePhotoImage]
        private string PersonalPhotoUri { set; get; }

        private Stream ProfilePhotoImageMS { set; get; }
        private ImageSource _profilePhotoImage = GlobalSetting.Instance.CurrentCustomerModel.PersonalPhotoUrl;
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

        public ValidatableObject<string> VerificationId
        {

            get { return _verificationId; }
            set
            {

                _verificationId = value;
                RaisePropertyChanged(() => LastName);
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
        #region validation
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
        private bool ValidateVerificationId()
        {
            return _verificationId.Validate();
        }
        private bool ValidateUserEmail()
        {
          
           return _userEmail.Check(UserEmail.Value);

        }

        private void AddValidations()
        {
            _firstName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "First Name is required." });
            _lastName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Last Name is required." });

            _primaryPhone.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Contact Phone is required." });
            _phone.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Cell Phone is required." });
            _userEmail.ValidationMessage = "User Name is required.";
          
        }

        #endregion

        public ICommand ValidateFirstNameCommand => new Command(() => ValidateFirstName());
        public ICommand ValidateLastNameCommand => new Command(() => ValidateLastName());
        public ICommand ValidatePrimaryPhoneCommand => new Command(() => ValidatePrimaryPhone());
        public ICommand ValidatePhoneCommand => new Command(() => ValidatePhone());
        public ICommand ValidateVerificationIdCommand => new Command(() => ValidateVerificationId());

        public ICommand ValidateUserEmailCommand => new Command(() => ValidateUserEmail());


        public ICommand SubmitCommand => new Command(async () => await SubmitAsync());


        public async Task SubmitAsync()
        { 
            IsBusy = true;

            bool isValid = Validate();
            if (isValid)
            {    
                var user = new CustomerModel
                { 
                    FirstName = FirstName.Value,
                    LastName = LastName.Value,
                    PrimaryPhone = PrimaryPhone.Value,
                    Phone = Phone.Value,
                    Email= UserEmail.Value,
                    VerificationId=VerificationId.Value

                };

                //bool isvalidpi = ProfilePhotoImageMS != null ? true : false;
                //if (!isvalidpi)
                //{
                //    await DialogService.ShowAlertAsync("Need a profile photo", "Saving data ProfilePhotoImage", "Ok");
                //    IsBusy = false;
                //    return;
                //}

                if (ProfilePhotoImageMS!=null && ProfilePhotoImageMS.CanRead)
                {
                    user.PersonalPhotoUri = await _commons.UploadImage(ProfilePhotoImageMS, "sender"); 

                    if (string.IsNullOrWhiteSpace(user.PersonalPhotoUri))
                    {
                        IsBusy = false;
                        await DialogService.ShowAlertAsync("Unable to Save Photos !", "My Personal Information", "Ok");
                        return;
                    }
                }

                var response = "Info updated";
                try
                {
                    response = await _senderService.UpdateSenderAsync(user);


                }
                catch (Exception ex)
                {
                    response = ex.Message;

                }
                if (response == "Updated")
                {
                    await DialogService.ShowAlertAsync("Info updated", "My Personal Information", "Ok");


                    //var navigationService = ViewModelLocator.Resolve<INavigationService>();
                    //await navigationService.InitializeAsync();
                }
                else
                {
                    await DialogService.ShowAlertAsync(response, "My Personal Information", "Ok");

                }

            }
            else
            {
                await DialogService.ShowAlertAsync("Something Wrong !", "My Personal Information", "Ok");



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

            return isValidFirstName &&
                    isValidLastName &&
                    isValiPP &&
                    isValiP &&
                    isValiE  ;
        }
    }
}
