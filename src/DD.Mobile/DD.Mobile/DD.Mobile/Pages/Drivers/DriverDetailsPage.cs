using DD.Mobile.Models.Drivers;
using DD.Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace DD.Mobile.Pages.Drivers
{
    public class DriverDetailsPage : ContentPage
    {
      

        public DriverDetailsPage(DriverEntry driverEntry)
        {

            Title = "Driver Details Page";

            BindingContext = new DriverDetailsViewModel(driverEntry);

            var beging = new Button
            {
                BackgroundColor = Color.FromHex("#008080"),
                TextColor=Color.White,
                Text = "Begin this"
            };
            //set up our event handler
            beging.Clicked += (send, e) =>
            {
                if (driverEntry == null) return;
                Navigation.PushAsync(new DriverTravelPage(driverEntry));
                Navigation.RemovePage(this);
                driverEntry = null;
            };

            var driverImage = new Image()
            {
                Aspect = Aspect.AspectFill,
                Source = driverEntry.PersonalPhotoUri
            };

            driverImage.SetBinding(Image.SourceProperty, "driverEntry.PersonalPhotoUri");

            var driverFirstNameLabel = new Label()
            {
                FontSize =38,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.Black,
                Text=driverEntry.FirstName,
            };
            driverFirstNameLabel.SetBinding(Label.TextProperty, "driverEntry.FirstName");

            var driverLastNameLabel = new Label()
            {
                FontSize = 38,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.Black,
                Text = driverEntry.LastName,
            };
            driverLastNameLabel.SetBinding(Label.TextProperty, "driverEntry.LastName");

            var driverMaxPackageLabel = new Label()
            {
                FontSize = 38,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.Black,
                Text = $"Lenght:{driverEntry.MaxPackage} PPL" ,

            };
            driverMaxPackageLabel.SetBinding(Label.TextProperty, "driverEntry.MaxPackage",stringFormat:"Lenght:{0}");









            // Dictionary to get Color from color name.
            Dictionary<string, Color> nameToColor = new Dictionary<string, Color>
        {
            { "Aqua", Color.Aqua }, { "Black", Color.Black },
            { "Blue", Color.Blue }, { "Fucshia", Color.Fuchsia },
            { "Gray", Color.Gray }, { "Green", Color.Green },
            { "Lime", Color.Lime }, { "Maroon", Color.Maroon },
            { "Navy", Color.Navy }, { "Olive", Color.Olive },
            { "Purple", Color.Purple }, { "Red", Color.Red },
            { "Silver", Color.Silver }, { "Teal", Color.Teal },
            { "White", Color.White }, { "Yellow", Color.Yellow }
        };
            Label header = new Label
            {
                Text = "Picker",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center
            };

            Picker picker = new Picker
            {
                Title = "Color",
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            foreach (string colorName in nameToColor.Keys)
            {
                picker.Items.Add(colorName);
            }

            // Create BoxView for displaying picked Color
            BoxView boxView = new BoxView
            {
                WidthRequest = 150,
                HeightRequest = 150,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            picker.SelectedIndexChanged += (sender, args) =>
            {
                if (picker.SelectedIndex == -1)
                {
                    boxView.Color = Color.Default;
                }
                else
                {
                    string colorName = picker.Items[picker.SelectedIndex];
                    boxView.Color = nameToColor[colorName];
                }
            };

            // Accomodate iPhone status bar.
          //  this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);


            //// Build the page.
            this.Content = new ScrollView
            {
                Padding = 10,
                Content = new StackLayout
                {
                    Orientation = StackOrientation.Vertical,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Children =
                    {
                     
                        driverImage,
                        driverFirstNameLabel,
                        driverLastNameLabel,
                        driverMaxPackageLabel,
                           beging,
                          header,
                    picker,
                    boxView
                    }
                }

            };





            //// Build the page.
            //this.Content = new StackLayout
            //{
            //    Children =
            //    {
            //        header,
            //        picker,
            //        boxView
            //    }
            //};

            //Content = new StackLayout
            //{
            //    Children = {
            //        new Label { Text = "Welcome to Xamarin Forms!" }
            //    }
            //};
        }
    }
}