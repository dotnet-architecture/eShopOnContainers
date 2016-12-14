using eShopOnContainers.Core.Extensions;
using eShopOnContainers.Core.Models.Orders;
using eShopOnContainers.Core.Models.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace eShopOnContainers.Core.Services.Order
{
    public class OrderMockService : IOrderService
    {
        private static DateTime MockExpirationDate = DateTime.Now.AddYears(5);

        private static Address MockAdress = new Address
        {
            Id = Guid.NewGuid(),
            City = "Seattle, WA",
            Street = "120 E 87th Street",
            CountryCode = "98122",
            Country = "United States",
            Latitude = 40.785091,
            Longitude = -73.968285,
            State = "Seattle",
            StateCode = "WA",
            ZipCode = "98101"
        };

        private static PaymentInfo MockPaymentInfo = new PaymentInfo
        {
            Id = Guid.NewGuid(),
            CardHolderName = "American Express",
            CardNumber = "378282246310005",
            CardType = new CardType
            {
                Id = 3,
                Name = "MasterCard"
            },
            Expiration = MockExpirationDate.ToString(),
            ExpirationMonth = MockExpirationDate.Month,
            ExpirationYear = MockExpirationDate.Year,
            SecurityNumber = "123"
        };

        private List<Models.Orders.Order> MockOrders = new List<Models.Orders.Order>()
        {
            new Models.Orders.Order { SequenceNumber = 123, OrderDate = DateTime.Now, State = OrderState.Delivered, OrderItems = MockOrderItems, PaymentInfo = MockPaymentInfo, ShippingAddress = MockAdress },
            new Models.Orders.Order { SequenceNumber = 132, OrderDate = DateTime.Now, State = OrderState.Delivered, OrderItems = MockOrderItems, PaymentInfo = MockPaymentInfo, ShippingAddress = MockAdress },
            new Models.Orders.Order { SequenceNumber = 231, OrderDate = DateTime.Now, State = OrderState.Delivered, OrderItems = MockOrderItems, PaymentInfo = MockPaymentInfo, ShippingAddress = MockAdress },
        };

        private static List<OrderItem> MockOrderItems = new List<OrderItem>()
        {
            new OrderItem { OrderId = Guid.NewGuid(), ProductId = Common.Common.MockCatalogItemId01, Discount = 15, ProductName = ".NET Bot Blue Sweatshirt (M)", Quantity = 1, UnitPrice = 16.50M },
            new OrderItem { OrderId = Guid.NewGuid(), ProductId = Common.Common.MockCatalogItemId03, Discount = 0, ProductName = ".NET Bot Black Sweatshirt (M)", Quantity = 2, UnitPrice = 19.95M }
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