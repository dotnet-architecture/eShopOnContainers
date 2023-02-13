namespace EventBusKafka;

public class EventBusKafka : IEventBus, IDisposable
{
    const string BROKER_NAME = "eshop_event_bus";

    private readonly IKafkaPersistentConnection _persistentConnection;
    private readonly ILogger<EventBusKafka> _logger;
    private readonly IEventBusSubscriptionsManager _subsManager;
    private readonly int _retryCount;
    
    
    // Object that will be registered as singleton to each service on startup,
    // which will be used to publish and subscribe to events.
    public EventBusKafka(IKafkaPersistentConnection persistentConnection,
        ILogger<EventBusKafka> logger, IEventBusSubscriptionsManager subsManager, int retryCount = 5)
    {
        _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
        _retryCount = retryCount;
    }
    
    public void Publish(IntegrationEvent @event)
    {
        throw new NotImplementedException();
    }

    public void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
    {
        throw new NotImplementedException();
    }

    public void SubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
    {
        throw new NotImplementedException();
    }

    // Taken directly from rabbitMQ implementation
    public void UnsubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
    {
        _subsManager.RemoveDynamicSubscription<TH>(eventName);
    }

    // Taken directly from rabbitMQ implementation
    public void Unsubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
    {
        var eventName = _subsManager.GetEventKey<T>();
        _logger.LogInformation("Unsubscribing from event {EventName}", eventName);
        _subsManager.RemoveSubscription<T, TH>();        
    }

    public void Dispose()
    {
        _subsManager.Clear();
    }
}