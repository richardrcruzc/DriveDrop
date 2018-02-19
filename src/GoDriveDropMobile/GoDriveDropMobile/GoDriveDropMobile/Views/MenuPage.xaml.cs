using GoDriveDrop.Core.Models;
using GoDriveDrop.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoDriveDrop.Core.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MenuPage : ContentPage
	{
        RootPage root;
        List<HomeMenuItem> menuItems;
        public MenuPage(RootPage root)
        {

            this.root = root;
            InitializeComponent();
            if (Device.RuntimePlatform == Device.UWP)                
            {
                BackgroundColor = Color.FromHex("#03A9F4");
                ListViewMenu.BackgroundColor = Color.FromHex("#F5F5F5");
            }

            BindingContext = new BaseViewModel
            {
                Title = "Driver/Sender/Admin: Richard Cruz",
                Subtitle = "Driver / Sender / Admin: Richard Cruz, Sub",
                Icon = "profile"
            };

            ListViewMenu.ItemsSource = menuItems = new List<HomeMenuItem>
                {
                   
                    new HomeMenuItem { Title = "My Personal Info", MenuType = MenuType.Info, Icon = "profilecard.png" },
                    new HomeMenuItem { Title = "My Addresses", MenuType = MenuType.Address, Icon = "iconservice03.png" },
                    new HomeMenuItem { Title = "My Packages", MenuType = MenuType.Package, Icon = "redio02.png" },
                    new HomeMenuItem { Title = "Change Password", MenuType = MenuType.Password, Icon = "keys.png" }, 

                     new HomeMenuItem { Title = "About", MenuType = MenuType.About, Icon ="about.png" },

                };

            ListViewMenu.SelectedItem = menuItems[0];

            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (ListViewMenu.SelectedItem == null)
                    return;

                await this.root.NavigateAsync((int)((HomeMenuItem)e.SelectedItem).MenuType);
            };
        }
    }
}