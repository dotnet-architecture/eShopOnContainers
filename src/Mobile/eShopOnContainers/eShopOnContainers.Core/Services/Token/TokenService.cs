using System.Net;
using System.Threading.Tasks;
using eShopOnContainers.Core.Services.RequestProvider;
using eShopOnContainers.Core.Models.Token;

namespace eShopOnContainers.Core.Services.Token
{
    public class TokenService : ITokenService
    {
        private readonly IRequestProvider _requestProvider;

        public TokenService(IRequestProvider requestProvider)
        {
            _requestProvider = requestProvider;
        }

        public async Task<UserToken> GetTokenAsync(string code)
        {
            string data = string.Format("grant_type=authorization_code&code={0}&redirect_uri={1}", code, WebUtility.UrlEncode(GlobalSetting.Instance.IdentityCallback));
            var token = await _requestProvider.PostAsync<UserToken>(GlobalSetting.Instance.TokenEndpoint, data, GlobalSetting.Instance.ClientId, GlobalSetting.Instance.ClientSecret);
            return token;
        }
    }
}
