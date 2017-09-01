using DD.Mobile.Pages;
using DD.Mobile.Views;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace DD.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //check the targe OS platform
            if (Device.RuntimePlatform == Device.Android)
            {
                MainPage = new SplashPage();
            }
            else
            {
                // the roor page your application
                var navPage =  new NavigationPage( new DD.Mobile.Pages.DriverPage(){Title="Driver Page" });
                MainPage = navPage;
            }



            //SetMainPage();
            
            //var content = new ContentPage {
            //    Title ="DriveDrop",
            //    Content = new StackLayout {
            //        VerticalOptions=LayoutOptions.Center,                    
            //        Children = { new Label{
            //            HorizontalTextAlignment = TextAlignment.Center,
            //            Text ="Welcome to DriveDrop App!"
            //         }
            //        }
            //    }
                
            //};
           // MainPage = new NavigationPage(content);
        }

        public static void SetMainPage()
        {
            Current.MainPage = new TabbedPage
            {
                Children =
                {
                    new NavigationPage(new ItemsPage())
                    {
                        Title = "Browse",
                        Icon = Device.OnPlatform("tab_feed.png",null,null)
                    },
                    new NavigationPage(new AboutPage())
                    {
                        Title = "About",
                        Icon = Device.OnPlatform("tab_about.png",null,null)
                    },
                }
            };
        }
    }
}
