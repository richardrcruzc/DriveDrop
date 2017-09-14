using DD.Mobile.Models;
using DD.Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace DD.Mobile.Pages
{
    public class LoginPage : ContentPage
    {

        private LoginViewModel loginViewModel;

        private LoginViewModel Model
        {
            get
            {
                if (loginViewModel == null)
                    loginViewModel = new LoginViewModel(App.Service);

                return loginViewModel;
            }
        }

        public LoginPage()
        {
            BindingContext = Model;

            var logo = new Image { Source = FileImageSource.FromFile("logo.png") };

            var usernameEntry = new Entry { Placeholder = "Username", StyleId = "UserId" };
            usernameEntry.SetBinding(Entry.TextProperty, "Username");

            var passwordEntry = new Entry { IsPassword = true, Placeholder = "Password", StyleId = "PassId" };
            passwordEntry.SetBinding(Entry.TextProperty, "Password");

            var loginButton = new Button { Text = "Login" };
            loginButton.Clicked += OnLoginClicked;

            var helpButton = new Button { Text = "Help" };
            helpButton.Clicked += OnHelpClicked;

            var toolbarItem = new ToolbarItem
            {
                Text = "Sign Up"
            };
            toolbarItem.Clicked += OnSignUpButtonClicked;
            ToolbarItems.Add(toolbarItem);



            var grid = new Grid()
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };

            if (Device.RuntimePlatform == Device.iOS)
            {
                grid.Children.Add(loginButton, 0, 0);
                grid.Children.Add(helpButton, 1, 0);

                Content = new StackLayout()
                {
                    VerticalOptions = LayoutOptions.StartAndExpand,
                    Padding = new Thickness(30),
                    Children = { logo, usernameEntry, passwordEntry, grid }
                };

                BackgroundImage = "login_box";

            }
            else
            {
                grid.Children.Add(logo, 0, 0);
                grid.Children.Add(helpButton, 1, 0);

                Content = new StackLayout()
                {
                    VerticalOptions = LayoutOptions.Center,
                    Padding = new Thickness(30),
                    Children = { grid, usernameEntry, passwordEntry, loginButton },
                };
            }

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private void OnLoginClicked(object sender, EventArgs e)
        {
            if (loginViewModel.CanLogin)
            {
                loginViewModel
                .LoginAsync(System.Threading.CancellationToken.None)
                .ContinueWith(_ => {
                    App.LastUseTime = System.DateTime.UtcNow;
                    if(Navigation.NavigationStack!=null && Navigation.NavigationStack.Count>0)
                    Navigation.PopAsync();

                });

                Navigation.PopModalAsync();
            }
            else
            {
                DisplayAlert("Error", loginViewModel.ValidationErrors, "OK");
            }
        }

        private void OnHelpClicked(object sender, EventArgs e)
        {
            DisplayAlert("Help", "Enter any username and password", "OK");
        }
        async void OnSignUpButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SignUpPage());
        }

        //    Entry usernameEntry, passwordEntry;
        //    Label messageLabel;

        //    public LoginPage()
        //    {
        //        var toolbarItem = new ToolbarItem
        //        {
        //            Text = "Sign Up"
        //        };
        //        toolbarItem.Clicked += OnSignUpButtonClicked;
        //        ToolbarItems.Add(toolbarItem);

        //        var logo = new Image { Source = FileImageSource.FromFile("logo.png") };


        //        messageLabel = new Label();
        //        usernameEntry = new Entry
        //        {
        //            Placeholder = "username"
        //        };
        //        passwordEntry = new Entry
        //        {
        //            IsPassword = true
        //        };
        //        var loginButton = new Button
        //        {
        //            Text = "Login"
        //        };
        //        loginButton.Clicked += OnLoginButtonClicked; 

        //        var forgotPasswordButton = new Button { Text = "Forgot Password" };
        //        forgotPasswordButton.Clicked += OnHelpClicked;



        //        Title = "Login";
        //        Content = new StackLayout
        //        {
        //            VerticalOptions = LayoutOptions.StartAndExpand,
        //            Padding = new Thickness(30),
        //            Children = {
        //                logo,
        //                new Label { Text = "Username" },
        //                usernameEntry,
        //                new Label { Text = "Password" },
        //                passwordEntry,
        //                loginButton,
        //                messageLabel, 
        //                forgotPasswordButton,


        //            }
        //        };

        //        //if (Device.RuntimePlatform == Device.iOS)
        //        //{
        //        //    BackgroundImage = "login_box";
        //        //}

        //    }

        //    async void OnSignUpButtonClicked(object sender, EventArgs e)
        //    {
        //        await Navigation.PushAsync(new SignUpPage());
        //    }


        //    async void OnLoginButtonClicked(object sender, EventArgs e)
        //    {
        //        var user = new User
        //        {
        //            Username = usernameEntry.Text,
        //            Password = passwordEntry.Text
        //        };

        //        var isValid = AreCredentialsCorrect(user);
        //        if (isValid)
        //        {
        //            App.LastUseTime = System.DateTime.UtcNow;
        //            App.IsUserLoggedIn = true;
        //            Navigation.InsertPageBefore(new MainPage(), this);
        //            await Navigation.PopAsync();
        //        }
        //        else
        //        {
        //            await DisplayAlert("Error", "Login failed", "OK");
        //            //messageLabel.Text = "Login failed";
        //            passwordEntry.Text = string.Empty;
        //        }
        //    }

        //    bool AreCredentialsCorrect(User user)
        //    {
        //        return user.Username == Constants.Username && user.Password == Constants.Password;
        //    }

        //    private void OnHelpClicked(object sender, EventArgs e)
        //    {
        //        DisplayAlert("Help", "Enter any username and password", "OK");
        //    }
    }
}