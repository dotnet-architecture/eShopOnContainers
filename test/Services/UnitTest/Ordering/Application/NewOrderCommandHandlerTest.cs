using Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;
using Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure.Services;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.BuyerAggregate;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.Ordering.Application
{
    public class NewOrderRequestHandlerTest
    {
        private readonly Mock<IBuyerRepository<Buyer>> _buyerRepositoryMock;
        private readonly Mock<IOrderRepository<Order>> _orderRepositoryMock;
        private readonly Mock<IIdentityService> _identityServiceMock;

        public NewOrderRequestHandlerTest()
        {
            
            _buyerRepositoryMock = new Mock<IBuyerRepository<Buyer>>();
            _orderRepositoryMock = new Mock<IOrderRepository<Order>>();
            _identityServiceMock = new Mock<IIdentityService>();
        }

        [Fact]
        public async Task Handle_returns_true_when_order_is_persisted_succesfully()
        {

            var buyerId = "1234";
            // Arrange
            _buyerRepositoryMock.Setup(buyerRepo => buyerRepo.FindAsync(buyerId))
               .Returns(Task.FromResult<Buyer>(FakeBuyer()));

            _buyerRepositoryMock.Setup(buyerRepo => buyerRepo.UnitOfWork.SaveChangesAsync(default(CancellationToken)))
                .Returns(Task.FromResult(1));

            _orderRepositoryMock.Setup(or => or.Add(FakeOrder()))
                .Returns(FakeOrder());

            _orderRepositoryMock.Setup(or => or.UnitOfWork.SaveChangesAsync(default(CancellationToken)))
                .Returns(Task.FromResult(1));

            _identityServiceMock.Setup(svc => svc.GetUserIdentity()).Returns(buyerId);
            //Act
            var handler = new CreateOrderCommandHandler(_buyerRepositoryMock.Object, _orderRepositoryMock.Object, _identityServiceMock.Object);
            var result = await handler.Handle(FakeOrderRequestWithBuyer());
            
            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Handle_return_false_if_order_is_not_persisted()
        {
            var buyerId = "1234";
            _buyerRepositoryMock.Setup(buyerRepo => buyerRepo.FindAsync(buyerId))
                .Returns(Task.FromResult<Buyer>(FakeBuyer()));

            _buyerRepositoryMock.Setup(buyerRepo => buyerRepo.UnitOfWork.SaveChangesAsync(default(CancellationToken)))
                .Returns(Task.FromResult(1));

            _orderRepositoryMock.Setup(or => or.Add(FakeOrder())).Returns(FakeOrder());
            _orderRepositoryMock.Setup(or => or.UnitOfWork.SaveChangesAsync(default(CancellationToken)))
                .Returns(Task.FromResult(0));
            _identityServiceMock.Setup(svc => svc.GetUserIdentity()).Returns(buyerId);

            //Act
            var handler = new CreateOrderCommandHandler(_buyerRepositoryMock.Object, _orderRepositoryMock.Object, _identityServiceMock.Object);
            var result = await handler.Handle(FakeOrderRequestWithBuyer());

            //Assert
            Assert.False(result);
        }

        private Buyer FakeBuyer()
        {
            return new Buyer(Guid.NewGuid().ToString());
        }

        private Order FakeOrder()
        {
            return new Order(1, 1, new Address("street", "city", "state", "country", "zipcode"));
        }

        private CreateOrderCommand FakeOrderRequestWithBuyer()
        {
            return new CreateOrderCommand(
                city: null,
                street: null,
                state: null,
                country: null,
                zipcode: null,
                cardNumber: "1234",
                cardExpiration: DateTime.Now.AddYears(1),
                cardSecurityNumber: "123",
                cardHolderName: "XXX",
                cardTypeId: 0);
        }
    }
}
