using Microsoft.AspNetCore.TestHost;
using Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;
using Microsoft.eShopOnContainers.WebMVC.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands.CreateOrderCommand;

namespace FunctionalTests.Services.Ordering
{
    public class OrderingScenarios : OrderingScenariosBase
    {
        [Fact]
        public async Task Create_order_and_return_the_order_by_id()
        {
            using (var server = CreateServer())
            {
                var client = server.CreateClient();

                //Arrange               
                await client.PostAsync(Post.AddNewOrder, new StringContent(BuildOrder(), UTF8Encoding.UTF8, "application/json"));

                var ordersResponse = await client.GetAsync(Get.Orders);
                var responseBody = await ordersResponse.Content.ReadAsStringAsync();
                dynamic orders = JsonConvert.DeserializeObject(responseBody);                
                string orderId = orders[0].ordernumber;

                //Act
                var order= await client.GetAsync(Get.OrderBy(int.Parse(orderId)));
                var orderBody = await order.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Order>(orderBody);

                //Assert
                Assert.Equal(orderId, result.OrderNumber);
                Assert.Equal("inprocess", result.Status);
                Assert.Equal(1, result.OrderItems.Count);
                Assert.Equal(10, result.OrderItems[0].UnitPrice);
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
    }
}
