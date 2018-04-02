using GoDriveDrop.Core.Models.Commons;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoDriveDrop.Core
{
    public class GlobalSetting
    {
        public CustomerModel _currentCustomer;
        public string _user = "admin@driveDrop.com";
        public string _passworrd = "Pass@word1";

      

        public const string AzureTag = "Azure";
        public const string MockTag = "Mock";
              
        // public const string DefaultEndpoint = "http://godrivedrop.azurewebsites.net/"; 
        public const string DefaultEndpoint =  "http://169.254.80.80:5205/";


        private string _baseEndpoint;

        private static readonly GlobalSetting _instance = new GlobalSetting();

        public GlobalSetting()
        {
            AuthToken = "INSERT AUTHENTICATION TOKEN";
            BaseEndpoint = DefaultEndpoint;
        }

        public static GlobalSetting Instance
        {
            get { return _instance; }
        }

        public string BaseEndpoint
        {
            get { return _baseEndpoint; }
            set
            {
                _baseEndpoint = value;
                UpdateEndpoint(_baseEndpoint);
            }
        }

        public string ApiEndpoint
        {
            get { return _baseEndpoint; }
            set
            {
                _baseEndpoint = value;
                UpdateEndpoint(_baseEndpoint);
            }
        }

        public CustomerModel CurrentCustomerModel
        {
            get { return _currentCustomer; }
            set
            {
                _currentCustomer = value;
            }
        }
        public string CurrentCustomer { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }

   

        public string ClientId { get { return "xamarin"; } }

        public string ClientSecret { get { return "secret"; } }

        public string AuthToken { get; set; }

        public string RegisterWebsite { get; set; }

        public string RegisterDriver { get; set; }

        public string RegisterSender { get; set; }

        public string CatalogEndpoint { get; set; }

        public string OrdersEndpoint { get; set; }

        public string BasketEndpoint { get; set; }

        public string IdentityEndpoint { get; set; }

        public string LocationEndpoint { get; set; }

        public string MarketingEndpoint { get; set; }

        public string UserInfoEndpoint { get; set; }

        public string TokenEndpoint { get; set; }

        public string LogoutEndpoint { get; set; }

        public string IdentityCallback { get; set; }

        public string LogoutCallback { get; set; }

        public string AuthAccessToken { get; set; }
        public string AuthIdToken { get; set; }


        public string DriverEndPoint { get; set; }

        public string UserValidation { get; set; }


        public string CustomerId { get; set; }
        public string CommonEndpoint { get; set; }


        public string User
        {
            get { return _user; }
            set
            {
                _user = value;
            }
        }
        public string Password
        {
            get { return _passworrd; }
            set
            {
                _passworrd = value;
            }
        }

        public string PicBaseUrl { get; set; }


        private void UpdateEndpoint(string baseEndpoint)
        {

            PicBaseUrl = $"{baseEndpoint}Pic/GetImage/fileName/[0]/pic/";

            CurrentCustomer = $"{baseEndpoint}/api/v1/CurrentUser";
            RegisterWebsite = $"{baseEndpoint}/home/register";
            RegisterSender = $"{baseEndpoint}Sender/NewSender";
            RegisterDriver = $"{baseEndpoint}Driver/NewDriver";
            CommonEndpoint = $"{ApiEndpoint}common";

            CatalogEndpoint = $"{baseEndpoint}";
            OrdersEndpoint = $"{baseEndpoint}";
            BasketEndpoint = $"{baseEndpoint}";
            IdentityEndpoint = $"{baseEndpoint}connect/authorize";
            UserInfoEndpoint = $"{baseEndpoint}connect/userinfo";
            TokenEndpoint = $"{baseEndpoint}connect/token";
            LogoutEndpoint = $"{baseEndpoint}connect/endsession";
           IdentityCallback = $"{baseEndpoint}xamarincallback";
           // IdentityCallback = "http://421D6EA8F42B4F269D21672217D437FD/xamarincallback"; 

            LogoutCallback = $"{baseEndpoint}Account/Redirecting";
            LocationEndpoint = $"{baseEndpoint}";
            MarketingEndpoint = $"{baseEndpoint}";



        }
        private void UpdateApiEndpoint(string baseEndpoint)
        {

            CatalogEndpoint = $"{baseEndpoint}5101";
            OrdersEndpoint = $"{baseEndpoint}5102";
            BasketEndpoint = $"{baseEndpoint}5103";


            LocationEndpoint = $"{baseEndpoint}5109";
            MarketingEndpoint = $"{baseEndpoint}5110";

            DriverEndPoint = $"{ApiEndpoint}drivers";
          

        }
    }
}
