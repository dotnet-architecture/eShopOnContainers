using FunctionalTests.Extensions;
using FunctionalTests.Services.Basket;
using Microsoft.eShopOnContainers.Services.Basket.API.Model;
using Microsoft.eShopOnContainers.WebMVC.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebMVC.Models;
using Xunit;

namespace FunctionalTests.Services.Ordering
{
    public class OrderingScenarios : OrderingScenariosBase
    {        
        [Fact]
        public async Task Cancel_basket_and_check_order_status_cancelled()
        {
            using (var orderServer = new OrderingScenariosBase().CreateServer())
            using (var basketServer = new BasketScenariosBase().CreateServer())
            {
                // Expected data
                var cityExpected = $"city-{Guid.NewGuid()}";
                var orderStatusExpected = "cancelled";

                var basketClient = basketServer.CreateIdempotentClient();
                var orderClient = orderServer.CreateIdempotentClient();

                // GIVEN a basket is created 
                var contentBasket = new StringContent(BuildBasket(), UTF8Encoding.UTF8, "application/json");
                await basketClient.PostAsync(BasketScenariosBase.Post.CreateBasket, contentBasket);

                // AND basket checkout is sent
                await basketClient.PostAsync(BasketScenariosBase.Post.Checkout, new StringContent(BuildCheckout(cityExpected), UTF8Encoding.UTF8, "application/json"));

                // WHEN Order is created in Ordering.api
                var newOrder = await TryGetNewOrderCreated(cityExpected, orderClient);

                // AND Order is cancelled in Ordering.api
                await orderClient.PutAsync(OrderingScenariosBase.Put.CancelOrder, new StringContent(BuildCancelOrder(newOrder.OrderNumber), UTF8Encoding.UTF8, "application/json"));

                // AND the requested order is retrieved
                var order = await TryGetNewOrderCreated(cityExpected, orderClient);

                // THEN check status
                Assert.Equal(orderStatusExpected, order.Status);
            }
        }

        private async Task<Order> TryGetNewOrderCreated(string city, HttpClient orderClient)
        {
            var counter = 0;
            Order order = null;

            while (counter < 20)
            {
                //get the orders and verify that the new order has been created
                var ordersGetResponse = await orderClient.GetStringAsync(OrderingScenariosBase.Get.Orders);
                var orders = JsonConvert.DeserializeObject<List<Order>>(ordersGetResponse);

                if (orders == null || orders.Count == 0) {
                    counter++;
                    await Task.Delay(100);
                    continue;
                }

                var lastOrder = orders.OrderByDescending(o => o.Date).First();
                int.TryParse(lastOrder.OrderNumber, out int id);
                var orderDetails = await orderClient.GetStringAsync(OrderingScenariosBase.Get.OrderBy(id));
                order = JsonConvert.DeserializeObject<Order>(orderDetails);

                if (IsOrderCreated(order, city))
                {
                    break;
                }                
            }                
            
            return order;
        }

        private bool IsOrderCreated(Order order, string city)
        {
            return order.City == city;
        }

        string BuildBasket()
        {
            var order = new CustomerBasket("9e3163b9-1ae6-4652-9dc6-7898ab7b7a00");
            order.Items = new List<Microsoft.eShopOnContainers.Services.Basket.API.Model.BasketItem>()
            {
                new Microsoft.eShopOnContainers.Services.Basket.API.Model.BasketItem()
                {
                    Id = "1",
                    ProductName = "ProductName",
                    ProductId = "1",
                    UnitPrice = 10,
                    Quantity = 1
                }
            };
            return JsonConvert.SerializeObject(order);
        }

        string BuildCancelOrder(string orderId)
        {
            var order = new OrderDTO()
            {
                OrderNumber = orderId
            };           
            return JsonConvert.SerializeObject(order);
        }

        string BuildCheckout(string cityExpected)
        {
            var checkoutBasket = new BasketDTO()
            {
                City = cityExpected,
                Street = "street",
                State = "state",
                Country = "coutry",
                ZipCode = "zipcode",
                CardNumber = "1111111111111",
                CardHolderName = "CardHolderName",
                CardExpiration = DateTime.Now.AddYears(1),
                CardSecurityNumber = "123",
                CardTypeId = 1,
                Buyer = "Buyer",                
                RequestId = Guid.NewGuid()
            };

            return JsonConvert.SerializeObject(checkoutBasket);
        }
    }
}
 