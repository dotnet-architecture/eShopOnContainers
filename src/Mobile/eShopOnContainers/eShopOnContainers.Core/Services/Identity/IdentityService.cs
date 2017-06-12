using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using eShopOnContainers.Core.Services.RequestProvider;
using eShopOnContainers.Core.Models.Token;

namespace eShopOnContainers.Core.Services.Identity
{
    public class IdentityService : IIdentityService
    {
		private readonly IRequestProvider _requestProvider;

		public IdentityService(IRequestProvider requestProvider)
		{
			_requestProvider = requestProvider;
		}

        public string CreateAuthorizationRequest()
        {
            // Create URI to authorization endpoint
            var authorizeRequest = new AuthorizeRequest(GlobalSetting.Instance.IdentityEndpoint);

            // Dictionary with values for the authorize request
            var dic = new Dictionary<string, string>();
            dic.Add("client_id", GlobalSetting.Instance.ClientId);
            dic.Add("client_secret", GlobalSetting.Instance.ClientSecret); 
            dic.Add("response_type", "code id_token");
            dic.Add("scope", "openid profile basket orders offline_access");
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
            if (string.IsNullOrEmpty(token))
            {
                return string.Empty;
            }

            return string.Format("{0}?id_token_hint={1}&post_logout_redirect_uri={2}", 
                GlobalSetting.Instance.LogoutEndpoint,
                token,
                GlobalSetting.Instance.LogoutCallback);
        }

		public async Task<UserToken> GetTokenAsync(string code)
		{
			string data = string.Format("grant_type=authorization_code&code={0}&redirect_uri={1}", code, WebUtility.UrlEncode(GlobalSetting.Instance.IdentityCallback));
			var token = await _requestProvider.PostAsync<UserToken>(GlobalSetting.Instance.TokenEndpoint, data, GlobalSetting.Instance.ClientId, GlobalSetting.Instance.ClientSecret);
			return token;
		}
    }
}
