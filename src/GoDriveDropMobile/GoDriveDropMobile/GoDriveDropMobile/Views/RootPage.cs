using GoDriveDrop.Core.Controls;
using GoDriveDrop.Core.Models;
using GoDriveDrop.Core.Services.Navigation;
using GoDriveDrop.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GoDriveDrop.Core.Views
{
    public class RootPage : MasterDetailPage
    {
        public static bool IsUWPDesktop { get; set; }
        Dictionary<int, NavigationPage> Pages { get; set; }
        
        public RootPage()
        {
            if (IsUWPDesktop)
                this.MasterBehavior = MasterBehavior.Popover;        

            Pages = new Dictionary<int, NavigationPage>();
            Master = new MenuPage(this);


          
            BindingContext = new BaseViewModel
            {
               // Title = title,
               // Subtitle = title,
              //  Icon = "profile"
            };


            



            //setup home page
            Pages.Add((int)MenuType.About, new GoDriveDropNavigationPage(new AboutPage()));
            Detail = Pages[(int)MenuType.About];

            InvalidateMeasure();
        }



        public async Task NavigateAsync(int id)
        { 

            if (Detail != null)
            {
                if (IsUWPDesktop || Device.Idiom != TargetIdiom.Tablet)
                    IsPresented = false;

                if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(300);
            }

            Page newPage;
            if (!Pages.ContainsKey(id))
            {
                var navigationService = ViewModelLocator.Resolve<INavigationService>();
                switch (id)
                {
                    case (int)MenuType.Info:
                        IsBusy = true;
                        Pages.Add(id, new GoDriveDropNavigationPage(new PersonalInfoView()));
                        
                         IsBusy = false;
                        break;
                    case (int)MenuType.About:
                        Pages.Add(id, new GoDriveDropNavigationPage(new AboutPage()));
                        break;

                    case (int)MenuType.LogOut:
                        IsBusy = true;
                        // Logout 
                        await navigationService.NavigateToAsync<LoginViewModel>(new LogoutParameter { Logout = true });
                       await navigationService.RemoveBackStackAsync();
                         
                        IsBusy = false;
                        return;
                    default:
                        Pages.Add(id, new GoDriveDropNavigationPage(new AboutPage()));
                        break;
                        //case (int)MenuType.Blog:
                        //    Pages.Add(id, new GoDriveDropNavigationPage(new BlogPage()));
                        //    break;
                        //case (int)MenuType.DeveloperLife:
                        //    Pages.Add(id, new GoDriveDropNavigationPage(new PodcastPage((MenuType)id)));
                        //    break;
                        //case (int)MenuType.Hanselminutes:
                        //    Pages.Add(id, new GoDriveDropNavigationPage(new PodcastPage((MenuType)id)));
                        //    break;
                        //case (int)MenuType.Ratchet:
                        //    Pages.Add(id, new GoDriveDropNavigationPage(new PodcastPage((MenuType)id)));
                        //    break;
                        //case (int)MenuType.Twitter:
                        //    Pages.Add(id, new GoDriveDropNavigationPage(new TwitterPage()));
                        //    break;
                        //case (int)MenuType.Videos:
                        //    Pages.Add(id, new GoDriveDropNavigationPage(new Channel9VideosPage()));
                        //    break;
                }
            }

            newPage = Pages[id];
            if (newPage == null)
                return;

            //pop to root for Windows Phone
            if (Detail != null && Device.RuntimePlatform == Device.WinPhone)
            {
                await Detail.Navigation.PopToRootAsync();
            }

            Detail = newPage;
        }
    }
}
