namespace IntegrationTests.Services.Ordering
{
    using Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;
    using Newtonsoft.Json;
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;
    using static Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands.CreateOrderCommand;

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
        public async Task AddNewOrder_add_new_order_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var content = new StringContent(BuildOrder(), UTF8Encoding.UTF8, "application/json");
                var response = await server.CreateClient()
                    .PostAsync(Post.AddNewOrder, content);

                response.EnsureSuccessStatusCode();
            }
        }

        [Fact]
        public async Task AddNewOrder_response_bad_request_if_card_expiration_is_invalid()
        {
            using (var server = CreateServer())
            {
                var content = new StringContent(BuildOrderWithInvalidExperationTime(), UTF8Encoding.UTF8, "application/json");

                var response = await server.CreateClient()
                    .PostAsync(Post.AddNewOrder, content);

                Assert.True(response.StatusCode == System.Net.HttpStatusCode.BadRequest);
            }
        }

        string BuildOrder()
        {
            var order = new CreateOrderCommand(
                cardExpiration: DateTime.UtcNow.AddYears(1),
                cardNumber: "5145-555-5555",
                cardHolderName: "Jhon Senna",
                cardSecurityNumber: "232",
                cardTypeId: 1,
                city: "Redmon",
                country: "USA",
                state: "WA",
                street: "One way",
                zipcode: "zipcode"
            );

            order.AddOrderItem(new OrderItemDTO()
            {
                ProductId = 1,
                Discount = 10M,
                UnitPrice = 10,
                Units = 1,
                ProductName = "Some name"
            });

            return JsonConvert.SerializeObject(order);
        }
        string BuildOrderWithInvalidExperationTime()
        {
            var order = new CreateOrderCommand(
                cardExpiration: DateTime.UtcNow.AddYears(-1),
                cardNumber: "5145-555-5555",
                cardHolderName: "Jhon Senna",
                cardSecurityNumber: "232",
                cardTypeId: 1,
                city: "Redmon",
                country: "USA",
                state: "WA",
                street: "One way",
                zipcode: "zipcode"
            );

            return JsonConvert.SerializeObject(order);
        }
    }
}
