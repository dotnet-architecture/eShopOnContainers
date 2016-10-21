using Microsoft.eShopOnContainers.WebMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    public interface IOrderingService
    {
        List<Order> GetOrders();
        Order GetOrder(Guid Id);
        void AddOrder(Order Order);
    }
}
