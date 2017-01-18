namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.RepositoryContracts
{
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;

    public interface IOrderRepository
        :IRepository
    {
        Order Add(Order order);
    }
}
