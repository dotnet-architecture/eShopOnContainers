using System;
using System.Collections.Generic;

namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBusRabbitMQ
{
    public interface IMultiRabbitMQPersistentConnections
    {
        List<IRabbitMQPersistentConnection> GetConnections();
        void AddConnection(IRabbitMQPersistentConnection connection);
        void RemoveConnection(IRabbitMQPersistentConnection connection);
    }
}