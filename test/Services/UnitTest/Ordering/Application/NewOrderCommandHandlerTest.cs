using Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;
using Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure.Services;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.BuyerAggregate;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace UnitTest.Ordering.Application
{
    using global::Ordering.API.Application.Models;
    using MediatR;
    using System.Collections;
    using System.Collections.Generic;
    using Xunit;
    using static Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands.CreateOrderCommand;

    public class NewOrderRequestHandlerTest
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IIdentityService> _identityServiceMock;
        private readonly Mock<IMediator> _mediator;

        public NewOrderRequestHandlerTest()
        {

            _orderRepositoryMock = new Mock<IOrderRepository>();
            _identityServiceMock = new Mock<IIdentityService>();
            _mediator = new Mock<IMediator>();
        }

        [Fact]
        public async Task Handle_return_false_if_order_is_not_persisted()
        {
            var buyerId = "1234";

            var fakeOrderCmd = FakeOrderRequestWithBuyer(new Dictionary<string, object>
            { ["cardExpiration"] = DateTime.Now.AddYears(1) });

            _orderRepositoryMock.Setup(orderRepo => orderRepo.GetAsync(It.IsAny<int>()))
               .Returns(Task.FromResult<Order>(FakeOrder()));

            _orderRepositoryMock.Setup(buyerRepo => buyerRepo.UnitOfWork.SaveChangesAsync(default(CancellationToken)))
                .Returns(Task.FromResult(1));

            _identityServiceMock.Setup(svc => svc.GetUserIdentity()).Returns(buyerId);

            //Act
            var handler = new CreateOrderCommandHandler(_mediator.Object, _orderRepositoryMock.Object, _identityServiceMock.Object);
            var cltToken = new System.Threading.CancellationToken();
            var result = await handler.Handle(fakeOrderCmd, cltToken);

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void Handle_throws_exception_when_no_buyerId()
        {
            //Assert
            Assert.Throws<ArgumentNullException>(() => new Buyer(string.Empty, string.Empty));
        }

        private Buyer FakeBuyer()
        {
            return new Buyer(Guid.NewGuid().ToString(), "1");
        }

        private Order FakeOrder()
        {
            return new Order("1", "fakeName", new Address("street", "city", "state", "country", "zipcode"), 1, "12", "111", "fakeName", DateTime.Now.AddYears(1));
        }

        private CreateOrderCommand FakeOrderRequestWithBuyer(Dictionary<string, object> args = null)
        {
            return new CreateOrderCommand(
                new List<BasketItem>(),
                userId: args != null && args.ContainsKey("userId") ? (string)args["userId"] : null,
                userName: args != null && args.ContainsKey("userName") ? (string)args["userName"] : null,
                city: args != null && args.ContainsKey("city") ? (string)args["city"] : null,
                street: args != null && args.ContainsKey("street") ? (string)args["street"] : null,
                state: args != null && args.ContainsKey("state") ? (string)args["state"] : null,
                country: args != null && args.ContainsKey("country") ? (string)args["country"] : null,
                zipcode: args != null && args.ContainsKey("zipcode") ? (string)args["zipcode"] : null,
                cardNumber: args != null && args.ContainsKey("cardNumber") ? (string)args["cardNumber"] : "1234",
                cardExpiration: args != null && args.ContainsKey("cardExpiration") ? (DateTime)args["cardExpiration"] : DateTime.MinValue,
                cardSecurityNumber: args != null && args.ContainsKey("cardSecurityNumber") ? (string)args["cardSecurityNumber"] : "123",
                cardHolderName: args != null && args.ContainsKey("cardHolderName") ? (string)args["cardHolderName"] : "XXX",
                cardTypeId: args != null && args.ContainsKey("cardTypeId") ? (int)args["cardTypeId"] : 0);               
        }
    }
}
