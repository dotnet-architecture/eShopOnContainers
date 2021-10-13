namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBusServiceBus;

public interface IServiceBusPersisterConnection : IDisposable
{
    ITopicClient TopicClient { get; }
    ISubscriptionClient SubscriptionClient { get; }
}
