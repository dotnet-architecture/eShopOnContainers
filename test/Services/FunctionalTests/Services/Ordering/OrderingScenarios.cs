namespace FunctionalTests.Services.Ordering
{
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.eShopOnContainers.Services.Ordering.API.Models;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain;
    using Newtonsoft.Json;
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
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
                    .PostAsync(Post.AddNewOrder,content);

                Assert.True(response.StatusCode == System.Net.HttpStatusCode.BadRequest);
            }
        }

        string BuildOrder()
        {
            var order = new NewOrderViewModel()
            {
                CardExpiration = DateTime.UtcNow.AddYears(1),
                CardNumber = "5145-555-5555",
                CardHolderName = "Jhon Senna",
                CardSecurityNumber = "232",
                CardType = "Amex",
                ShippingCity = "Redmon",
                ShippingCountry = "USA",
                ShippingState = "WA",
                ShippingStreet = "One way"
            };

            return JsonConvert.SerializeObject(order);
        }
        string BuildOrderWithInvalidExperationTime()
        {
            var order = new NewOrderViewModel()
            {
                CardExpiration = DateTime.UtcNow.AddYears(-1),
                CardNumber = "5145-555-5555",
                CardHolderName = "Jhon Senna",
                CardSecurityNumber = "232",
                CardType = "Amex",
                ShippingCity = "Redmon",
                ShippingCountry = "USA",
                ShippingState = "WA",
                ShippingStreet = "One way"
            };

            return JsonConvert.SerializeObject(order);
        }
    }
}
