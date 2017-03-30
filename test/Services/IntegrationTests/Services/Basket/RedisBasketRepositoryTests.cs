
namespace IntegrationTests.Services.Basket
{
    using Microsoft.eShopOnContainers.Services.Basket.API;
    using Microsoft.eShopOnContainers.Services.Basket.API.Model;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;
    using Moq;


    public class RedisBasketRepositoryTests
    {
        private Mock<IOptionsSnapshot<BasketSettings>> _optionsMock;

        public RedisBasketRepositoryTests()
        {
            _optionsMock = new Mock<IOptionsSnapshot<BasketSettings>>();
        }

        [Fact]
        public async Task UpdateBasket_return_and_add_basket()
        {
            var redisBasketRepository = BuildBasketRepository();

            var basket = await redisBasketRepository.UpdateBasketAsync(new CustomerBasket("customerId")
            {
                BuyerId = "buyerId",
                Items = BuildBasketItems()
            });

            Assert.NotNull(basket);
            Assert.Equal(1, basket.Items.Count);
        }

        [Fact]
        public async Task Delete_Basket_return_null()
        {
            var redisBasketRepository = BuildBasketRepository();

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

        RedisBasketRepository BuildBasketRepository()
        {
            var loggerFactory = new LoggerFactory();            
            var basketSettings = new BasketSettings() { ConnectionString = "127.0.0.1" };
            _optionsMock.Setup(x => x.Value).Returns(basketSettings);
            
            return new RedisBasketRepository(_optionsMock.Object, loggerFactory);
        }

        List<BasketItem> BuildBasketItems()
        {
            return new List<BasketItem>()
            {
                new BasketItem()
                {
                    Id = "basketId",
                    PictureUrl = "pictureurl",
                    ProductId = "productId",
                    ProductName = "productName",
                    Quantity = 1,
                    UnitPrice = 1
                }
            };
        }
    }
}
