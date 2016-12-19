using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eShopOnContainers.Core.Services.Identity
{
    public class IdentityService : IIdentityService
    {
        public string CreateAuthorizeRequest()
        {
            // Create URI to authorize endpoint
            var authorizeRequest =
                new AuthorizeRequest(GlobalSetting.Instance.IdentityEndpoint);

            // Dictionary with values for the authorize request
            var dic = new Dictionary<string, string>();
            dic.Add("client_id", "xamarin");
            dic.Add("response_type", "id_token token");
            dic.Add("scope", "openid profile basket orders");

            dic.Add("redirect_uri", GlobalSetting.Instance.IdentityCallback);
            dic.Add("nonce", Guid.NewGuid().ToString("N"));

            // Add CSRF token to protect against cross-site request forgery attacks.
            var currentCSRFToken = Guid.NewGuid().ToString("N");
            dic.Add("state", currentCSRFToken);

            var authorizeUri = authorizeRequest.Create(dic);

            return authorizeUri;
        }

        public string CreateLogoutRequest(string token)
        {
            if(string.IsNullOrEmpty(token))
            {
                return string.Empty;
            }

            return string.Format("{0}?id_token_hint={1}&post_logout_redirect_uri={2}", 
                GlobalSetting.Instance.LogoutEndpoint,
                token,
                GlobalSetting.Instance.LogoutCallback);
        }

        public string DecodeToken(string token)
        {
            var parts = token.Split('.');

            string partToConvert = parts[1];
            partToConvert = partToConvert.Replace('-', '+');
            partToConvert = partToConvert.Replace('_', '/');
            switch (partToConvert.Length % 4)
            {
                case 0:
                    break;
                case 2:
                    partToConvert += "==";
                    break;
                case 3:
                    partToConvert += "=";
                    break;
            }

            var partAsBytes = Convert.FromBase64String(partToConvert);
            var partAsUTF8String = Encoding.UTF8.GetString(partAsBytes, 0, partAsBytes.Count());

            return JObject.Parse(partAsUTF8String).ToString();
        }
    }
}
