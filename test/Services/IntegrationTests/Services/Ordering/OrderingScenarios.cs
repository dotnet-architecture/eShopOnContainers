namespace IntegrationTests.Services.Ordering
{
    using IntegrationTests.Services.Extensions;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;
    using Newtonsoft.Json;
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;
    using System.Collections;
    using static Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands.CreateOrderCommand;
    using System.Collections.Generic;

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
                var response = await server.CreateIdempotentClient()
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

                var response = await server.CreateIdempotentClient()
                    .PostAsync(Post.AddNewOrder, content);

                Assert.True(response.StatusCode == System.Net.HttpStatusCode.BadRequest);
            }
        }

        //public CreateOrderCommand(string city, string street, string state, string country, string zipcode,
        //   string cardNumber, string cardHolderName, DateTime cardExpiration,
        //   string cardSecurityNumber, int cardTypeId, int paymentId, int buyerId) : this()

        string BuildOrder()
        {
            List<OrderItemDTO> orderItemsList = new List<OrderItemDTO>();
            orderItemsList.Add(new OrderItemDTO()
                                                {
                                                    ProductId = 1,
                                                    Discount = 10M,
                                                    UnitPrice = 10,
                                                    Units = 1,
                                                    ProductName = "Some name"
                                                }
                               );

            var order = new CreateOrderCommand(
                orderItemsList,
                cardExpiration: DateTime.UtcNow.AddYears(1),
                cardNumber: "5145-555-5555",
                cardHolderName: "Jhon Senna",
                cardSecurityNumber: "232",
                cardTypeId: 1,
                city: "Redmon",
                country: "USA",
                state: "WA",
                street: "One way",
                zipcode: "zipcode",
                paymentId: 1,
                buyerId: 1               
            );

            return JsonConvert.SerializeObject(order);
        }
        string BuildOrderWithInvalidExperationTime()
        {
            var order = new CreateOrderCommand(
                new List<OrderItemDTO>(),
                cardExpiration: DateTime.UtcNow.AddYears(-1),
                cardNumber: "5145-555-5555",
                cardHolderName: "Jhon Senna",
                cardSecurityNumber: "232",
                cardTypeId: 1,
                city: "Redmon",
                country: "USA",
                state: "WA",
                street: "One way",
                zipcode: "zipcode",
                buyerId: 1,
                paymentId:1
            );

            return JsonConvert.SerializeObject(order);
        }
    }        
}
