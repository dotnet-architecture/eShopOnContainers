namespace eShopOnContainers.Core
{
    public class GlobalSetting
    {
        public const string AzureTag = "Azure";
        public const string MockTag = "Mock";
        public const string DefaultEndpoint = "http://13.88.8.119";

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

        public string ClientId { get { return "xamarin"; }}

        public string ClientSecret { get { return "secret"; }}

        public string AuthToken { get; set; }

        public string RegisterWebsite { get; set; }

        public string CatalogEndpoint { get; set; }

        public string OrdersEndpoint { get; set; }

        public string BasketEndpoint { get; set; }

        public string IdentityEndpoint { get; set; }

        public string LocationEndpoint { get; set; }

        public string UserInfoEndpoint { get; set; }

        public string TokenEndpoint { get; set; }

        public string LogoutEndpoint { get; set; }

        public string IdentityCallback { get; set; }

        public string LogoutCallback { get; set; }

        private void UpdateEndpoint(string baseEndpoint)
        {
            RegisterWebsite = string.Format("{0}:5105/Account/Register", baseEndpoint);
            CatalogEndpoint = string.Format("{0}:5101", baseEndpoint);
            OrdersEndpoint = string.Format("{0}:5102", baseEndpoint);
            BasketEndpoint = string.Format("{0}:5103", baseEndpoint);
            IdentityEndpoint = string.Format("{0}:5105/connect/authorize", baseEndpoint);
            UserInfoEndpoint = string.Format("{0}:5105/connect/userinfo", baseEndpoint);
            TokenEndpoint = string.Format("{0}:5105/connect/token", baseEndpoint);
            LogoutEndpoint = string.Format("{0}:5105/connect/endsession", baseEndpoint);
            IdentityCallback = string.Format("{0}:5105/xamarincallback", baseEndpoint);
            LogoutCallback = string.Format("{0}:5105/Account/Redirecting", baseEndpoint);
            LocationEndpoint = string.Format("{0}:5109", baseEndpoint);
        }
    }
}