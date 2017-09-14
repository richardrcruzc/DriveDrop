using DD.Mobile.Pages;
using DD.Mobile.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace DD.Mobile
{
    public class App : Application
    {
        public static ILoginService Service { get; set; }

        public static DateTime LastUseTime { get; set; }

        public App()
        {

            var task = Task.Run(() =>
            {
                 
                Service = new LoginService();
            });

            task.Wait();


            // Check the Device Target OS Platform
            if (Device.RuntimePlatform == Device.Android) 
            {
                // Set the root page of your application
                MainPage = new SplashPage();
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                // Set our Walks Page to be the root page of our application
                var mainPage = new NavigationPage(new MainPage()
                {
                    Title = "DriveDrop - iOS",
                });

                // Set the NavigationBar TextColor and Background Color
                mainPage.BarBackgroundColor = Color.FromHex("#440099");
                mainPage.BarTextColor = Color.White;


                // Declare our DependencyService Interface
                var navService = DependencyService.Get<ILoginService>() as LoginService;
                //  navService.navigation = mainPage.Navigation;
               
                // Navigate to our Walks Main Page
                App.Current.MainPage = mainPage;
            }



        }
    }
}
