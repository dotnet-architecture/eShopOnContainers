using eShopOnContainers.Core.Models.Basket;
using System.Threading.Tasks;

namespace eShopOnContainers.Core.Services.Basket
{
    public interface IBasketService
    {
        Task<CustomerBasket> GetBasketAsync(string guidUser);
        Task<CustomerBasket> UpdateBasketAsync(CustomerBasket customerBasket);
    }
}