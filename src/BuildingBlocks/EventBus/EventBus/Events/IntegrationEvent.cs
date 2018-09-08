using DateTime = System.DateTime;
using Guid = System.Guid;

namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events
{
	public class IntegrationEvent
	{
		public IntegrationEvent()
		{
			Id = Guid.NewGuid();
			CreationDate = DateTime.UtcNow;
		}

		public Guid Id { get; }
		public DateTime CreationDate { get; }
	}
}
