using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.eShopOnContainers.BuildingBlocks.IntegrationEventLogEF;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Catalog.API.IntegrationEvents
{
    public class IntegrationEventLogService : IIntegrationEventLogService
    {
        private readonly IntegrationEventLogContext _integrationEventLogContext;
        private readonly CatalogContext _catalogContext;

        public IntegrationEventLogService(CatalogContext catalogContext)
        {
            _catalogContext = catalogContext;
            _integrationEventLogContext = new IntegrationEventLogContext(
                new DbContextOptionsBuilder<IntegrationEventLogContext>()
                    .UseSqlServer(catalogContext.Database.GetDbConnection())
                    .ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning))
                    .Options);
        }

        public Task SaveEventAsync(IntegrationEvent @event)
        {
            var eventLogEntry = new IntegrationEventLogEntry(@event);
            
            // as a constraint this transaction has to be done together with a catalogContext transaction
            _integrationEventLogContext.Database.UseTransaction(_catalogContext.Database.CurrentTransaction.GetDbTransaction());
            _integrationEventLogContext.IntegrationEventLogs.Add(eventLogEntry);

            return _integrationEventLogContext.SaveChangesAsync();
        }

        public Task MarkEventAsPublishedAsync(IntegrationEvent @event)
        {
            var eventLogEntry = _integrationEventLogContext.IntegrationEventLogs.Single(ie => ie.EventId == @event.Id);
            eventLogEntry.TimesSent++;
            eventLogEntry.State = EventStateEnum.Published;

            _integrationEventLogContext.IntegrationEventLogs.Update(eventLogEntry);

            return _integrationEventLogContext.SaveChangesAsync();
        }
    }
}
