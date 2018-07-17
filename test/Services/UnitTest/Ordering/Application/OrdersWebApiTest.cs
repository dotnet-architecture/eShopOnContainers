using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;
using Microsoft.eShopOnContainers.Services.Ordering.API.Application.Queries;
using Microsoft.eShopOnContainers.Services.Ordering.API.Controllers;
using Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure.Services;
using Moq;
using Ordering.API.Application.Commands;
using System;
using System.Linq;
using System.Threading;
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
            _mediatorMock.Setup(x => x.Send(It.IsAny<IdentifiedCommand<CancelOrderCommand, bool>>(), default(CancellationToken)))
                .Returns(Task.FromResult(true));

            //Act
            var orderController = new OrdersController(_mediatorMock.Object, _orderQueriesMock.Object, _identityServiceMock.Object);
            var actionResult = await orderController.CancelOrder(new CancelOrderCommand(1), Guid.NewGuid().ToString()) as OkResult;

            //Assert
            Assert.Equal(actionResult.StatusCode, (int)System.Net.HttpStatusCode.OK);

        }

        [Fact]
        public async Task Cancel_order_bad_request()
        {
            //Arrange
            _mediatorMock.Setup(x => x.Send(It.IsAny<IdentifiedCommand<CancelOrderCommand, bool>>(), default(CancellationToken)))
                .Returns(Task.FromResult(true));

            //Act
            var orderController = new OrdersController(_mediatorMock.Object, _orderQueriesMock.Object, _identityServiceMock.Object);
            var actionResult = await orderController.CancelOrder(new CancelOrderCommand(1), String.Empty) as BadRequestResult;

            //Assert
            Assert.Equal(actionResult.StatusCode, (int)System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Ship_order_with_requestId_success()
        {
            //Arrange
            _mediatorMock.Setup(x => x.Send(It.IsAny<IdentifiedCommand<ShipOrderCommand, bool>>(), default(System.Threading.CancellationToken)))
                .Returns(Task.FromResult(true));

            //Act
            var orderController = new OrdersController(_mediatorMock.Object, _orderQueriesMock.Object, _identityServiceMock.Object);
            var actionResult = await orderController.ShipOrder(new ShipOrderCommand(1), Guid.NewGuid().ToString()) as OkResult;

            //Assert
            Assert.Equal(actionResult.StatusCode, (int)System.Net.HttpStatusCode.OK);

        }

        [Fact]
        public async Task Ship_order_bad_request()
        {
            //Arrange
            _mediatorMock.Setup(x => x.Send(It.IsAny<IdentifiedCommand<CreateOrderCommand, bool>>(), default(System.Threading.CancellationToken)))
                .Returns(Task.FromResult(true));

            //Act
            var orderController = new OrdersController(_mediatorMock.Object, _orderQueriesMock.Object, _identityServiceMock.Object);
            var actionResult = await orderController.ShipOrder(new ShipOrderCommand(1), String.Empty) as BadRequestResult;

            //Assert
            Assert.Equal(actionResult.StatusCode, (int)System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Get_orders_success()
        {
            //Arrange
            var fakeDynamicResult = Enumerable.Empty<OrderSummary>();
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
            var fakeDynamicResult = new Order();
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
            var fakeDynamicResult = Enumerable.Empty<CardType>();
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
