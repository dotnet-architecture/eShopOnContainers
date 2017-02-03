using Microsoft.eShopOnContainers.Services.Ordering.Domain.Seedwork;

namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate
{ 
    //This is just the RepositoryContracts or Interface defined at the Domain Layer
    //as requisite for the Order Aggregate
    public interface IOrderRepository
        :IAggregateRepository
    {
        Order Add(Order order);
    }
}
