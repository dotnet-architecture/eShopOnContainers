using Basket.FunctionalTests.Base;
using Microsoft.eShopOnContainers.Services.Basket.API.Infrastructure.Repositories;
using Microsoft.eShopOnContainers.Services.Basket.API.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Basket.FunctionalTests
{
    public class RedisBasketRepositoryTests
        : BasketScenarioBase
    {

        [Fact]
        public async Task UpdateBasket_return_and_add_basket()
        {
            using (var server = CreateServer())
            {
                var redis = server.Host.Services.GetRequiredService<ConnectionMultiplexer>();

                var redisBasketRepository = BuildBasketRepository(redis);

                var basket = await redisBasketRepository.UpdateBasketAsync(new CustomerBasket("customerId")
                {
                    BuyerId = "buyerId",
                    Items = BuildBasketItems()
                });

                Assert.NotNull(basket);
                Assert.Single(basket.Items);
            }

            
        }

        [Fact]
        public async Task Delete_Basket_return_null()
        {

            using (var server = CreateServer())
            {
                var redis = server.Host.Services.GetRequiredService<ConnectionMultiplexer>();

                var redisBasketRepository = BuildBasketRepository(redis);

                var basket = await redisBasketRepository.UpdateBasketAsync(new CustomerBasket("customerId")
                {
                    BuyerId = "buyerId",
                    Items = BuildBasketItems()
                });

                var deleteResult = await redisBasketRepository.DeleteBasketAsync("buyerId");

                var result = await redisBasketRepository.GetBasketAsync(basket.BuyerId);

                Assert.True(deleteResult);
                Assert.Null(result);
            }            
        }

        RedisBasketRepository BuildBasketRepository(ConnectionMultiplexer connMux)
        {
            var loggerFactory = new LoggerFactory();
            return new RedisBasketRepository(loggerFactory, connMux);
        }

        List<BasketItem> BuildBasketItems()
        {
            return new List<BasketItem>()
            {
                new BasketItem()
                {
                    Id = "basketId",
                    PictureUrl = "pictureurl",
                    ProductId = 1,
                    ProductName = "productName",
                    Quantity = 1,
                    UnitPrice = 1
                }
            };
        }
    }
}
