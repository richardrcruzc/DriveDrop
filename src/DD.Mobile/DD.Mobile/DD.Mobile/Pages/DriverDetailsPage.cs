﻿using DD.Mobile.Models.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace DD.Mobile.Pages
{
    public class DriverDetailsPage : ContentPage
    {
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

        public DriverDetailsPage(DriverEntry driverEntry )
        {

            Title = "Driver Details Page";
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
            this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

            // Build the page.
            this.Content = new StackLayout
            {
                Children =
                {
                    header,
                    picker,
                    boxView
                }
            };

            //Content = new StackLayout
            //{
            //    Children = {
            //        new Label { Text = "Welcome to Xamarin Forms!" }
            //    }
            //};
        }
    }
}