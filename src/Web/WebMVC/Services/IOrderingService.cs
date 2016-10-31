using Microsoft.eShopOnContainers.WebMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    public interface IOrderingService
    {
        int ItemsInCart { get; }
        List<Order> GetMyOrders(ApplicationUser user);
        Order GetActiveOrder(ApplicationUser user);
        void AddToCart(ApplicationUser user, OrderItem product);
        Order GetOrder(ApplicationUser user, Guid orderId);
    }
}
