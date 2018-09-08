using Microsoft.EntityFrameworkCore;
using static System.Linq.Enumerable;
using ArgumentNullException = System.ArgumentNullException;
using DbConnection = System.Data.Common.DbConnection;
using DbTransaction = System.Data.Common.DbTransaction;
using RelationalEventId = Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.eShopOnContainers.BuildingBlocks.IntegrationEventLogEF.Services
{
	using IntegrationEvent = EventBus.Events.IntegrationEvent;

	public class IntegrationEventLogService : IIntegrationEventLogService
	{
		private readonly IntegrationEventLogContext _integrationEventLogContext;
		private readonly DbConnection _dbConnection;

		public IntegrationEventLogService(DbConnection dbConnection)
		{
			_dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
			_integrationEventLogContext = new IntegrationEventLogContext(
			    new DbContextOptionsBuilder<IntegrationEventLogContext>()
				   .UseSqlServer(_dbConnection)
				   .ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning))
				   .Options);
		}

		public Task SaveEventAsync(IntegrationEvent @event, DbTransaction transaction)
		{
			if (transaction == null)
			{
				throw new ArgumentNullException(nameof(transaction), $"A {typeof(DbTransaction).FullName} is required as a pre-requisite to save the event.");
			}

			var eventLogEntry = new IntegrationEventLogEntry(@event);

			_integrationEventLogContext.Database.UseTransaction(transaction);
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
