using eShopOnContainers.Core.Models.User;
using System.Threading.Tasks;

namespace eShopOnContainers.Core.Services.User
{
    public interface IUserService
    {
        Task<UserInfo> GetUserInfoAsync(string authToken);
    }
}
