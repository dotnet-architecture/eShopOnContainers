using PurchaseBff.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PurchaseBff.Services
{
    public interface IOrderApiClient
    {
        Task<OrderData> GetOrderDraftFromBasket(BasketData basket);
    }
}
