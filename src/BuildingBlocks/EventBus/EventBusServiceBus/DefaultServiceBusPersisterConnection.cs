using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBusServiceBus
{
    public class DefaultServiceBusPersisterConnection : ServiceBusConnection, IServiceBusPersisterConnection
    {
        private readonly ILogger<ServiceBusConnection> _logger;
        private readonly ServiceBusConnectionStringBuilder _serviceBusConnectionStringBuilder;
        private ITopicClient _topicClient;

        bool _disposed;
        object sync_root = new object();

        public DefaultServiceBusPersisterConnection(ServiceBusConnectionStringBuilder serviceBusConnectionStringBuilder, 
            TimeSpan operationTimeout, RetryPolicy retryPolicy, ILogger<ServiceBusConnection> logger)
            : base(operationTimeout, retryPolicy)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            InitializeConnection(serviceBusConnectionStringBuilder);
            _serviceBusConnectionStringBuilder = serviceBusConnectionStringBuilder ?? 
                throw new ArgumentNullException(nameof(serviceBusConnectionStringBuilder));
        }

        public bool IsConnected => _topicClient.IsClosedOrClosing;

        public ServiceBusConnectionStringBuilder ServiceBusConnectionStringBuilder => _serviceBusConnectionStringBuilder;

        public ITopicClient CreateModel()
        {
            if(_topicClient.IsClosedOrClosing)
            {
                _topicClient = new TopicClient(_serviceBusConnectionStringBuilder, RetryPolicy);
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
