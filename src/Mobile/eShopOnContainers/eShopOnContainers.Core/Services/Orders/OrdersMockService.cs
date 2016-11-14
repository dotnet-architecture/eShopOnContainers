using eShopOnContainers.Core.Models.Orders;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace eShopOnContainers.Core.Services.Orders
{
    public class OrdersMockService : IOrdersService
    {
        public async Task<ObservableCollection<Order>> GetOrdersAsync()
        {
            await Task.Delay(500);

            return new ObservableCollection<Order>
            {
                new Order { SequenceNumber = 123, Total = 56.40, OrderDate = DateTime.Now, Status = OrderStatus.Delivered, OrderItems = GetOrderItems() },
                new Order { SequenceNumber = 132, Total = 56.40, OrderDate = DateTime.Now, Status = OrderStatus.Delivered, OrderItems = GetOrderItems() },
                new Order { SequenceNumber = 231, Total = 56.40, OrderDate = DateTime.Now, Status = OrderStatus.Delivered, OrderItems = GetOrderItems() },
            };
        }

        public async Task<Order> GetCartAsync()
        {
            await Task.Delay(500);

            return new Order { SequenceNumber = 0123456789, Total = 56.40, OrderDate = DateTime.Now, Status = OrderStatus.Pending, OrderItems = GetOrderItems() };
        }

        private List<OrderItem> GetOrderItems()
        {
            return new List<OrderItem>
            {
                new OrderItem { OrderId = Guid.NewGuid(), ProductId = 1, Discount = 15, ProductName = ".NET Bot Blue Sweatshirt (M)", Quantity = 1, UnitPrice = 16.50M },
                new OrderItem { OrderId = Guid.NewGuid(), ProductId = 3, Discount = 0, ProductName = ".NET Bot Black Sweatshirt (M)", Quantity = 2, UnitPrice = 19.95M }
            };
        }
    }
}