using Microsoft.eShopOnContainers.WebMVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Services.ModelDTOs;

namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    public interface IOrderingService
    {
        Task<List<Order>> GetMyOrders(ApplicationUser user);
        Task<Order> GetOrder(ApplicationUser user, string orderId);
        Task CancelOrder(string orderId);
        Task ShipOrder(string orderId);
        Order MapUserInfoIntoOrder(ApplicationUser user, Order order);
        BasketDTO MapOrderToBasket(Order order);
        void OverrideUserInfoIntoOrder(Order original, Order destination);
    }
}
