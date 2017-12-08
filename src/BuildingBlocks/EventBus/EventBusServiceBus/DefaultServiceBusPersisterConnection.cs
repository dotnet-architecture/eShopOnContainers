using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using System;

namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBusServiceBus
{
    public class DefaultServiceBusPersisterConnection :IServiceBusPersisterConnection
    {
        private readonly ILogger<DefaultServiceBusPersisterConnection> _logger;
        private readonly ServiceBusConnectionStringBuilder _serviceBusConnectionStringBuilder;
        private ITopicClient _topicClient;

        bool _disposed;

        public DefaultServiceBusPersisterConnection(ServiceBusConnectionStringBuilder serviceBusConnectionStringBuilder,
            ILogger<DefaultServiceBusPersisterConnection> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            _serviceBusConnectionStringBuilder = serviceBusConnectionStringBuilder ?? 
                throw new ArgumentNullException(nameof(serviceBusConnectionStringBuilder));
            _topicClient = new TopicClient(_serviceBusConnectionStringBuilder, RetryPolicy.Default);
        }

        public ServiceBusConnectionStringBuilder ServiceBusConnectionStringBuilder => _serviceBusConnectionStringBuilder;

        public ITopicClient CreateModel()
        {
            if(_topicClient.IsClosedOrClosing)
            {
                _topicClient = new TopicClient(_serviceBusConnectionStringBuilder, RetryPolicy.Default);
            }

            return _topicClient;
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;
        }
    }
}
