using MediatR;
using Microsoft.eShopOnContainers.Services.Ordering.Api.Application.Commands;
using Microsoft.eShopOnContainers.Services.Ordering.Domain;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.Repositories;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
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
            //Mocks;
            _buyerRepositoryMock = new Mock<IBuyerRepository>();
            _orderRepositoryMock = new Mock<IOrderRepository>();
        }

        [Fact]
        public async Task Handle_ReturnsTrue_WhenOrderIsPersistedSuccesfully()
        {
            // Arrange
            _buyerRepositoryMock.Setup(buyerRepo => buyerRepo.FindAsync(FakeOrderRequestWithBuyer().Buyer))
               .Returns(Task.FromResult<Buyer>(FakeBuyer()));
            _buyerRepositoryMock.Setup(buyerRepo => buyerRepo.UnitOfWork.SaveChangesAsync(default(CancellationToken)))
                .Returns(Task.FromResult(1));

            _orderRepositoryMock.Setup(or => or.Add(FakeOrder())).Returns(FakeOrder());
            _orderRepositoryMock.Setup(or => or.UnitOfWork.SaveChangesAsync(default(CancellationToken))).Returns(Task.FromResult(1));

            //Act
            var handler = new NewOrderRequestHandler(_buyerRepositoryMock.Object, _orderRepositoryMock.Object);
            var result = await handler.Handle(FakeOrderRequestWithBuyer());
            
            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Handle_ReturnsFalse_WhenOrderIsNotPersisted()
        {
            _buyerRepositoryMock.Setup(buyerRepo => buyerRepo.FindAsync(FakeOrderRequestWithBuyer().Buyer))
                .Returns(Task.FromResult<Buyer>(FakeBuyer()));
            _buyerRepositoryMock.Setup(buyerRepo => buyerRepo.UnitOfWork.SaveChangesAsync(default(CancellationToken)))
                .Returns(Task.FromResult(1));

            _orderRepositoryMock.Setup(or => or.Add(FakeOrder())).Returns(FakeOrder());
            _orderRepositoryMock.Setup(or => or.UnitOfWork.SaveChangesAsync(default(CancellationToken))).Returns(Task.FromResult(0));

            //Act
            var handler = new NewOrderRequestHandler(_buyerRepositoryMock.Object, _orderRepositoryMock.Object);
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
            return new Order(1, 1)
            {

            };
        }

        private NewOrderRequest FakeOrderRequestWithBuyer()
        {
            return new NewOrderRequest
            {
                Buyer = "1234",
                CardNumber = "1234",
                CardExpiration = DateTime.Now.AddYears(1), 
                CardSecurityNumber = "123", 
                CardHolderName = "XXX"
            };
        }
    }
}
