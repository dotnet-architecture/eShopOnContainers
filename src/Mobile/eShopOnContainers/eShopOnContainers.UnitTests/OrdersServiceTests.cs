using eShopOnContainers.Core.Services.Orders;
using System.Threading.Tasks;
using Xunit;

namespace eShopOnContainers.UnitTests
{
    public class OrdersServiceTests
    {
        [Fact]
        public async Task GetFakeOrdersTest()
        {
            var ordersMockService = new OrdersMockService();
            var result = await ordersMockService.GetOrdersAsync();
            Assert.NotEqual(0, result.Count);
        }
    }
}