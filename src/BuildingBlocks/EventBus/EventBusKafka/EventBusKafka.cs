namespace EventBusKafka;

public class EventBusKafka : IEventBus, IDisposable
{
    // for now use single topic and event names as keys for messages
    // such that they always land in same partition and we have ordering guarantee
    // then the consumers have to ignore events they are not subscribed to
    // alternatively could have multiple topics (associated with event name)
    private readonly string _topicName = "eshop_event_bus";

    private readonly IKafkaPersistentConnection _persistentConnection;
    private readonly ILogger<EventBusKafka> _logger;
    private readonly IEventBusSubscriptionsManager _subsManager;
    private readonly int _retryCount;
    private const string INTEGRATION_EVENT_SUFFIX = "IntegrationEvent";
    
    
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
        var eventName = @event.GetType().Name.Replace(INTEGRATION_EVENT_SUFFIX, "");
        var jsonMessage = JsonSerializer.Serialize(@event, @event.GetType());
        
        // map Integration event to kafka message
        // event name something like OrderPaymentSucceededIntegrationEvent
        var message = new Message<string, string> { Key = eventName, Value = jsonMessage };
        var kafkaHandle = 
            new DependentProducerBuilder<string, string>(_persistentConnection.Handle).Build();
        kafkaHandle.ProduceAsync(_topicName, message);
    }

    public void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
    {
        var eventName = _subsManager.GetEventKey<T>();
        // DoInternalSubscription(eventName);

        _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, typeof(TH).GetGenericTypeName());

        _subsManager.AddSubscription<T, TH>();
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