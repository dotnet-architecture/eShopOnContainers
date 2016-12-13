using System;
using eShopOnContainers.Core.Models.User;
using System.Threading.Tasks;
using eShopOnContainers.Core.Models.Orders;

namespace eShopOnContainers.Core.Services.User
{
    public class UserMockService : IUserService
    {
        private static DateTime MockExpirationDate = DateTime.Now.AddYears(5);

        private Address MockAdress = new Address
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

        private PaymentInfo MockPaymentInfo = new PaymentInfo
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

        public async Task<Address> GetAddressAsync()
        {
            await Task.Delay(500);

            return MockAdress;
        }

        public async Task<PaymentInfo> GetPaymentInfoAsync()
        {
            await Task.Delay(500);

            return MockPaymentInfo;
        }
    }
}
