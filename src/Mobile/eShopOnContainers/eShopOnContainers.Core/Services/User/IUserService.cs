using System.Threading.Tasks;

namespace eShopOnContainers.Core.Services.User
{
    public interface IUserService
    {
        Task<Models.User.User> GetUserAsync();
    }
}
