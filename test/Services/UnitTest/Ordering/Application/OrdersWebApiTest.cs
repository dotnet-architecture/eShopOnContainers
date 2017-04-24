using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;
using Microsoft.eShopOnContainers.Services.Ordering.API.Application.Queries;
using Microsoft.eShopOnContainers.Services.Ordering.API.Controllers;
using Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure.Services;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.Ordering.Application
{
    public class OrdersWebApiTest
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IOrderQueries> _orderQueriesMock;
        private readonly Mock<IIdentityService> _identityServiceMock;

        public OrdersWebApiTest()
        {
            _mediatorMock = new Mock<IMediator>();
            _orderQueriesMock = new Mock<IOrderQueries>();
            _identityServiceMock = new Mock<IIdentityService>();
        }

        [Fact]
        public async Task Create_order_with_requestId_success()
        {
            //Arrange
            _mediatorMock.Setup(x => x.SendAsync(It.IsAny<IdentifiedCommand<CreateOrderCommand, bool>>()))
                .Returns(Task.FromResult(true));

            //Act
            var orderController = new OrdersController(_mediatorMock.Object, _orderQueriesMock.Object, _identityServiceMock.Object);
            var actionResult = await orderController.CreateOrder(new CreateOrderCommand(), Guid.NewGuid().ToString()) as OkResult;

            //Assert
            Assert.Equal(actionResult.StatusCode, (int)System.Net.HttpStatusCode.OK);

        }

        [Fact]
        public async Task Create_order_bad_request()
        {
            //Arrange
            _mediatorMock.Setup(x => x.SendAsync(It.IsAny<IdentifiedCommand<CreateOrderCommand, bool>>()))
                .Returns(Task.FromResult(true));

            //Act
            var orderController = new OrdersController(_mediatorMock.Object, _orderQueriesMock.Object, _identityServiceMock.Object);
            var actionResult = await orderController.CreateOrder(new CreateOrderCommand(), String.Empty) as BadRequestResult;

            //Assert
            Assert.Equal(actionResult.StatusCode, (int)System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Get_orders_success()
        {
            //Arrange
            var fakeDynamicResult = new Object();
            _orderQueriesMock.Setup(x => x.GetOrdersAsync())
                .Returns(Task.FromResult(fakeDynamicResult));

            //Act
            var orderController = new OrdersController(_mediatorMock.Object, _orderQueriesMock.Object, _identityServiceMock.Object);
            var actionResult = await orderController.GetOrders() as OkObjectResult;

            //Assert
            Assert.Equal(actionResult.StatusCode, (int)System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Get_order_success()
        {
            //Arrange
            var fakeOrderId = 123;
            var fakeDynamicResult = new Object();
            _orderQueriesMock.Setup(x => x.GetOrderAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(fakeDynamicResult));

            //Act
            var orderController = new OrdersController(_mediatorMock.Object, _orderQueriesMock.Object, _identityServiceMock.Object);
            var actionResult = await orderController.GetOrder(fakeOrderId) as OkObjectResult;

            //Assert
            Assert.Equal(actionResult.StatusCode, (int)System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Get_cardTypes_success()
        {
            //Arrange
            var fakeDynamicResult = new Object();
            _orderQueriesMock.Setup(x => x.GetCardTypesAsync())
                .Returns(Task.FromResult(fakeDynamicResult));

            //Act
            var orderController = new OrdersController(_mediatorMock.Object, _orderQueriesMock.Object, _identityServiceMock.Object);
            var actionResult = await orderController.GetCardTypes() as OkObjectResult;

            //Assert
            Assert.Equal(actionResult.StatusCode, (int)System.Net.HttpStatusCode.OK);
        }
    }
}
