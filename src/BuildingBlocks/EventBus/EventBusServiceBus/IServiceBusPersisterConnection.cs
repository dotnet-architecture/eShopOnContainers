namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBusServiceBus
{
    using System;
    using Microsoft.Azure.ServiceBus;

    public interface IServiceBusPersisterConnection : IDisposable
    {
        ServiceBusConnectionStringBuilder ServiceBusConnectionStringBuilder { get; }

        ITopicClient CreateModel();
    }
}