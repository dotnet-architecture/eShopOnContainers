using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Services.Basket.API.Model
{
    public interface IBasketRepository
    {
        Task<CustomerBasket> GetBasket(string customerId);
        Task<CustomerBasket> UpdateBasket(CustomerBasket basket);
        Task<bool> DeleteBasket(string id);
    }
}
