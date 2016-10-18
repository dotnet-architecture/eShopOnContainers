using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Services.Basket.API.Model
{
    public interface IBasketRepository
    {
        Task<CustomerBasket> GetBasket(Guid customerId);
        Task<bool> UpdateBasket(CustomerBasket basket);
        Task<bool> DeleteBasket(Guid id);
    }
}
