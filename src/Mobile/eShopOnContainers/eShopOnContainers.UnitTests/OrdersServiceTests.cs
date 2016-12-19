using eShopOnContainers.Core;
using eShopOnContainers.Core.Services.Order;
using eShopOnContainers.Core.Services.RequestProvider;
using System.Threading.Tasks;
using Xunit;

namespace eShopOnContainers.UnitTests
{
    public class OrdersServiceTests
    {
        [Fact]
        public async Task GetFakeOrdersTest()
        {
            var ordersMockService = new OrderMockService();
            var result = await ordersMockService.GetOrdersAsync(GlobalSetting.Instance.AuthToken);

            Assert.NotEqual(0, result.Count);
        }

        [Fact]
        public async Task GetOrdersTest()
        {
            var requestProvider = new RequestProvider();
            var ordersService = new OrderService(requestProvider);
            var result = await ordersService.GetOrdersAsync(GlobalSetting.Instance.AuthToken);

            Assert.NotEqual(0, result.Count);
        }
    }
}