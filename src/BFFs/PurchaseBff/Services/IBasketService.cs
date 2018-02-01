using PurchaseBff.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PurchaseBff.Services
{
    public interface IBasketService
    {
        Task<BasketData> GetById(string id);
        Task Update(BasketData currentBasket);

        OrderData MapBasketToOrder(BasketData basket, bool isDraft);
    }
}
