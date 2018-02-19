using MediatR;
using Ordering.Domain.SeedWork;

namespace Microsoft.eShopOnContainers.Services.Ordering.Infrastructure
{
    public class DomainEventNotification<T>: INotification where T: IDomainEvent
    {
        public DomainEventNotification(T domainEvent)
        {
            Event = domainEvent;
        }

        public T Event { get; }
    }
}