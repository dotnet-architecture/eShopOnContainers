using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.WebMVC.Controllers;
using Microsoft.eShopOnContainers.WebMVC.Services;
using Microsoft.eShopOnContainers.WebMVC.ViewModels;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using BasketModel = Microsoft.eShopOnContainers.WebMVC.ViewModels.Basket;

namespace UnitTest.Ordering.Application
{
    public class OrderControllerTest
    {
        private readonly Mock<IOrderingService> _orderServiceMock;
        private readonly Mock<IBasketService> _basketServiceMock;
        private readonly Mock<IIdentityParser<ApplicationUser>> _identityParserMock;
        private readonly Mock<HttpContext> _contextMock;

        public OrderControllerTest()
        {
            _orderServiceMock = new Mock<IOrderingService>();
            _basketServiceMock = new Mock<IBasketService>();
            _identityParserMock = new Mock<IIdentityParser<ApplicationUser>>();
            _contextMock = new Mock<HttpContext>();
        }

        [Fact]
        public async Task Get_order_list_success()
        {
            //Arrange
            var expectedTotalResults = 1;
            var fakeOrder = GetFakeOrder();

            _orderServiceMock.Setup(x => x.GetMyOrders(It.IsAny<ApplicationUser>()))
                .Returns(Task.FromResult(new List<Order>() { fakeOrder }));

            //Act
            var orderController = new OrderController(_orderServiceMock.Object, _basketServiceMock.Object, _identityParserMock.Object);
            orderController.ControllerContext.HttpContext = _contextMock.Object;
            var actionResult = await orderController.Index(fakeOrder);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(actionResult);
            var model = Assert.IsAssignableFrom<List<Order>>(viewResult.ViewData.Model);
            Assert.Equal(model.Count, expectedTotalResults);
        }

        [Fact]
        public async Task Get_order_detail_success()
        {
            //Arrange
            var fakeOrderId = "12";
            var fakeOrder = GetFakeOrder();

            _orderServiceMock.Setup(x => x.GetOrder(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .Returns(Task.FromResult(fakeOrder));

            //Act
            var orderController = new OrderController(_orderServiceMock.Object, _basketServiceMock.Object, _identityParserMock.Object);
            orderController.ControllerContext.HttpContext = _contextMock.Object;
            var actionResult = await orderController.Detail(fakeOrderId);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(actionResult);
            Assert.IsAssignableFrom<Order>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Get_create_order_success()
        {
            //Arrange
            var fakeBuyerId = "1";
            var fakeBasket = GetFakeBasket(fakeBuyerId);
            var fakeOrder = GetFakeOrder();

            _basketServiceMock.Setup(x => x.GetBasket(It.IsAny<ApplicationUser>()))
                .Returns(Task.FromResult(fakeBasket));

            _basketServiceMock.Setup(x => x.MapBasketToOrder(It.IsAny<BasketModel>()))
                .Returns(fakeOrder);

            _orderServiceMock.Setup(x => x.MapUserInfoIntoOrder(It.IsAny<ApplicationUser>(), It.IsAny<Order>()))
                .Returns(fakeOrder);

            //Act
            var orderController = new OrderController(_orderServiceMock.Object, _basketServiceMock.Object, _identityParserMock.Object);
            orderController.ControllerContext.HttpContext = _contextMock.Object;
            var actionResult = await orderController.Create();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(actionResult);
            Assert.IsAssignableFrom<Order>(viewResult.ViewData.Model);
        }        

        private BasketModel GetFakeBasket(string buyerId)
        {
            return new BasketModel()
            {
                BuyerId = buyerId
            };
        }

        private Order GetFakeOrder()
        {
            return new Order()
            {
                OrderNumber = "1",
                CardNumber = "12",
                CardSecurityNumber = "1212",
                Status = "Pending",
                RequestId = Guid.NewGuid(),
                CardExpiration = DateTime.Now.AddYears(1),
            };
        }
    }
}
