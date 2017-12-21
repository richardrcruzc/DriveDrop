namespace DriveDrop.Core
{
    public class GlobalSetting
    {
        public const string AzureTag = "Azure";
        public const string MockTag = "Mock";
        public const string DefaultEndpoint = "http://identity.godrivedrop.com/";
       public const string CallBackEndpoint = "http://api.godrivedrop.com/";

        //public const string DefaultEndpoint = "http://10.0.0.51:58652/";
        //public const string CallBackEndpoint = "http://api.godrivedrop.com/";

        private string _baseEndpoint;
        private static readonly GlobalSetting _instance = new GlobalSetting();

        public GlobalSetting()
        {
            AuthToken = "INSERT AUTHENTICATION TOKEN";
            BaseEndpoint = DefaultEndpoint;
            ApiEndpoint = CallBackEndpoint;
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
                UpdateApiEndpoint(_baseEndpoint);
            }
        }

        public string ClientId { get { return "xamarin"; }}

        public string ClientSecret { get { return "secret"; }}

        public string AuthToken { get; set; }

        public string RegisterWebsite { get; set; }

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

        private void UpdateEndpoint(string baseEndpoint)
        {
            RegisterWebsite = $"{baseEndpoint}Account/Register";
              IdentityEndpoint = $"{baseEndpoint}connect/authorize";
             TokenEndpoint = $"{baseEndpoint}connect/token";
            LogoutEndpoint = $"{baseEndpoint}connect/endsession";
            IdentityCallback = $"{baseEndpoint}xamarincallback";
            LogoutCallback = $"{baseEndpoint}Account/Redirecting";
            
        }
        private void UpdateApiEndpoint(string baseEndpoint)
        {
            CatalogEndpoint = $"{baseEndpoint}5101";
            OrdersEndpoint = $"{baseEndpoint}5102";
            BasketEndpoint = $"{baseEndpoint}5103";
         
            LocationEndpoint = $"{baseEndpoint}5109";
            MarketingEndpoint = $"{baseEndpoint}5110";
        }
    }
}