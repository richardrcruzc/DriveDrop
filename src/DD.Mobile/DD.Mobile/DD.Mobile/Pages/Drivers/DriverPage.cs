using DD.Mobile.Models.Drivers;
using DD.Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace DD.Mobile.Pages.Drivers
{
    public class DriverPage : ContentPage
    {
        public DriverPage()
        {

            var newDriverItem = new ToolbarItem {Text="Add Driver" };
            newDriverItem.Clicked += (sender, e) => {
                Navigation.PushAsync(new DriverEntryPage());
            };
            ToolbarItems.Add(newDriverItem);

            var driverItems = new List<DriverEntry>
            {
                new DriverEntry{ FirstName="One", LastName="1wewe",longitude=-122.360348, Latitude=47.175708,  MaxPackage=1, PersonalPhotoUri="http://trailswa.com.au/media/cache/media/images/trails/_mid/FullSizeRender1_600_480_c1.jpg",},
                new DriverEntry{ FirstName="Two", LastName="2we wewe" ,longitude=-122.360348, Latitude=47.175708, MaxPackage=2, PersonalPhotoUri="http://trailswa.com.au/media/cache/media/images/trails/_mid/Ancient_Empire_534_480_c1.jpg", },
                new DriverEntry{ FirstName="Three", LastName="3 we wewe", longitude=-122.360348, Latitude=47.175708, MaxPackage=3, PersonalPhotoUri="http://trailswa.com.au/media/cache/media/images/trails/_mid/Ancient_Empire_534_480_c1.jpg", },

            };

            BindingContext = new DriverViewModel();

            var itemTemplate = new DataTemplate(typeof(ImageCell));
            itemTemplate.SetBinding(TextCell.TextProperty, "FirstName");
            itemTemplate.SetBinding(TextCell.TextProperty, "FirstName");
            itemTemplate.SetBinding(TextCell.DetailProperty, "LastName");
            //itemTemplate.SetBinding(TextCell.TextProperty, "MaxPackage");
            //itemTemplate.SetBinding(TextCell.DetailProperty, "Note");
            itemTemplate.SetBinding(ImageCell.ImageSourceProperty, "PersonalPhotoUri");

            var driversList = new ListView {
                HasUnevenRows = true,
                 ItemTemplate= itemTemplate,
               //  ItemsSource = driverItems,
                 SeparatorColor = Color.FromHex("#ddd"),

            };

            //set binding property for our driver Entries

            driversList.SetBinding(ItemsView<Cell>.ItemsSourceProperty, "driverEntries");


            //set up event handler

            driversList.ItemTapped += (object sender, ItemTappedEventArgs e) => {
                var item = (DriverEntry)e.Item;
                if (item == null) return;
                Navigation.PushAsync(new DriverDetailsPage(item));
                item = null;
            };
            Content = driversList;


            //Content = new StackLayout
            //{
            //    Children = {
            //        new Label { Text = "Welcome to Xamarin Forms!" }
            //    }
            //};
        }
    }
}