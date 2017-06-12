using eShopOnContainers.Core.Models.Token;
using System.Threading.Tasks;

namespace eShopOnContainers.Core.Services.Token
{
    public interface ITokenService
    {
        Task<UserToken> GetTokenAsync(string code);
    }
}
