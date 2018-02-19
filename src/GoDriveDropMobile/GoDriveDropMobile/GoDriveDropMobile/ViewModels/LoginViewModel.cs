using GoDriveDrop.Core.Controls;
using GoDriveDrop.Core.Identity;
using GoDriveDrop.Core.Services;
using GoDriveDrop.Core.Services.OpenUrl;
using GoDriveDrop.Core.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace GoDriveDrop.Core.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private IOpenUrlService _openUrlService;
        private IIdentityService _identityService;


        //Dictionary<int, NavigationPage> Pages { get; set; }
        //public Action DisplayInvalidLoginPrompt;
        //public Action DisplayInvalidActionPrompt;

        private string email;
        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                OnPropertyChanged();
            }
        }
        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                OnPropertyChanged();
            }
        }
        private string erroMessage;
        public string ErroMessage
        {
            get { return erroMessage; }
            set
            {
                erroMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand SubmitCommand => new Command(async () => await OnSubmitAsync());
        public ICommand BeDriverCommand => new Command(async () => await BeDriverOnSubmit());
        public ICommand BSenderCommand => new Command(async () => await BeSenderOnSubmit());

         
        public LoginViewModel(IOpenUrlService openUrlService, IIdentityService identityService)
        {
            Title = "Login";
            Icon = "serviceicon04";
            _openUrlService = openUrlService;
         _identityService= identityService;
           

            // Set the NavigationBar TextColor and Background Color
           
        }
        public async Task BeDriverOnSubmit()
        {

            //await NavigationService.NavigateToAsync<NewDriverViewModel>(new TabParameter { TabIndex = 1 });
            await NavigationService.NavigateToAsync<NewDriverViewModel>();
            await NavigationService.RemoveLastFromBackStackAsync();



            //var mainPage = new NavigationPage(new NewDriverPage()
            //{
            //    Title = "Be a Driver",
            //    Icon = "serviceicon04"
            //});
            //{
            //    // Set the NavigationBar TextColor and Background Color
            //    BarBackgroundColor = Color.FromHex("#4C5678"),
            //    BarTextColor = Color.White
            //};
            //// Declare our DependencyService Interface
            //var navService = DependencyService.Get<IGoNavService>() as GoNavService;
            //navService.navigation = mainPage.Navigation;

            //// Register our View Model Mappings between our ViewModels and Views (Pages)
            //navService.RegisterViewMapping(typeof(LoginViewModel), typeof(LoginPage));


            // Application.Current.MainPage = mainPage;
        }
        public async Task BeSenderOnSubmit()
        {
            await DialogService.ShowAlertAsync("Invalid Option!", "Login", "Ok");
            //Pages.Add(0, new GoDriveDropNavigationPage(new AboutPage()));
            //var mainPage = new NavigationPage(new AboutPage()
            //{
            //    Title = "About",
            //    Icon = "serviceicon04"
            //});
            //Application.Current.MainPage = mainPage;
        }
  
        public async  Task OnSubmitAsync()
        { 

            if (email != "1" || password != "2")
            {
                //DisplayInvalidLoginPrompt();

                await DialogService.ShowAlertAsync("Invalid Login!", "Login", "Ok");
                return;
            }

            var title = "goDriveDrop - iOS";
            if (Device.RuntimePlatform == Device.iOS)
                title = "goDriveDrop - iOS";
            if (Device.RuntimePlatform == Device.Android)
                title = "goDriveDrop - Android";
            if (Device.RuntimePlatform == Device.UWP)
                title = "goDriveDrop - UWP";
            if (Device.RuntimePlatform == Device.WinPhone)
                title = "goDriveDrop - WinPhone";


            //await NavigationService.NavigateToAsync<RootPage>()


            // Set our Walks Page to be the root page of our application
            var mainPage = new NavigationPage(new RootPage()
            {
                Title = title, 
            });


            // Set the NavigationBar TextColor and Background Color
             mainPage.BarBackgroundColor = Color.FromHex("#223669");
             mainPage.BarTextColor = Color.White;

            //// Declare our DependencyService Interface
            //  var navService = DependencyService.Get<IGoNavService>() as GoNavService;
            // navService.navigation = mainPage.Navigation;

            //// Register our View Model Mappings between our ViewModels and Views (Pages)
            // navService.RegisterViewMapping(typeof(LoginViewModel), typeof(LoginPage));
            // navService.RegisterViewMapping(typeof(NewDriverViewModel), typeof(NewDriverPage));
            //navService.RegisterViewMapping(typeof(WalksPageViewModel), typeof(WalksPage));
            //navService.RegisterViewMapping(typeof(WalkEntryViewModel), typeof(WalkEntryPage));
            //navService.RegisterViewMapping(typeof(WalksTrailViewModel), typeof(WalkTrailPage));
            //navService.RegisterViewMapping(typeof(DistTravelledViewModel), typeof(DistanceTravelledPage));

            // Navigate to our Walks Main Page
              App.Current.MainPage = mainPage;
        }
    }
}
