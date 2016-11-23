using eShopOnContainers.Core.Models.Orders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eShopOnContainers.Core.Services.User
{
    public class UserMockService : IUserService
    {
        private Models.User.User MockUser = new Models.User.User
        {
            GuidUser = "9245fe4a-d402-451c-b9ed-9c1a04247482",
            Name = "Jhon",
            LastName = "Doe",
            City = "Seattle, WA",
            Street = "120 E 87th Street",
            CountryCode = "98122",
            Country = "United States"
        };

        private List<Order> MockOrders = new List<Order>()
        {
            new Order { SequenceNumber = 123, Total = 56.40M, OrderDate = DateTime.Now, Status = OrderStatus.Delivered, OrderItems = MockOrderItems },
            new Order { SequenceNumber = 132, Total = 56.40M, OrderDate = DateTime.Now, Status = OrderStatus.Delivered, OrderItems = MockOrderItems },
            new Order { SequenceNumber = 231, Total = 56.40M, OrderDate = DateTime.Now, Status = OrderStatus.Delivered, OrderItems = MockOrderItems },
        };

        private static List<OrderItem> MockOrderItems = new List<OrderItem>()
        {
            new OrderItem { OrderId = Guid.NewGuid(), ProductId = "1", Discount = 15, ProductName = ".NET Bot Blue Sweatshirt (M)", Quantity = 1, UnitPrice = 16.50M },
            new OrderItem { OrderId = Guid.NewGuid(), ProductId = "3", Discount = 0, ProductName = ".NET Bot Black Sweatshirt (M)", Quantity = 2, UnitPrice = 19.95M }
        };

        public async Task<Models.User.User> GetUserAsync()
        {
            await Task.Delay(500);

            return MockUser;
        }

        public async Task<List<Order>> GetOrdersAsync()
        {
            await Task.Delay(500);

            return MockOrders;
        }
    }
}