using DD.Mobile.Services; 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace DD.Mobile.Pages.Senders
{
    public class SenderEntryPage : ContentPage
    {
        private const int IMAGE_SIZE = 150;
       
        private Image photo;

        Entry userEmail,
       password,
       confirmPassword,
       lastName,
       firstName,
       phone,
       primaryPhone,
       pickupStreet,
       pickupCity,
       pickupState,
       pickupCountry,
       pickupZipCode;




        public SenderEntryPage()
        {

            Title = "New Sender Information";

            photo = new Image { WidthRequest = IMAGE_SIZE, HeightRequest = IMAGE_SIZE };
            photo.SetBinding(Image.SourceProperty, "DetailsPlaceholder.jpg");


            lastName = new Entry
            {
                Placeholder = "Last Name"
            };
            firstName = new Entry
            {
                Placeholder = "First Name"
            };
            primaryPhone = new Entry
            {
                Placeholder = "Primary Phone"
            };
            phone = new Entry
            {
                Placeholder = "Phone"
            };
            pickupStreet = new Entry
            {
                Placeholder = "Street"
            };
            pickupCity = new Entry
            {
                Placeholder = "City"
            };
            pickupState = new Entry
            {
                Placeholder = "State"
            };
            pickupCountry = new Entry
            {
                Placeholder = "Country"
            };
            pickupZipCode = new Entry
            {
                Placeholder = "Zip Code"
            };

 
            userEmail = new Entry
            {
                Placeholder = "User Email"
            };
            password = new Entry
            {
                Placeholder = "Password"
            };
            confirmPassword = new Entry
            {
                Placeholder = "Confirm Password"
            };



            Button pickPictureButton = new Button
            {
                Text = "Pick Photo",
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };

            Button takePhoto = new Button
            {
                Text = "take Photo ",
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };


            var headerView = new StackLayout
            {
                Padding = new Thickness(10, 20, 10, 0),
                HorizontalOptions = LayoutOptions.StartAndExpand,
                Orientation = StackOrientation.Horizontal,
                Children = { photo }
            };





            ScrollView scrollView = new ScrollView
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                Content = new StackLayout
                {
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    Padding = new Thickness(30),
                    Children = {
                  
      new Label { Text = "Sender Information" },
       lastName,
       firstName,
       phone,
       primaryPhone,
       new Label { Text = "Profile Photo" },
        photo,
        new Label { Text = "Address for Future Pickup and Dropoff" },
        new Label { Text = "Default Address" },
       pickupStreet,
       pickupCity,
       pickupState,
       pickupCountry,
       pickupZipCode ,
           new Label { Text = "Login Infomation" },
       userEmail,
       password,
       confirmPassword

        }
                }
            };

            // Build the page.
            this.Content = new StackLayout
            {
                Children =
                {
                    headerView,
                    scrollView
                }
            };
            
            var saveDriverITem = new ToolbarItem { Text="Save"};
           saveDriverITem.SetBinding(MenuItem.CommandProperty, "saveDriverITem");


            saveDriverITem.Clicked += (sender, e) => {
                Navigation.PopToRootAsync(true);
            };

            ToolbarItems.Add(saveDriverITem);

            pickPictureButton.Clicked += async (sender, e) =>
            {
                pickPictureButton.IsEnabled = false;
                Stream stream = await DependencyService.Get<IPicturePicker>().GetImageStreamAsync();

                if (stream != null)
                {
                    Image image = new Image
                    {
                        Source = ImageSource.FromStream(() => stream),
                        BackgroundColor = Color.Gray
                    };

                    TapGestureRecognizer recognizer = new TapGestureRecognizer();
                    recognizer.Tapped += (sender2, args) =>
                    {
                        //(MainPage as ContentPage).Content = stack;
                        pickPictureButton.IsEnabled = true;
                    };
                    image.GestureRecognizers.Add(recognizer);

                    //(MainPage as ContentPage).Content = image;
                }
                else
                {
                    pickPictureButton.IsEnabled = true;
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