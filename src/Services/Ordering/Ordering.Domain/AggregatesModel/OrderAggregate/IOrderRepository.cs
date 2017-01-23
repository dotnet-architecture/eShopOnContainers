namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate
{
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;

    //This is just the RepositoryContracts or Interface defined at the Domain Layer
    //as requisite for the Order Aggregate
    public interface IOrderRepository
        :IRepository
    {
        Order Add(Order order);
    }
}
