using System;
using Xunit;
using System.Threading.Tasks;
using Moq;
using MediatR;
using Microsoft.eShopOnContainers.Services.Ordering.API.Controllers;
using Microsoft.eShopOnContainers.Services.Ordering.Api.Application.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure.Services;
using Microsoft.eShopOnContainers.Services.Ordering.Api.Application.Queries;
using System.Collections.Generic;

namespace UnitTest.Ordering.Controllers
{
    public class OrderControllerTest
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IIdentityService> _identityMock;
        private readonly Mock<IOrderQueries> _queriesMock;

        public OrderControllerTest()
        {
            //Mocks;
            _mediatorMock = new Mock<IMediator>();
            _identityMock = new Mock<IIdentityService>();
            _queriesMock = new Mock<IOrderQueries>();
        }

        [Fact]
        public async Task AddOrder_ReturnsBadRequestResult_WhenPersitenceOperationFails()
        {
            // Arrange
            var orderRequest = new object() as IAsyncRequest<bool>;

            _mediatorMock.Setup(mediator => mediator.SendAsync(OrderFakeNotExpired()))
                .Returns(Task.FromResult(false));

            _identityMock.Setup(identity => identity.GetUserIdentity())
                .Returns(Guid.NewGuid().ToString());

            var controller = new OrdersController(_mediatorMock.Object, _queriesMock.Object, _identityMock.Object);

            // Act
            var badRequestResult = await controller.AddOrder(OrderFakeNotExpired());

            // Assert
            Assert.IsType<BadRequestResult>(badRequestResult);
        }

        [Fact]
        public async Task AddOrder_ReturnsOK_WhenPersistenceOperationSucceed()
        {
            // Arrange
            _mediatorMock.Setup(mediator => mediator.SendAsync(OrderFakeNotExpired()))
                .Returns(Task.FromResult(true));

            _identityMock.Setup(identity => identity.GetUserIdentity())
                .Returns(Guid.NewGuid().ToString());

            var controller = new OrdersController(_mediatorMock.Object, _queriesMock.Object, _identityMock.Object);

            // Act
            var badRequestResult = await controller.AddOrder(OrderFakeNotExpired());

            // Assert
            Assert.IsType<BadRequestResult>(badRequestResult);
        }

        [Fact]
        public async Task GetOrder_ReturnsNotFound_WhenItemNotFound()
        {            
            // Arrange
            _queriesMock.Setup(queries => queries.GetOrder(1))
                .Throws(new KeyNotFoundException());
                
            var controller = new OrdersController(_mediatorMock.Object, _queriesMock.Object, _identityMock.Object);

            // Act
            var notFoundResult = await controller.GetOrder(1);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
        }

        [Fact]
        public async Task GetOrder_ReturnsOkObjecResult_WheItemFound()
        {
            // Arrange
            _queriesMock.Setup(queries => queries.GetOrder(1))
                .Returns(Task.FromResult(new object()));

            var controller = new OrdersController(_mediatorMock.Object, _queriesMock.Object, _identityMock.Object);

            // Act
            var OkObjectResult = await controller.GetOrder(1);

            // Assert
            Assert.IsType<OkObjectResult>(OkObjectResult);
        }

        [Fact]
        public async Task GetOrders_ReturnsOkObjectResult()
        {
            // Arrange
            _queriesMock.Setup(queries => queries.GetOrders())
                .Returns(Task.FromResult(new object()));

            var controller = new OrdersController(_mediatorMock.Object, _queriesMock.Object, _identityMock.Object);

            // Act
            var OkObjectResult = await controller.GetOrders();

            // Assert
            Assert.IsType<OkObjectResult>(OkObjectResult);
        }

        [Fact]
        public async Task GetCardTypes()
        {
            // Arrange
            _queriesMock.Setup(queries => queries.GetCardTypes())
                .Returns(Task.FromResult(new object()));

            var controller = new OrdersController(_mediatorMock.Object, _queriesMock.Object, _identityMock.Object);

            // Act
            var OkObjectResult = await controller.GetCardTypes();

            // Assert
            Assert.IsType<OkObjectResult>(OkObjectResult);
        }

        //Fakes
        private NewOrderRequest OrderFakeNotExpired()
        {
            return new NewOrderRequest()
            {
                CardTypeId = 1,
                CardExpiration = DateTime.Now.AddYears(1)
            };
        }

        private NewOrderRequest OrderFakeExpired()
        {
            return new NewOrderRequest()
            {
                CardTypeId = 1,
                CardExpiration = DateTime.Now.AddYears(-1)
            };
        }
    }
}
