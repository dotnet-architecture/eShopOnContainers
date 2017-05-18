using eShopOnContainers.Core.Extensions;
using eShopOnContainers.Core.Models.Orders;
using eShopOnContainers.Core.Models.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

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
            new Models.Orders.Order { OrderNumber = "1", SequenceNumber = 123, OrderDate = DateTime.Now, State = OrderState.Delivered, OrderItems = MockOrderItems, CardTypeId = MockPaymentInfo.CardType.Id, CardHolderName = MockPaymentInfo.CardHolderName, CardNumber = MockPaymentInfo.CardNumber, CardSecurityNumber = MockPaymentInfo.SecurityNumber, CardExpiration = new DateTime(MockPaymentInfo.ExpirationYear, MockPaymentInfo.ExpirationMonth, 1), ShippingCity = MockAdress.City, ShippingState = MockAdress.State, ShippingCountry = MockAdress.Country, ShippingStreet = MockAdress.Street, Total = 36.46M },
            new Models.Orders.Order { OrderNumber = "2", SequenceNumber = 132, OrderDate = DateTime.Now, State = OrderState.Delivered, OrderItems = MockOrderItems, CardTypeId = MockPaymentInfo.CardType.Id, CardHolderName = MockPaymentInfo.CardHolderName, CardNumber = MockPaymentInfo.CardNumber, CardSecurityNumber = MockPaymentInfo.SecurityNumber, CardExpiration = new DateTime(MockPaymentInfo.ExpirationYear, MockPaymentInfo.ExpirationMonth, 1), ShippingCity = MockAdress.City, ShippingState = MockAdress.State, ShippingCountry = MockAdress.Country, ShippingStreet = MockAdress.Street, Total = 36.46M },
            new Models.Orders.Order { OrderNumber = "3", SequenceNumber = 231, OrderDate = DateTime.Now, State = OrderState.Delivered, OrderItems = MockOrderItems, CardTypeId = MockPaymentInfo.CardType.Id, CardHolderName = MockPaymentInfo.CardHolderName, CardNumber = MockPaymentInfo.CardNumber, CardSecurityNumber = MockPaymentInfo.SecurityNumber, CardExpiration = new DateTime(MockPaymentInfo.ExpirationYear, MockPaymentInfo.ExpirationMonth, 1), ShippingCity = MockAdress.City, ShippingState = MockAdress.State, ShippingCountry = MockAdress.Country, ShippingStreet = MockAdress.Street, Total = 36.46M }
        };

        private static List<OrderItem> MockOrderItems = new List<OrderItem>()
        {
			new OrderItem { OrderId = Guid.NewGuid(), ProductId = Common.Common.MockCatalogItemId01, Discount = 15, ProductName = ".NET Bot Blue Sweatshirt (M)", Quantity = 1, UnitPrice = 16.50M, PictureUrl = Device.RuntimePlatform != Device.Windows ? "fake_product_01.png" : "Assets/fake_product_01.png" },
			new OrderItem { OrderId = Guid.NewGuid(), ProductId = Common.Common.MockCatalogItemId03, Discount = 0, ProductName = ".NET Bot Black Sweatshirt (M)", Quantity = 2, UnitPrice = 19.95M, PictureUrl = Device.RuntimePlatform != Device.Windows ? "fake_product_03.png" : "Assets/fake_product_03.png" }
        };

        private static List<CardType> MockCardTypes = new List<CardType>()
        {
            new CardType { Id = 1, Name = "Amex" },
            new CardType { Id = 2, Name = "Visa" },
            new CardType { Id = 3, Name = "MasterCard" },
        };

        public async Task CreateOrderAsync(Models.Orders.Order newOrder, string token)
        {
            await Task.Delay(500);

            if (!string.IsNullOrEmpty(token))
            {
                newOrder.OrderNumber = string.Format("{0}", MockOrders.Count + 1);

                MockOrders.Insert(0, newOrder);
            }
        }

        public async Task<ObservableCollection<Models.Orders.Order>> GetOrdersAsync(string token)
        {
            await Task.Delay(500);

            if (!string.IsNullOrEmpty(token))
            {
                return MockOrders
                    .OrderByDescending(o => o.OrderNumber)
                    .ToObservableCollection();
            }
            else
                return new ObservableCollection<Models.Orders.Order>();
        }

        public async Task<Models.Orders.Order> GetOrderAsync(int orderId, string token)
        {
            await Task.Delay(500);

            if (!string.IsNullOrEmpty(token))
                return MockOrders
                    .FirstOrDefault(o => o.OrderNumber.Equals(orderId.ToString(), 
                    StringComparison.CurrentCultureIgnoreCase));
            else
                return new Models.Orders.Order();
        }

        public async Task<ObservableCollection<CardType>> GetCardTypesAsync(string token)
        {
            await Task.Delay(500);

            if (!string.IsNullOrEmpty(token))
                return MockCardTypes.ToObservableCollection();
            else
                return new ObservableCollection<CardType>();
        }
    }
}