﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using GoDriveDrop.Core.Services;
using GoDriveDrop.Core.Services.Dependency;
using GoDriveDrop.Core.Services.Location;
using GoDriveDrop.Core.Services.Navigation;
using GoDriveDrop.Core.ViewModels;
using GoDriveDrop.Core.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace GoDriveDrop.Core
{
    public partial class App : Application
    {
       
        public bool UseMockServices { get; set; }
        public App()
        {
            InitializeComponent();

            InitApp();
            if (Device.RuntimePlatform == Device.UWP)
            {
                InitNavigation();
            }
            // Check the Device Target OS Platform
            //if ( Device.RuntimePlatform == Device.Android)
            //{
            //    // Set the root page of your application
            //    MainPage = new  SplashPage();
            //}
            //else if (Device.RuntimePlatform == Device.iOS)
            //{ 

            // Set our Walks Page to be the root page of our application
            //var mainPage = new NavigationPage(new LoginPage()
            //{
            //    Title = "goDriveDrop- iOS",
            //});

            //var mainPage = new NavigationPage(new MainPage()
            //{
            //    Title = "goDriveDrop - iOS",
            //});

            // Set the NavigationBar TextColor and Background Color
            //mainPage.BarBackgroundColor = Color.FromHex("#223669");
            //mainPage.BarTextColor = Color.White;

            //// Declare our DependencyService Interface
            //var navService = DependencyService.Get<IWalkNavService>() as WalkNavService;
            //navService.navigation = mainPage.Navigation;

            //// Register our View Model Mappings between our ViewModels and Views (Pages)
            //navService.RegisterViewMapping(typeof(WalksPageViewModel), typeof(WalksPage));
            //navService.RegisterViewMapping(typeof(WalkEntryViewModel), typeof(WalkEntryPage));
            //navService.RegisterViewMapping(typeof(WalksTrailViewModel), typeof(WalkTrailPage));
            //navService.RegisterViewMapping(typeof(DistTravelledViewModel), typeof(DistanceTravelledPage));

            // Navigate to our Walks Main Page
            //App.Current.MainPage = mainPage;
            //  }
            //MainPage = new MainPage();
        }
        private void InitApp()
        { 
            ViewModelLocator.RegisterDependencies();
        }
        private Task InitNavigation()
        {
            var navigationService = ViewModelLocator.Resolve<INavigationService>();
            ViewModelLocator.RegisterDependencies();
            return navigationService.InitializeAsync();
        }
        protected override async void OnStart()
        {
            // Handle when your app starts
            base.OnStart();

            if (Device.RuntimePlatform != Device.UWP)
            {
                await InitNavigation();
            }
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
        private async Task GetGpsLocation()
        {
            var dependencyService = ViewModelLocator.Resolve<IDependencyService>();
            var locator = dependencyService.Get<ILocationServiceImplementation>();

           
                locator.DesiredAccuracy = 50;

                try
                {
                    var position = await locator.GetPositionAsync();
                 GlobalSetting.Instance.Latitude = position.Latitude.ToString();
                GlobalSetting.Instance.Longitude = position.Longitude.ToString();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
           
        }
    }
}
