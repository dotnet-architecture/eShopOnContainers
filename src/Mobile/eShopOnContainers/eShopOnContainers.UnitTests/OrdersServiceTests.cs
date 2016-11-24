using eShopOnContainers.Core.Services.Basket;
using eShopOnContainers.Core.Services.User;
using System.Threading.Tasks;
using Xunit;

namespace eShopOnContainers.UnitTests
{
    public class OrdersServiceTests
    {
        [Fact]
        public async Task GetFakeOrdersTest()
        {
            var userMockService = new UserMockService();
            var user = await userMockService.GetUserAsync();

            var ordersMockService = new BasketMockService();
            var result = await ordersMockService.GetBasketAsync(user.GuidUser);
            Assert.NotEqual(0, result.Items.Count);
        }
    }
}