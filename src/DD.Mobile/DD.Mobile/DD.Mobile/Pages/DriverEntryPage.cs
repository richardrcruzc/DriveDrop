using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace DD.Mobile.Pages
{
    public class DriverEntryPage : ContentPage
    {
        public DriverEntryPage()
        {



            Title = "New Driver Entry";
            var driverFirtName = new EntryCell {
                Label = "First Name:",
                Placeholder= "FirstName",
            };
            var driverLastName = new EntryCell
            {
                Label = "Last Name:",
                Placeholder = "LastName",
            };
            var driverMaxPackage = new EntryCell
            {
                Label = "MaxPackage:",
                Placeholder = "MaxPackage",
                Keyboard = Keyboard.Numeric,
            };
            var driverImageUrl= new EntryCell
            {
                Label = "ImageUrl:",
                Placeholder = "PersonalPhotoUri" 
            };

            Content = new TableView {
                Intent = TableIntent.Form,
                Root = new TableRoot {
                    new TableSection()
                    {
                        driverFirtName,
                        driverLastName,
                        driverMaxPackage,
                        driverImageUrl
                    }

                }

            };

            var saveDriverITem = new ToolbarItem { Text="Save"};

            saveDriverITem.Clicked += (sender, e) => {
                Navigation.PopToRootAsync(true);
            };

            ToolbarItems.Add(saveDriverITem);


            //Content = new StackLayout
            //{
            //    Children = {
            //        new Label { Text = "Welcome to Xamarin Forms!" }
            //    }
            //};
        }
    }
}