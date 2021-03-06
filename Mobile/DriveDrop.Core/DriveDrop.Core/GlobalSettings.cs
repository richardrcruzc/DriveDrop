﻿namespace DriveDrop.Core
{
    public class GlobalSetting
    {

        public   string _user = "admin@driveDrop.com";
        public   string _passworrd = "Pass@word1";


        public const string AzureTag = "Azure";
        public const string MockTag = "Mock";

        // public const string DefaultEndpoint = "http://identity.godrivedrop.com/";
        //public const string DriveDropEndpoint = "http://api.godrivedrop.com/api/v1/";

        
        //public const string DefaultEndpoint = "http://10.0.0.51:5205/";
        public const string DefaultEndpoint = "http://godrivedrop.azurewebsites.net/";


        private string _baseEndpoint;
        private string _drivedrppEndpoint;
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
            get { return _drivedrppEndpoint; }
            set
            {
                _drivedrppEndpoint = value;
                UpdateApiEndpoint(_drivedrppEndpoint);
            }
        }

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



        public string DriverEndPoint { get; set; }

        public string UserValidation { get; set; }


        public string CustomerId { get; set; }
        public string CommonEndpoint { get; set; }


        public string User {
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


        private void UpdateEndpoint(string baseEndpoint)
        {
            RegisterWebsite = $"{baseEndpoint}/home/register";
            RegisterSender = $"{baseEndpoint}Sender/NewSender";
            RegisterDriver = $"{baseEndpoint}Driver/NewDriver/NewDriver";
            IdentityEndpoint = $"{baseEndpoint}Account/Login";
            TokenEndpoint = $"{baseEndpoint}connect/token";
            LogoutEndpoint = $"{baseEndpoint}connect/endsession";
            IdentityCallback = $"{baseEndpoint}xamarincallback";
            LogoutCallback = $"{baseEndpoint}Account/Redirecting";
            UserInfoEndpoint = $"{baseEndpoint}connect/userinfo";

            UserValidation = $"{baseEndpoint}Account/XamariLogin";
        }
        private void UpdateApiEndpoint(string baseEndpoint)
        {

            CatalogEndpoint = $"{baseEndpoint}5101";
            OrdersEndpoint = $"{baseEndpoint}5102";
            BasketEndpoint = $"{baseEndpoint}5103";


            LocationEndpoint = $"{baseEndpoint}5109";
            MarketingEndpoint = $"{baseEndpoint}5110";

            DriverEndPoint = $"{ApiEndpoint}drivers";
            CommonEndpoint = $"{ApiEndpoint}common";

        }
    }
}