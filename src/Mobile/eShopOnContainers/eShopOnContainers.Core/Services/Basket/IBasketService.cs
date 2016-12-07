using eShopOnContainers.Core.Models.Basket;
using System.Threading.Tasks;

namespace eShopOnContainers.Core.Services.Basket
{
    public interface IBasketService
    {
        Task<CustomerBasket> GetBasketAsync(string guidUser, string token);
        Task<CustomerBasket> UpdateBasketAsync(CustomerBasket customerBasket);
        Task ClearBasketAsync(string guidUser);
    }
}