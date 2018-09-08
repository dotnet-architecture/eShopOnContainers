using IDisposable = System.IDisposable;

namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBusServiceBus
{
	using ITopicClient = Azure.ServiceBus.ITopicClient;
	using ServiceBusConnectionStringBuilder = Azure.ServiceBus.ServiceBusConnectionStringBuilder;

	public interface IServiceBusPersisterConnection : IDisposable
	{
		ServiceBusConnectionStringBuilder ServiceBusConnectionStringBuilder { get; }

		ITopicClient CreateModel();
	}
}