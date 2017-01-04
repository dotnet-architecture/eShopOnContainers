
//namespace FunctionalTests.Services.Basket
//{
//    using Microsoft.eShopOnContainers.Services.Basket.API;
//    using Microsoft.eShopOnContainers.Services.Basket.API.Model;
//    using Microsoft.Extensions.Logging;
//    using Microsoft.Extensions.Options;
//    using System.Collections.Generic;
//    using System.Threading.Tasks;
//    using Xunit;


//    public class RedisBasketRepositoryTests
//    {
//        [Fact]
//        public async Task UpdateBasket_return_and_add_basket()
//        {
//            var redisBasketRepository = BuildBasketRepository();

//            var basket = await redisBasketRepository.UpdateBasket(new CustomerBasket("customerId")
//            {
//                BuyerId = "buyerId",
//                Items = BuildBasketItems()
//            });

//            Assert.NotNull(basket);
//            Assert.Equal(1, basket.Items.Count);
//        }

//        [Fact]
//        public async Task GetBasket_return_existing_basket()
//        {
//        }

//        RedisBasketRepository BuildBasketRepository()
//        {
//            var loggerFactory = new LoggerFactory();

//            var options = Options.Create<BasketSettings>(new BasketSettings()
//            {
//                ConnectionString = "127.0.0.1"
//            });

//            return new RedisBasketRepository(options, loggerFactory);
//        }

//        List<BasketItem> BuildBasketItems()
//        {
//            return new List<BasketItem>()
//            {
//                new BasketItem()
//                {
//                    Id = "basketId",
//                    PictureUrl = "pictureurl",
//                    ProductId = "productId",
//                    ProductName = "productName",
//                    Quantity = 1,
//                    UnitPrice = 1
//                }
//            };
//        }
//    }
//}
