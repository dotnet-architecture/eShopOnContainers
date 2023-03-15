namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBusKafka;

public class EventBusKafka : IEventBus, IDisposable
{
    // for now use single topic and event names as keys for messages
    // such that they always land in same partition and we have ordering guarantee
    // then the consumers have to ignore events they are not subscribed to
    // alternatively could have multiple topics (associated with event name)
    private const string TopicName = "eshop_event_bus";

    private readonly IKafkaPersistentConnection _persistentConnection;
    private readonly ILogger<EventBusKafka> _logger;
    private readonly IEventBusSubscriptionsManager _subsManager;
    private const string IntegrationEventSuffix = "IntegrationEvent";
    
    
    // Object that will be registered as singleton to each service on startup,
    // which will be used to publish and subscribe to events.
    public EventBusKafka(IKafkaPersistentConnection persistentConnection,
        ILogger<EventBusKafka> logger, IEventBusSubscriptionsManager subsManager)
    {
        _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _subsManager = subsManager;
    }
    
    public void Publish(IntegrationEvent @event)
    {
        var eventName = @event.GetType().Name;
        var jsonMessage = JsonSerializer.Serialize(@event, @event.GetType());
        
        // map Integration event to kafka message
        // event name something like OrderPaymentSucceededIntegrationEvent
        var message = new Message<string, string> { Key = eventName, Value = jsonMessage };
        var kafkaHandle = 
            new DependentProducerBuilder<string, string>(_persistentConnection.Handle).Build();
        
        Console.WriteLine($"Publishing event: {eventName}\n Content: {Utils.CalculateMd5Hash(jsonMessage)}");
        
        kafkaHandle.ProduceAsync(TopicName, message);
    }

    public void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
    {
        var eventName = _subsManager.GetEventKey<T>();
        // DoInternalSubscription(eventName);

        _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, typeof(TH).GetGenericTypeName());

        try {
            _subsManager.AddSubscription<T, TH>();
        } catch (Exception e)
        {
            Console.WriteLine($"Failed to add subscription {eventName}, because: {e.Message}");
        }
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
