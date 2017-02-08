using Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;
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
        private readonly Mock<IBuyerRepository> _buyerRepositoryMock;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;

        public NewOrderRequestHandlerTest()
        {
            
            _buyerRepositoryMock = new Mock<IBuyerRepository>();
            _orderRepositoryMock = new Mock<IOrderRepository>();
        }

        [Fact]
        public async Task Handle_returns_true_when_order_is_persisted_succesfully()
        {
            // Arrange
            _buyerRepositoryMock.Setup(buyerRepo => buyerRepo.FindAsync(FakeOrderRequestWithBuyer().BuyerIdentityGuid))
               .Returns(Task.FromResult<Buyer>(FakeBuyer()));

            _buyerRepositoryMock.Setup(buyerRepo => buyerRepo.UnitOfWork.SaveChangesAsync(default(CancellationToken)))
                .Returns(Task.FromResult(1));

            _orderRepositoryMock.Setup(or => or.Add(FakeOrder()))
                .Returns(FakeOrder());

            _orderRepositoryMock.Setup(or => or.UnitOfWork.SaveChangesAsync(default(CancellationToken)))
                .Returns(Task.FromResult(1));

            //Act
            var handler = new CreateOrderCommandHandler(_buyerRepositoryMock.Object, _orderRepositoryMock.Object);
            var result = await handler.Handle(FakeOrderRequestWithBuyer());
            
            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Handle_return_false_if_order_is_not_persisted()
        {
            _buyerRepositoryMock.Setup(buyerRepo => buyerRepo.FindAsync(FakeOrderRequestWithBuyer().BuyerIdentityGuid))
                .Returns(Task.FromResult<Buyer>(FakeBuyer()));

            _buyerRepositoryMock.Setup(buyerRepo => buyerRepo.UnitOfWork.SaveChangesAsync(default(CancellationToken)))
                .Returns(Task.FromResult(1));

            _orderRepositoryMock.Setup(or => or.Add(FakeOrder())).Returns(FakeOrder());
            _orderRepositoryMock.Setup(or => or.UnitOfWork.SaveChangesAsync(default(CancellationToken)))
                .Returns(Task.FromResult(0));

            //Act
            var handler = new CreateOrderCommandHandler(_buyerRepositoryMock.Object, _orderRepositoryMock.Object);
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
            return new CreateOrderCommand
            {
                BuyerIdentityGuid = "1234",
                CardNumber = "1234",
                CardExpiration = DateTime.Now.AddYears(1), 
                CardSecurityNumber = "123", 
                CardHolderName = "XXX"
            };
        }
    }
}
