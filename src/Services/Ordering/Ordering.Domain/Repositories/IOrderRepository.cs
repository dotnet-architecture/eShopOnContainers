namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.Repositories
{
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;

    public interface IOrderRepository
        :IRepository
    {
        Order Add(Order order);
    }
}
