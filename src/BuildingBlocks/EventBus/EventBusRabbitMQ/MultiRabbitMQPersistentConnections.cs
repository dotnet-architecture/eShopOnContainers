using System.Collections.Generic;

namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBusRabbitMQ
{
    public class MultiRabbitMQPersistentConnections : IMultiRabbitMQPersistentConnections
    {
        public List<IRabbitMQPersistentConnection> Connections;

        public MultiRabbitMQPersistentConnections()
        {
            Connections = new List<IRabbitMQPersistentConnection>();
        }


        public List<IRabbitMQPersistentConnection> GetConnections()
        {
            return Connections;
        }

        public void AddConnection(IRabbitMQPersistentConnection connection)
        {
            Connections.Add((connection));
        }

        public void RemoveConnection(IRabbitMQPersistentConnection connection)
        {
            Connections.Remove(connection);
        }
    }
}