using Task = System.Threading.Tasks.Task;

namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions
{
	using IntegrationEvent = Events.IntegrationEvent;

	public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
	   where TIntegrationEvent : IntegrationEvent
	{
		Task Handle(TIntegrationEvent @event);
	}

	public interface IIntegrationEventHandler
	{
	}
}
