namespace FunctionalTests.Services.Ordering
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
        public async Task Get_get_order_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Get.OrderBy(31));

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
            var order = new CreateOrderCommand()
            {
                CardExpiration = DateTime.UtcNow.AddYears(1),
                CardNumber = "5145-555-5555",
                CardHolderName = "Jhon Senna",
                CardSecurityNumber = "232",
                CardTypeId = 1,
                City = "Redmon",
                Country = "USA",
                State = "WA",
                Street = "One way",
                ZipCode = "zipcode",
            };

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
            var order = new CreateOrderCommand()
            {
                CardExpiration = DateTime.UtcNow.AddYears(-1),
                CardNumber = "5145-555-5555",
                CardHolderName = "Jhon Senna",
                CardSecurityNumber = "232",
                CardTypeId = 1,
                City = "Redmon",
                Country = "USA",
                State = "WA",
                Street = "One way",
                ZipCode = "zipcode"
            };

            return JsonConvert.SerializeObject(order);
        }
    }
}
