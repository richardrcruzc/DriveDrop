using DD.Mobile.Pages;
using DD.Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace DD.Mobile.Pages
{
 
        public class MainPage : MasterDetailPage
        {
            MasterPage masterPage;
        private ToolbarItem toolbarItem;

        public MainPage()
            {
              toolbarItem = new ToolbarItem
            {
                Text = "Logout"
            };
            toolbarItem.Clicked += OnLogoutButtonClicked;
            ToolbarItems.Add(toolbarItem);

            Title = "DriveDrop";

            masterPage = new MasterPage();
                Master = masterPage;
                Detail = new NavigationPage(new ContactsPage());

                masterPage.ListView.ItemSelected += OnItemSelected;

                if (Device.RuntimePlatform == Device.Windows)
                {
                    MasterBehavior = MasterBehavior.Popover;
                }  
            }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            if (LoginViewModel.ShouldShowLogin(App.LastUseTime))
                await Navigation.PushModalAsync(new LoginPage());
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            SetToolbarItems(false);
        }

        private void SetToolbarItems(bool show)
        {
            if (Device.RuntimePlatform != Device.WinPhone)
                return;

            if (show)
            {
                ToolbarItems.Add(toolbarItem);
            }
            else if (Device.RuntimePlatform == Device.WinPhone)
            {
                ToolbarItems.Remove(toolbarItem);
            }
        }
        void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
            {
                var item = e.SelectedItem as MasterPageItem;
                if (item != null)
                {
                    Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargetType));
                    masterPage.ListView.SelectedItem = null;
                    IsPresented = false;
                }
            }
            async void OnLogoutButtonClicked(object sender, EventArgs e)
            {
               // App.IsUserLoggedIn = false;
                Navigation.InsertPageBefore(new LoginPage(), this);
                await Navigation.PopAsync();
            }
        }
       
    }
 