namespace IntegrationTests.Services.Ordering
{
    using IntegrationTests.Services.Extensions;
    using Newtonsoft.Json;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using WebMVC.Models;
    using Xunit;

    public class OrderingScenarios
        : OrderingScenarioBase
    {
        [Fact]
        public async Task Get_get_all_stored_orders_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Get.Orders);

                response.EnsureSuccessStatusCode();
            }
        }

        [Fact]
        public async Task Cancel_order_no_order_created_bad_request_response()
        {
            using (var server = CreateServer())
            {
                var content = new StringContent(BuildOrder(), UTF8Encoding.UTF8, "application/json");
                var response = await server.CreateIdempotentClient()
                    .PutAsync(Put.CancelOrder, content);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Fact]
        public async Task Ship_order_no_order_created_bad_request_response()
        {
            using (var server = CreateServer())
            {
                var content = new StringContent(BuildOrder(), UTF8Encoding.UTF8, "application/json");
                var response = await server.CreateIdempotentClient()
                    .PutAsync(Put.ShipOrder, content);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        string BuildOrder()
        {
            var order = new OrderDTO()
            {
                OrderNumber = "-1"
            };
            return JsonConvert.SerializeObject(order);
        }        
    }        
}
