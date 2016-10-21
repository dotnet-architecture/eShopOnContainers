using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.eShopOnContainers.WebMVC.Models;

namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    public class OrderingService : IOrderingService
    {
        private List<Order> _orders;

        public OrderingService()
        {
            _orders = new List<Order>()
            {
                new Order()
                {
                    BuyerId = Guid.NewGuid(), OrderDate = DateTime.Now,
                    OrderItems = new List<OrderItem>()
                    {
                        new OrderItem() { UnitPrice = 12 }
                    }
                }
            };
        }

        public void AddOrder(Order Order)
        {
            _orders.Add(Order);
        }

        public Order GetOrder(Guid Id)
        {
            return _orders.Where(x => x.BuyerId == Id).FirstOrDefault();
        }

        public List<Order> GetOrders()
        {
            return _orders;
        }
    }
}
