using Microsoft.Azure.ServiceBus;
using System;

namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBusServiceBus
{
    public class DefaultServiceBusPersisterConnection : IServiceBusPersisterConnection
    {
        private readonly ServiceBusConnectionStringBuilder _serviceBusConnectionStringBuilder;
        private readonly string _subscriptionClientName;
        private ITopicClient _topicClient;
        private SubscriptionClient _subscriptionClient;

        bool _disposed;

        public DefaultServiceBusPersisterConnection(ServiceBusConnectionStringBuilder serviceBusConnectionStringBuilder,
            string subscriptionClientName)
        {
            _serviceBusConnectionStringBuilder = serviceBusConnectionStringBuilder ?? 
                throw new ArgumentNullException(nameof(serviceBusConnectionStringBuilder));
            _subscriptionClientName = subscriptionClientName;
            _topicClient = new TopicClient(_serviceBusConnectionStringBuilder, RetryPolicy.Default);
            _subscriptionClient = new SubscriptionClient(_serviceBusConnectionStringBuilder, subscriptionClientName);
        }

        public ITopicClient TopicClient
        {
            get
            {
                if (_topicClient.IsClosedOrClosing)
                {
                    _topicClient = new TopicClient(_serviceBusConnectionStringBuilder, RetryPolicy.Default);
                }

                return _topicClient;
            }
        }

        public ISubscriptionClient SubscriptionClient
        {
            get
            {
                if (_subscriptionClient.IsClosedOrClosing)
                {
                    _subscriptionClient = new SubscriptionClient(_serviceBusConnectionStringBuilder, _subscriptionClientName);
                }

                return _subscriptionClient;
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;
        }
    }
}
