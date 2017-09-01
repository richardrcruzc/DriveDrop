using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DD.Mobile.Pages
{
    public class SplashPage : ContentPage
    {
        public SplashPage()
        {


            AbsoluteLayout splashLayOut = new AbsoluteLayout { HeightRequest = 600 };

            var image = new Image()
            {
                Source = ImageSource.FromFile("brand.png"),
                Aspect = Aspect.AspectFill,
            };

            AbsoluteLayout.SetLayoutFlags(image, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(image, new Rectangle(0f, 0f, 1f, 1f));
            splashLayOut.Children.Add(image);

            Content = new StackLayout() { Children = { splashLayOut } };
        }



            protected override async void OnAppearing() {
            base.OnAppearing();
            //delay for a few senconds on the splash screen
            await Task.Delay(300);

            //instantiate a navigationPage with the mainpage
            var navPage = new NavigationPage(new DriverPage() {
                Title = "Driver Details"
            });

            Application.Current.MainPage = navPage;
        }
       


            //Content = new StackLayout
            //{
            //    Children = {
            //        new Label { Text = "Welcome to Xamarin Forms!" }
            //    }
            //};
        }
}
