using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using System;

namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBusServiceBus
{
    public class DefaultServiceBusPersisterConnection : IServiceBusPersisterConnection
    {
        private readonly string _serviceBusConnectionString;
        private ServiceBusClient _topicClient;
        private ServiceBusAdministrationClient _subscriptionClient;

        bool _disposed;

        public DefaultServiceBusPersisterConnection(string serviceBusConnectionString)
        {
            _serviceBusConnectionString = serviceBusConnectionString;
            _subscriptionClient = new ServiceBusAdministrationClient(_serviceBusConnectionString);
            _topicClient = new ServiceBusClient(_serviceBusConnectionString);
        }

        public ServiceBusClient TopicClient
        {
            get
            {
                if (_topicClient.IsClosed)
                {
                    _topicClient = new ServiceBusClient(_serviceBusConnectionString);
                }
                return _topicClient;
            }
        }

        public ServiceBusAdministrationClient AdministrationClient
        {
            get
            {
                _subscriptionClient = new ServiceBusAdministrationClient("Endpoint=sb://eshopsbahsjtib3he5fg.servicebus.windows.net/;SharedAccessKeyName=Root;SharedAccessKey=2PJt+7/85C1gN6Xx+Cjn+UprCmDvwjJ9hDZ42GQRYLE=;EntityPath=eshop_event_bus");
                return _subscriptionClient;
            }
        }

        public ServiceBusClient CreateModel()
        {
            if (_topicClient.IsClosed)
            {
                _topicClient = new ServiceBusClient(_serviceBusConnectionString);
            }

            return _topicClient;
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;
            _topicClient.DisposeAsync().GetAwaiter().GetResult();
        }
    }
}
