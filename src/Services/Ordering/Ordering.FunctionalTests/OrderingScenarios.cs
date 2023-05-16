using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Ordering.FunctionalTests
{
    public class OrderingScenarios
        : OrderingScenarioBase
    {
        [Fact]
        public async Task Get_get_all_stored_orders_and_response_ok_status_code()
        {
            using var server = CreateServer();
            var response = await server.CreateClient()
                .GetAsync(Get.Orders);

            var s = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Cancel_order_no_order_created_bad_request_response()
        {
            using var server = CreateServer();
            var content = new StringContent(BuildOrder(), UTF8Encoding.UTF8, "application/json")
            {
                Headers = { { "x-requestid", Guid.NewGuid().ToString() } }
            };
            var response = await server.CreateClient()
                .PutAsync(Put.CancelOrder, content);

            var s = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Ship_order_no_order_created_bad_request_response()
        {
            using var server = CreateServer();
            var content = new StringContent(BuildOrder(), UTF8Encoding.UTF8, "application/json")
            {
                Headers = { { "x-requestid", Guid.NewGuid().ToString() } }
            };
            var response = await server.CreateClient()
                .PutAsync(Put.ShipOrder, content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        string BuildOrder()
        {
            var order = new
            {
                OrderNumber = "-1"
            };
            return JsonSerializer.Serialize(order);
        }
    }
}
