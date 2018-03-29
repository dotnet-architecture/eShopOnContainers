using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Services.Identity.API.Services
{
    public interface ILoginService<T>
    {
        Task<bool> ValidateCredentialsAsync(T user, string password);
        Task<T> FindByUsernameAsync(string user);
        Task SignInAsync(T user);
    }
}
