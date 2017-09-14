using DD.Mobile.Models;
using DD.Mobile.Pages.Drivers;
using DD.Mobile.Pages.Senders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace DD.Mobile.Pages
{
    public class SignUpPage : ContentPage
    {       
        
        public SignUpPage()
        {

            Title = "Sign Up";

            var driverButton = new Button
            {
                Text = "As Driver" 
            };
            driverButton.Clicked += OnDriverButtonClicked;

            Label headerDriver = new Label
            {
                Text = "SigUp as sender to be able to send package" ,
                
                HorizontalOptions = LayoutOptions.Center
            };

            



            var senderButton = new Button { Text = "As Sender"  };
            senderButton.Clicked += OnSenderButtonClicked;

            Label headerSender = new Label
            {
                Text = "SigUp as driver to be able to deliver package" ,
              
                HorizontalOptions = LayoutOptions.Center
            };

           



            Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.StartAndExpand,
                Children = {
                    senderButton,                   
                    headerDriver,
                   
                     driverButton,
                     headerSender,
                    
                }
            };
        }

         async void OnDriverButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DriverEntryPage());
        }
        async void OnSenderButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SenderEntryPage());
        }
    }
}