using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Services
{
    public interface IIdentityService
    {
        string GetUserIdentity();
        string GetUserName();
        Task<string> GetUserToken();
    }
}
