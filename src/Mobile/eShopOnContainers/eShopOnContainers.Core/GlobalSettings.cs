namespace eShopOnContainers.Core
{
    public class GlobalSetting
    {
        public const string AzureTag = "Azure";
        public const string MockTag = "Mock";
        public const string DefaultEndpoint = "http://YOUR_IP_OR_DNS_NAME"; // i.e.: "http://YOUR_IP" or "http://YOUR_DNS_NAME"

        private string _baseIdentityEndpoint;
        private string _baseGatewayShoppingEndpoint;
        private string _baseGatewayMarketingEndpoint;

        public GlobalSetting()
        {
            AuthToken = "INSERT AUTHENTICATION TOKEN";

            BaseIdentityEndpoint = DefaultEndpoint;
            BaseGatewayShoppingEndpoint = DefaultEndpoint;
            BaseGatewayMarketingEndpoint = DefaultEndpoint;
        }

        public static GlobalSetting Instance { get; } = new GlobalSetting();

        public string BaseIdentityEndpoint
        {
            get { return _baseIdentityEndpoint; }
            set
            {
                _baseIdentityEndpoint = value;
                UpdateEndpoint(_baseIdentityEndpoint);
            }
        }

        public string BaseGatewayShoppingEndpoint
        {
            get { return _baseGatewayShoppingEndpoint; }
            set
            {
                _baseGatewayShoppingEndpoint = value;
                UpdateGatewayShoppingEndpoint(_baseGatewayShoppingEndpoint);
            }
        }

        public string BaseGatewayMarketingEndpoint
        {
            get { return _baseGatewayMarketingEndpoint; }
            set
            {
                _baseGatewayMarketingEndpoint = value;
                UpdateGatewayMarketingEndpoint(_baseGatewayMarketingEndpoint);
            }
        }

        public string ClientId { get { return "xamarin"; } }

        public string ClientSecret { get { return "secret"; } }

        public string AuthToken { get; set; }

        public string RegisterWebsite { get; set; }

        public string IdentityEndpoint { get; set; }

        public string UserInfoEndpoint { get; set; }

        public string TokenEndpoint { get; set; }

        public string LogoutEndpoint { get; set; }

        public string IdentityCallback { get; set; }

        public string LogoutCallback { get; set; }

        public string GatewayShoppingEndpoint { get; set; }

        public string GatewayMarketingEndpoint { get; set; }

        private void UpdateEndpoint(string endpoint)
        {
            var identityBaseEndpoint = $"{endpoint}/identity";
            RegisterWebsite = $"{identityBaseEndpoint}/Account/Register";
            LogoutCallback = $"{identityBaseEndpoint}/Account/Redirecting";

            var connectBaseEndpoint = $"{identityBaseEndpoint}/connect";
            IdentityEndpoint = $"{connectBaseEndpoint}/authorize";
            UserInfoEndpoint = $"{connectBaseEndpoint}/userinfo";
            TokenEndpoint = $"{connectBaseEndpoint}/token";
            LogoutEndpoint = $"{connectBaseEndpoint}/endsession";

            IdentityCallback = $"{endpoint}/xamarincallback";
        }

        private void UpdateGatewayShoppingEndpoint(string endpoint)
        {
            GatewayShoppingEndpoint = $"{endpoint}/mobileshoppingapigw";
        }

        private void UpdateGatewayMarketingEndpoint(string endpoint)
        {
            GatewayMarketingEndpoint = $"{endpoint}/mobilemarketingapigw";
        }
    }
}