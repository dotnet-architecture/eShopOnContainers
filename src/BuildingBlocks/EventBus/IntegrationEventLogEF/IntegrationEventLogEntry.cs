using DateTime = System.DateTime;
using Guid = System.Guid;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace Microsoft.eShopOnContainers.BuildingBlocks.IntegrationEventLogEF
{
	using IntegrationEvent = EventBus.Events.IntegrationEvent;

	public class IntegrationEventLogEntry
	{
		private IntegrationEventLogEntry() { }
		public IntegrationEventLogEntry(IntegrationEvent @event)
		{
			EventId = @event.Id;
			CreationTime = @event.CreationDate;
			EventTypeName = @event.GetType().FullName;
			Content = JsonConvert.SerializeObject(@event);
			State = EventStateEnum.NotPublished;
			TimesSent = 0;
		}
		public Guid EventId { get; private set; }
		public string EventTypeName { get; private set; }
		public EventStateEnum State { get; set; }
		public int TimesSent { get; set; }
		public DateTime CreationTime { get; private set; }
		public string Content { get; private set; }
	}
}
