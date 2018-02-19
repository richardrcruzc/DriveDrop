using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GoDriveDrop.Core.Views
{
    public class OldSplashPage : ContentPage
    {
        public OldSplashPage()
        {
            AbsoluteLayout splashLayout = new AbsoluteLayout
            {
                HeightRequest = 600
            };

            var image = new Image()
            {
                Source = ImageSource.FromFile("logo.png"),
                Aspect = Aspect.AspectFill,
            };
            AbsoluteLayout.SetLayoutFlags(image, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(image, new Rectangle(0f, 0f, 1f, 1f));

            splashLayout.Children.Add(image);

            Content = new StackLayout()
            {
                Children = { splashLayout }
            };
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Delay for a few seconds on the splash screen
            await Task.Delay(3000);

            // Set our Walks Page to be the root page of our application
            var mainPage = new NavigationPage(new MainPage()
            {
                Title = "goDriveDrop- Android",
            });

            // Set the NavigationBar TextColor and Background Color
            mainPage.BarBackgroundColor = Color.FromHex("#223669");
            mainPage.BarTextColor = Color.White;

            //// Declare our DependencyService Interface
            //var navService = DependencyService.Get<IWalkNavService>() as WalkNavService;
            //navService.navigation = mainPage.Navigation;

            //// Register our View Model Mappings between our ViewModels and Views (Pages)
            //navService.RegisterViewMapping(typeof(WalksPageViewModel), typeof(WalksPage));
            //navService.RegisterViewMapping(typeof(WalkEntryViewModel), typeof(WalkEntryPage));
            //navService.RegisterViewMapping(typeof(WalksTrailViewModel), typeof(WalkTrailPage));
            //navService.RegisterViewMapping(typeof(DistTravelledViewModel), typeof(DistanceTravelledPage));

            // Navigate to our Walks Main Page
            Application.Current.MainPage = mainPage;
        }
    }
}
