using System;
using MediatR;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.Seedwork;
using Microsoft.eShopOnContainers.Services.Ordering.Infrastructure;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ordering.Domain.SeedWork;

namespace Ordering.Infrastructure
{
    public static class MediatorExtension
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, OrderingContext ctx, CancellationToken cancellationToken)
        {
            var domainEntities = ctx.ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            domainEntities.ToList()
                .ForEach(entity => entity.Entity.ClearDomainEvents());

            var tasks = domainEvents
                .Select(async (domainEvent) => {
                    await mediator.PublishEvent(domainEvent, cancellationToken);
                });

            await Task.WhenAll(tasks);
        }

        public static Task PublishEvent(this IMediator mediator, IDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var eventType = domainEvent.GetType();
            var domainEventNotification = Activator.CreateInstance(typeof(DomainEventNotification<>).MakeGenericType(eventType), domainEvent);

            var mediatorPublishMethod = mediator.GetType().GetMethod("Publish");
            var genericMediatorPublishMethod = mediatorPublishMethod.MakeGenericMethod(domainEventNotification.GetType());
            return (Task)genericMediatorPublishMethod.Invoke(mediator, new[] { domainEventNotification, cancellationToken });
        }
    }
}
