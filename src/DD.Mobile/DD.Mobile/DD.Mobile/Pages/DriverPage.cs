using DD.Mobile.Models.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace DD.Mobile.Pages
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
                new DriverEntry{ FirstName="One", LastName="1", MaxPackage=1, PersonalPhotoUri="",},
                new DriverEntry{ FirstName="Two", LastName="2" , MaxPackage=2, PersonalPhotoUri="", },
                new DriverEntry{ FirstName="Three", LastName="3",  MaxPackage=3, PersonalPhotoUri="", },

            };

            var itemTemplate = new DataTemplate(typeof(ImageCell));
            itemTemplate.SetBinding(TextCell.TextProperty, "First Name");
            itemTemplate.SetBinding(TextCell.TextProperty, "Last Name");
            itemTemplate.SetBinding(TextCell.TextProperty, "Max Package");
            //itemTemplate.SetBinding(TextCell.DetailProperty, "Note");
            itemTemplate.SetBinding(ImageCell.ImageSourceProperty, "PersonalPhotoUri");

            var driversList = new ListView {
                HasUnevenRows = true,
                 ItemTemplate= itemTemplate,
                 ItemsSource = driverItems,
                 SeparatorColor = Color.FromHex("#ddd"),
            };

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