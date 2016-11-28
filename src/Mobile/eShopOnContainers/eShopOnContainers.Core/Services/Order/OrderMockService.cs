using eShopOnContainers.Core.Extensions;
using eShopOnContainers.Core.Models.Orders;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace eShopOnContainers.Core.Services.Order
{
    public class OrderMockService : IOrderService
    {
        private List<Models.Orders.Order> MockOrders = new List<Models.Orders.Order>()
        {
            new Models.Orders.Order { SequenceNumber = 123, Total = 56.40M, OrderDate = DateTime.Now, Status = OrderStatus.Delivered, OrderItems = MockOrderItems },
            new Models.Orders.Order { SequenceNumber = 132, Total = 56.40M, OrderDate = DateTime.Now, Status = OrderStatus.Delivered, OrderItems = MockOrderItems },
            new Models.Orders.Order { SequenceNumber = 231, Total = 56.40M, OrderDate = DateTime.Now, Status = OrderStatus.Delivered, OrderItems = MockOrderItems },
        };

        private static List<OrderItem> MockOrderItems = new List<OrderItem>()
        {
            new OrderItem { OrderId = Guid.NewGuid(), ProductId = "1", Discount = 15, ProductName = ".NET Bot Blue Sweatshirt (M)", Quantity = 1, UnitPrice = 16.50M },
            new OrderItem { OrderId = Guid.NewGuid(), ProductId = "3", Discount = 0, ProductName = ".NET Bot Black Sweatshirt (M)", Quantity = 2, UnitPrice = 19.95M }
        };

        private static List<CardType> MockCardTypes = new List<CardType>()
        {
            new CardType { Id = 1, Name = "Amex" },
            new CardType { Id = 2, Name = "Visa" },
            new CardType { Id = 3, Name = "MasterCard" },
        };

        public async Task CreateOrderAsync(Models.Orders.Order newOrder)
        {
            await Task.Delay(500);

            MockOrders.Insert(0, newOrder);
        }

        public async Task<ObservableCollection<Models.Orders.Order>> GetOrdersAsync()
        {
            await Task.Delay(500);

            return MockOrders.ToObservableCollection();
        }

        public async Task<Models.Orders.Order> GetOrderAsync(int orderId)
        {
            await Task.Delay(500);

            return MockOrders.FirstOrDefault(o => o.SequenceNumber == orderId);
        }

        public async Task<ObservableCollection<CardType>> GetCardTypesAsync()
        {
            await Task.Delay(500);

            return MockCardTypes.ToObservableCollection();
        }
    }
}