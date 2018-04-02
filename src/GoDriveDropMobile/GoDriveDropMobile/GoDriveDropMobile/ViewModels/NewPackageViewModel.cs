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
        public ICommand CalcTotalCommand => new Command(async () => await CalcTotalFunction());
        private async Task CalcTotalFunction()
        {
            IsBusy = true;
            //calc distnace in miles
            var pAddress = PackageInfo.PickupAddresses[PackageInfo.PickupAddressId];
            var dAddress = PackageInfo.DropAddresses[PackageInfo.DropAddressId];

            var distance = new Coordinates(pAddress.Latitude, pAddress.Longitude)
                   .DistanceTo(
                       new Coordinates(dAddress.Latitude, dAddress.Longitude),
                       UnitOfLength.Miles
                   );

            var authToken = GlobalSetting.Instance.AuthToken;
            CalculatedTotal = await _commons.CalcTotal(authToken, distance, PackageInfo.ShippingWeight??0, PackageInfo.PriorityTypeId, PackageInfo.PackageSizeId, PackageInfo.PromoCode, PackageInfo.ExtraCharge??0, PackageInfo.ExtraChargeNote);
  
            RaisePropertyChanged(() => CalculatedTotal);

            await DialogService.ShowAlertAsync($"{distance} distance", "Total", "OK");
            PackageInfo.ExtraChargeNote = "after been seing...";
            RaisePropertyChanged(() => PackageInfo);

            ShowCalculatedTotal = true;
            IsBusy = false;
        }


        public ICommand SubmitCommand => new Command(async () => await SendFunction());
        private async Task SendFunction()
        {
            IsBusy = true;
            await DialogService.ShowAlertAsync( ":( No camera avaialble.", "Submited", "OK");

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
            IsBusy = false;
           Generic generic = new Generic { Id = 122, Name = "New Address" };

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

            PackageInfo.ExtraChargeNote = "Note to the Driver, Pickup window, Special instructions, dimensions, if you're not inuding a picture, plase be descriptive.";
            PackageInfo.Note= "Extra Charge Note.";

            IsBusy = false;

        }
    }
}
