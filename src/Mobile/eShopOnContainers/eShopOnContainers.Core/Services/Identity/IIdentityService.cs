using System.Threading.Tasks;

namespace eShopOnContainers.Core.Services.Identity
{
    public interface IIdentityService
    {
        string CreateAuthorizeRequest();
        string CreateLogoutRequest(string token);
        string DecodeToken(string token);
    }
}