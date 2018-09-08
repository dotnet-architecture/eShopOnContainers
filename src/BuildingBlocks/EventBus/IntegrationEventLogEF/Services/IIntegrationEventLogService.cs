using DbTransaction = System.Data.Common.DbTransaction;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.eShopOnContainers.BuildingBlocks.IntegrationEventLogEF.Services
{
	using IntegrationEvent = EventBus.Events.IntegrationEvent;

	public interface IIntegrationEventLogService
	{
		Task SaveEventAsync(IntegrationEvent @event, DbTransaction transaction);
		Task MarkEventAsPublishedAsync(IntegrationEvent @event);
	}
}
