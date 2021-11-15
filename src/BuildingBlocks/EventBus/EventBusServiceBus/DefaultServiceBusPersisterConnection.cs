namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBusServiceBus;

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
