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
                BackgroundColor = Color.FromHex("#43b249");
                ListViewMenu.BackgroundColor = Color.FromHex("#F5F5F5");
            }

            var title = "Sender: ";
            if (GlobalSetting.Instance.CurrentCustomerModel.CustomerType == "Driver")
                title = "Driver: ";
            title += " " + GlobalSetting.Instance.CurrentCustomerModel.FullName;

            textUserName.Text = GlobalSetting.Instance.CurrentCustomerModel.FullName;
            imgProfile.Source = GlobalSetting.Instance.CurrentCustomerModel.PersonalPhotoUrl;
            BindingContext = new BaseViewModel
            {
                 Title = title,
                 Subtitle = title,
                Icon = "iconlogo"
            };
            
                ListViewMenu.ItemsSource = menuItems = new List<HomeMenuItem>
                { 
                    new HomeMenuItem { Title = "My Personal Info", MenuType = MenuType.Info, Icon = "profilecard.png" },
                    new HomeMenuItem { Title = "My Addresses", MenuType = MenuType.Address, Icon = "iconservice03.png" },
                    new HomeMenuItem { Title = "My Packages", MenuType = MenuType.Package, Icon = "redio02.png" },
                     new HomeMenuItem { Title = "Send a Package", MenuType = MenuType.NewPackage, Icon = "redio06.png" },
                    new HomeMenuItem { Title = "Change Password", MenuType = MenuType.Password, Icon = "keys.png" }, 
                    new HomeMenuItem { Title = "About", MenuType = MenuType.About, Icon ="about.png" },
                    new HomeMenuItem { Title = "LogOut", MenuType = MenuType.LogOut, Icon ="logout.png" }, 
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