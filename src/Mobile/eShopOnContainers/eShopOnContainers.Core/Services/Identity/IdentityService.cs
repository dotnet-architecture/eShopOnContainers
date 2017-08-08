using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using eShopOnContainers.Core.Services.RequestProvider;
using eShopOnContainers.Core.Models.Token;
using IdentityModel.Client;
using PCLCrypto;

namespace eShopOnContainers.Core.Services.Identity
{
    public class IdentityService : IIdentityService
    {
		private readonly IRequestProvider _requestProvider;
        private string _codeVerifier;

		public IdentityService(IRequestProvider requestProvider)
		{
			_requestProvider = requestProvider;
		}

        public string CreateAuthorizationRequest()
        {
            // Create URI to authorization endpoint
            var authorizeRequest = new AuthorizeRequest(GlobalSetting.Instance.IdentityEndpoint);

            // Create code verifier for PKCE
            _codeVerifier = RandomDataBase64Url(32);

            // Dictionary with values for the authorize request
            var dic = new Dictionary<string, string>();
            dic.Add("client_id", GlobalSetting.Instance.ClientId);
            dic.Add("client_secret", GlobalSetting.Instance.ClientSecret); 
            dic.Add("response_type", "code id_token");
            dic.Add("scope", "openid profile basket orders locations marketing offline_access");
            dic.Add("redirect_uri", GlobalSetting.Instance.IdentityCallback);
            dic.Add("nonce", Guid.NewGuid().ToString("N"));
            dic.Add("code_challenge", Base64UrlEncodeNoPadding(Sha256(_codeVerifier)));
            dic.Add("code_challenge_method", "S256");

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
			string data = string.Format("grant_type=authorization_code&code={0}&redirect_uri={1}&code_verifier={2}", code, WebUtility.UrlEncode(GlobalSetting.Instance.IdentityCallback), _codeVerifier);
			var token = await _requestProvider.PostAsync<UserToken>(GlobalSetting.Instance.TokenEndpoint, data, GlobalSetting.Instance.ClientId, GlobalSetting.Instance.ClientSecret);
			return token;
		}

        private string RandomDataBase64Url(int length)
        {
            byte[] bytes = WinRTCrypto.CryptographicBuffer.GenerateRandom(length);
            return Base64UrlEncodeNoPadding(bytes);
        }

        private byte[] Sha256(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            var sha256 = WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha256);
            return sha256.HashData(bytes);
        }

        private string Base64UrlEncodeNoPadding(byte[] buffer)
        {
            string base64 = Convert.ToBase64String(buffer);
            base64 = base64.Replace("+", "-");
            base64 = base64.Replace("/", "_");
            base64 = base64.Replace("=", string.Empty);
            return base64;
        }
    }
}
