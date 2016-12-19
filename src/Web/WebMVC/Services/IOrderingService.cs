using Microsoft.eShopOnContainers.WebMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    public interface IOrderingService
    {
        Task<List<Order>> GetMyOrders(ApplicationUser user);
        Task<Order> GetOrder(ApplicationUser user, string orderId);
        Task CreateOrder(Order order);
        Order MapUserInfoIntoOrder(ApplicationUser user, Order order);
        void OverrideUserInfoIntoOrder(Order original, Order destination);
    }
}
