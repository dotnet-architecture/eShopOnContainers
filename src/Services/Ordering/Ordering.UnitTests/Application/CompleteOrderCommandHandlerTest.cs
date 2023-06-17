namespace UnitTest.Ordering.Application;

using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;

public class CompleteOrderCommandHandlerTest
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;

    public CompleteOrderCommandHandlerTest()
    {

        _orderRepositoryMock = new Mock<IOrderRepository>();
    }

    [Theory]
    [InlineData(-1)]
    public async Task Handle_return_false_if_order_is_not_found(int orderId)
    {
        var fakeCompleteOrderCommand = FakeCompleteOrderCommand(orderId);

        _orderRepositoryMock.Setup(orderRepo => orderRepo.GetAsync(It.IsAny<int>()))
            .Returns(Task.FromResult<Order>(null));

        //Act
        var handler = new CompleteOrderCommandHandler(_orderRepositoryMock.Object);

        var result = await handler.Handle(fakeCompleteOrderCommand, new CancellationToken());

        //Assert
        Assert.False(result);
    }

    private CompleteOrderCommand FakeCompleteOrderCommand(int orderId) => new(orderId);
}
