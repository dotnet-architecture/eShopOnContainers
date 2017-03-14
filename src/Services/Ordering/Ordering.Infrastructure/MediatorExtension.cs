using MediatR;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.Seedwork;
using Microsoft.eShopOnContainers.Services.Ordering.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.Infrastructure
{
    public static class MediatorExtension
    {
        public static async Task RaiseDomainEventsAsync(this IMediator mediator, OrderingContext ctx)
        {
            var domainEntities = ctx.ChangeTracker.Entries<Entity>().Where(x => x.Entity.Events != null && x.Entity.Events.Any());
            var domainEvents = domainEntities.SelectMany(x => x.Entity.Events).ToList();
            domainEntities.ToList().ForEach(entity => entity.Entity.Events.Clear());

            var tasks = domainEvents
                .Select(async (domainEvent) => {
                    await mediator.PublishAsync(domainEvent);
                });

            await Task.WhenAll(tasks);
        }
    }
}
