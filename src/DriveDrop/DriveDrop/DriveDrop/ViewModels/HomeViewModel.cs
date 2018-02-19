using DriveDrop.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace DriveDrop.Core.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public ObservableCollection<HomeMenuItem> MenuItems { get; set; }
        public HomeViewModel()
        {
            CanLoadMore = true;
            Title = "Hanselman";
            MenuItems = new ObservableCollection<HomeMenuItem>();
            MenuItems.Add(new HomeMenuItem
            {
                Id = 0,
                Title = "About",
                MenuType = MenuType.About,
                Icon = "about.png"
            });
            MenuItems.Add(new HomeMenuItem
            {
                Id = 1,
                Title = "Blog",
                MenuType = MenuType.Address,
                Icon = "blog.png"
            });
            MenuItems.Add(new HomeMenuItem
            {
                Id = 2,
                Title = "Twitter",
                MenuType = MenuType.Info,
                Icon = "twitternav.png"
            });
            MenuItems.Add(new HomeMenuItem
            {
                Id = 3,
                Title = "Hanselminutes",
                MenuType = MenuType.Package,
                Icon = "hm.png"
            });
            MenuItems.Add(new HomeMenuItem
            {
                Id = 4,
                Title = "Ratchet & The Geek",
                MenuType = MenuType.Password,
                Icon = "ratchet.png"
            });

        
        }

    }
}
