using Basket.API.IntegrationEvents.Events;
using Basket.API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.Services.Basket.API.Controllers;
using Microsoft.eShopOnContainers.Services.Basket.API.Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using IBasketIdentityService = Microsoft.eShopOnContainers.Services.Basket.API.Services.IIdentityService;

namespace UnitTest.Basket.Application
{
    public class BasketWebApiTest
    {
        private readonly Mock<IBasketRepository> _basketRepositoryMock;
        private readonly Mock<IBasketIdentityService> _identityServiceMock;
        private readonly Mock<IEventBus> _serviceBusMock;

        public BasketWebApiTest()
        {
            _basketRepositoryMock = new Mock<IBasketRepository>();
            _identityServiceMock = new Mock<IBasketIdentityService>();
            _serviceBusMock = new Mock<IEventBus>();
        }

        [Fact]
        public async Task Get_customer_basket_success()
        {
            //Arrange
            var fakeCustomerId = "1";
            var fakeCustomerBasket = GetCustomerBasketFake(fakeCustomerId);

            _basketRepositoryMock.Setup(x => x.GetBasketAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(fakeCustomerBasket));
            _identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(fakeCustomerId);

            _serviceBusMock.Setup(x => x.Publish(It.IsAny<UserCheckoutAcceptedIntegrationEvent>()));
            //Act
            var basketController = new BasketController(
                _basketRepositoryMock.Object, _identityServiceMock.Object, _serviceBusMock.Object);
            var actionResult = await basketController.Get(fakeCustomerId) as OkObjectResult;

            //Assert
            Assert.Equal(actionResult.StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.Equal(((CustomerBasket)actionResult.Value).BuyerId, fakeCustomerId);
        }

        [Fact]
        public async Task Post_customer_basket_success()
        {
            //Arrange
            var fakeCustomerId = "1";
            var fakeCustomerBasket = GetCustomerBasketFake(fakeCustomerId);

            _basketRepositoryMock.Setup(x => x.UpdateBasketAsync(It.IsAny<CustomerBasket>()))
                .Returns(Task.FromResult(fakeCustomerBasket));
            _identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(fakeCustomerId);
            _serviceBusMock.Setup(x => x.Publish(It.IsAny<UserCheckoutAcceptedIntegrationEvent>()));
            //Act
            var basketController = new BasketController(
                _basketRepositoryMock.Object, _identityServiceMock.Object, _serviceBusMock.Object);

            var actionResult = await basketController.Post(fakeCustomerBasket) as OkObjectResult;

            //Assert
            Assert.Equal(actionResult.StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.Equal(((CustomerBasket)actionResult.Value).BuyerId, fakeCustomerId);
        }        

        [Fact]
        public async Task Doing_Checkout_Without_Basket_Should_Return_Bad_Request()
        {
            var fakeCustomerId = "2";
            _basketRepositoryMock.Setup(x => x.GetBasketAsync(It.IsAny<string>()))
                .Returns(Task.FromResult((CustomerBasket)null));
            _identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(fakeCustomerId);
            //Act
            var basketController = new BasketController(
                _basketRepositoryMock.Object, _identityServiceMock.Object, _serviceBusMock.Object);

            var result = await basketController.Checkout(new BasketCheckout(), Guid.NewGuid().ToString()) as BadRequestResult;
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Doing_Checkout_Wit_Basket_Should_Publish_UserCheckoutAccepted_Integration_Event()
        {
            var fakeCustomerId = "1";
            var fakeCustomerBasket = GetCustomerBasketFake(fakeCustomerId);
            _basketRepositoryMock.Setup(x => x.GetBasketAsync(It.IsAny<string>()))
                 .Returns(Task.FromResult(fakeCustomerBasket));
            _identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(fakeCustomerId);
            //Act
            var basketController = new BasketController(
                _basketRepositoryMock.Object, _identityServiceMock.Object, _serviceBusMock.Object);

            var result = await basketController.Checkout(new BasketCheckout(), Guid.NewGuid().ToString()) as AcceptedResult;
            _serviceBusMock.Verify(mock => mock.Publish(It.IsAny<UserCheckoutAcceptedIntegrationEvent>()), Times.Once);
            Assert.NotNull(result);
        }

        private CustomerBasket GetCustomerBasketFake(string fakeCustomerId)
        {            
            return new CustomerBasket(fakeCustomerId)
            {
                Items = new List<BasketItem>()
                {
                    new BasketItem()
                }
            };
        }
    }
}
