using GoDriveDrop.Core.Controls;
using GoDriveDrop.Core.Identity;
using GoDriveDrop.Core.Models;
using GoDriveDrop.Core.Services;
using GoDriveDrop.Core.Services.Navigation;
using GoDriveDrop.Core.Services.OpenUrl;
using GoDriveDrop.Core.Validations;
using GoDriveDrop.Core.Views;
using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace GoDriveDrop.Core.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private ValidatableObject<string> _userName;
        private ValidatableObject<string> _password;
        private bool _isMock;
        private bool _showLogin;
        private bool _isValid;
        private bool _isLogin;
        private string _authUrl;

        private IOpenUrlService _openUrlService;
        private IIdentityService _identityService;

        public LoginViewModel(
            IOpenUrlService openUrlService,
            IIdentityService identityService)
        {
            Title = "goDriveDrop.com - Login";

            _openUrlService = openUrlService;
            _identityService = identityService;

            _userName = new ValidatableObject<string>();
            _password = new ValidatableObject<string>();

            InvalidateMock();
            AddValidations();

        }

        public ValidatableObject<string> UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
                RaisePropertyChanged(() => UserName);
            }
        }

        public ValidatableObject<string> Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                RaisePropertyChanged(() => Password);
            }
        }

        public bool IsMock
        {
            get
            {
                return _isMock;
            }
            set
            {
                _isMock = value;
                RaisePropertyChanged(() => IsMock);
            }
        }

        public bool IsValid
        {
            get
            {
                return _isValid;
            }
            set
            {
                _isValid = value;
                RaisePropertyChanged(() => IsValid);
            }
        }
        
        public bool ShowLogin
        {
            get
            {
                return _showLogin;
            }
            set
            {
                _showLogin = value;
                RaisePropertyChanged(() => ShowLogin);
            }
        }
        public bool IsLogin
        {
            get
            {
                return _isLogin;
            }
            set
            {
                _isLogin = value;
                RaisePropertyChanged(() => IsLogin);
            }
        }

        public string LoginUrl
        {
            get
            {
                return _authUrl;
            }
            set
            {
                _authUrl = value;
                RaisePropertyChanged(() => LoginUrl);
            }
        }

        

        public ICommand SignInCommand => new Command(async () => await SignInAsync());
        public ICommand BSenderCommand => new Command(async () => await BeSenderCommandAsync());
        public ICommand BeDriverCommand => new Command(async () => await BeDriverCommandAsync());
        private async Task BeSenderCommandAsync()
        {
            IsBusy = true;
            // Logout

            var navigationService = ViewModelLocator.Resolve<INavigationService>();
            await navigationService.NavigateToAsync<NewSenderViewModel>();
            await navigationService.RemoveBackStackAsync();

            IsBusy = false;
        }
        private async Task BeDriverCommandAsync()
        { 
            IsBusy = true;
            // Logout

            var navigationService = ViewModelLocator.Resolve<INavigationService>();
              await navigationService.NavigateToAsync<NewDriverViewModel>();
            await navigationService.RemoveBackStackAsync();

            IsBusy = false;
        }


        //public ICommand MockSignInCommand => new Command(async () => await MockSignInAsync());
        //public ICommand RegisterCommand => new Command(Register);

             public ICommand NavigateCommand => new Command<string>(async (url) => await NavigateAsync(url));

            //public ICommand SettingsCommand => new Command(async () => await SettingsAsync());

            //public ICommand ValidateUserNameCommand => new Command(() => ValidateUserName());

            //public ICommand ValidatePasswordCommand => new Command(() => ValidatePassword());

        public override Task InitializeAsync(object navigationData)
        {
            var signin = true;
            if (navigationData is LogoutParameter)
            {
                var logoutParameter = (LogoutParameter)navigationData;
                signin = !logoutParameter.Logout;
                if (logoutParameter.Logout)
                {
                    Logout();
                }
            }
            if(signin)
            SignInAsync().GetAwaiter();
            return base.InitializeAsync(navigationData);
            
            
        }

        private async Task MockSignInAsync()
        {
            IsBusy = true;
            IsValid = true;
            bool isValid = Validate();
            bool isAuthenticated = false;

            if (isValid)
            {
                try
                {
                    await Task.Delay(1000);

                    isAuthenticated = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[SignIn] Error signing in: {ex}");
                }
            }
            else
            {
                IsValid = false;
            }

            if (isAuthenticated)
            {
                GlobalSetting.Instance.AuthAccessToken = GlobalSetting.Instance.AuthToken;

                //  await NavigationService.NavigateToAsync<MainViewModel>();
                await NavigationService.RemoveLastFromBackStackAsync();
            }

            IsBusy = false;
        }

        private async Task SignInAsync()
        {
            IsBusy = true;

            await Task.Delay(100);

            //var client = new DiscoveryClient("http://169.254.80.80:5205");
            //client.Policy.RequireHttps = false;

            //var disco = await client.GetAsync();
            ////var tt = disco.AuthorizeEndpoint;



            //// request token
            //var tokenClient = new TokenClient("http://169.254.80.80:5205", GlobalSetting.Instance.ClientId, GlobalSetting.Instance.ClientSecret);
            //var tokenResponse = await tokenClient.RequestClientCredentialsAsync("openid profile drivedrop offline_access");
            //if (tokenResponse.IsError)
            //{
            //}



            LoginUrl = _identityService.CreateAuthorizationRequest();

            IsValid = true;
            IsLogin = true;
            IsBusy = false;
        }

        private void Register()
        {
            _openUrlService.OpenUrl(GlobalSetting.Instance.RegisterWebsite);
        }

        private void Logout()
        {
            var authIdToken = GlobalSetting.Instance.AuthIdToken;
            var logoutRequest = _identityService.CreateLogoutRequest(authIdToken);

            if (!string.IsNullOrEmpty(logoutRequest))
            {
                // Logout
                LoginUrl = logoutRequest;
            }

            
                GlobalSetting.Instance.AuthAccessToken = string.Empty;
                GlobalSetting.Instance.AuthIdToken = string.Empty;
            

           // _settingsService.UseFakeLocation = false;
        }

        private async Task NavigateAsync(string url)
        {
            var unescapedUrl = System.Net.WebUtility.UrlDecode(url);

            if (unescapedUrl.Equals(GlobalSetting.Instance.LogoutCallback))
            {
                GlobalSetting.Instance.AuthAccessToken = string.Empty;
                GlobalSetting.Instance.AuthIdToken = string.Empty;
                IsLogin = false;
                LoginUrl = _identityService.CreateAuthorizationRequest();
            }
             else if (unescapedUrl.Contains(GlobalSetting.Instance.IdentityCallback)) 
            {
                var authResponse = new AuthorizeResponse(url);
                if (!string.IsNullOrWhiteSpace(authResponse.Code))
                {
                    var userToken = await _identityService.GetTokenAsync(authResponse.Code);
                    string accessToken = userToken.AccessToken;

                    if (!string.IsNullOrWhiteSpace(accessToken))
                    {
                        GlobalSetting.Instance.AuthAccessToken = accessToken;
                        GlobalSetting.Instance.AuthIdToken = authResponse.IdentityToken;
                      //  await NavigationService.NavigateToAsync<RootPage>();
                        //await NavigationService.RemoveLastFromBackStackAsync();

                        var title = "goDriveDrop - iOS";
                        if (Device.RuntimePlatform == Device.iOS)
                            title = "goDriveDrop - iOS";
                        if (Device.RuntimePlatform == Device.Android)
                            title = "goDriveDrop - Android";
                        if (Device.RuntimePlatform == Device.UWP)
                            title = "goDriveDrop - UWP";
                        if (Device.RuntimePlatform == Device.WinPhone)
                            title = "goDriveDrop - WinPhone";

                        //await NavigationService.NavigateToAsync<RootPage>()

                        // Set our Walks Page to be the root page of our application
                        var mainPage = new NavigationPage(new RootPage()
                        {
                            Title = title,
                        });

                        // Set the NavigationBar TextColor and Background Color
                        mainPage.BarBackgroundColor = Color.FromHex("#223669");
                        mainPage.BarTextColor = Color.White;
                        Application.Current.MainPage = mainPage;
                    }
                }
            }
        }

        private async Task SettingsAsync()
        {
            await Task.Delay(500);
            //await NavigationService.NavigateToAsync<SettingsViewModel>();
        }

        private bool Validate()
        {
            bool isValidUser = ValidateUserName();
            bool isValidPassword = ValidatePassword();

            return isValidUser && isValidPassword;
        }

        private bool ValidateUserName()
        {
            return _userName.Validate();
        }

        private bool ValidatePassword()
        {
            return _password.Validate();
        }

        private void AddValidations()
        {
            _userName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "A username is required." });
            _password.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "A password is required." });
        }

        public void InvalidateMock()
        {
            IsMock = false; // GlobalSetting.Instance.UseMocks;
        }
    }
}
