namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBusServiceBus
{
    using Microsoft.Azure.ServiceBus;
    using System;

    public interface IServiceBusPersisterConnection : IDisposable
    {
        ITopicClient TopicClient { get; }
        ISubscriptionClient SubscriptionClient { get; }
    }
}