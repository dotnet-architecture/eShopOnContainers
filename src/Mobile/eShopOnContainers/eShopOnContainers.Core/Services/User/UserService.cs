using eShopOnContainers.Core.Models.User;
using eShopOnContainers.Core.Services.RequestProvider;
using System;
using System.Threading.Tasks;

namespace eShopOnContainers.Core.Services.User
{
    public class UserService : IUserService
    {
        private readonly IRequestProvider _requestProvider;

        public UserService(IRequestProvider requestProvider)
        {
            _requestProvider = requestProvider;
        }

        public async Task<UserInfo> GetUserInfoAsync(string authToken)
        {
            UriBuilder builder = new UriBuilder(GlobalSetting.Instance.UserInfoEndpoint);
            string uri = builder.ToString();
            var userInfo = await _requestProvider.GetAsync<UserInfo>(uri, authToken);
            return userInfo;
        }
    }
}