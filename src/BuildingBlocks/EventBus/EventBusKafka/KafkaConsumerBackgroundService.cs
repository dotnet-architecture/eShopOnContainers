using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBusKafka;

/* Inspired by https://github.com/confluentinc/confluent-kafka-dotnet/blob/master/examples/Web/RequestTimeConsumer.cs */
public class KafkaConsumerBackgroundService : BackgroundService
{
    private readonly IEventBusSubscriptionsManager _subsManager;
    private readonly ILifetimeScope _autofac;
    private readonly ILogger<KafkaConsumerBackgroundService> _logger;
    private readonly IConsumer<string, string> _kafkaConsumer;
    private const string AutofacScopeName = "eshop_event_bus";
    private const string TopicName = "eshop_event_bus";

    public KafkaConsumerBackgroundService(IConfiguration configuration, 
        IEventBusSubscriptionsManager subsManager, 
        ILifetimeScope autofac,
        ILogger<KafkaConsumerBackgroundService> logger) 
    {
        var consumerConfig = new ConsumerConfig();
        configuration.GetSection("Kafka:ConsumerSettings").Bind(consumerConfig);
        _kafkaConsumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        
        _subsManager = subsManager;
        _autofac = autofac;
        _logger = logger;
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() => StartConsumerLoop(stoppingToken), stoppingToken);
    }

    private async Task StartConsumerLoop(CancellationToken cancellationToken)
    {
        _kafkaConsumer.Subscribe(TopicName);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = _kafkaConsumer.Consume(cancellationToken);

                var eventName = consumeResult.Message.Key;
                var messageContent = consumeResult.Message.Value;

                if (!_subsManager.HasSubscriptionsForEvent(eventName))
                {
                    _logger.LogWarning("No subscription for Kafka event: {EventName}", eventName);
                    continue;
                }
                
                await using var scope = _autofac.BeginLifetimeScope(AutofacScopeName);
                var subscriptions = _subsManager.GetHandlersForEvent(eventName);
                foreach (var subscription in subscriptions)
                {
                    #region dynamic subscription
                    /* We do not support dynamic subscription at the moment*/
                    // if (subscription.IsDynamic)
                    // {
                    //     if (scope.ResolveOptional(subscription.HandlerType) is not IDynamicIntegrationEventHandler handler) continue;
                    //     using dynamic eventData = JsonDocument.Parse(message);
                    //     await Task.Yield();
                    //     await handler.Handle(eventData);
                    // }
                    #endregion
                    
                    /* The code below applies to non-dynamic subscriptions only */
                    var handler = scope.ResolveOptional(subscription.HandlerType);
                    if (handler == null) continue;
                    var eventType = _subsManager.GetEventTypeByName(eventName);
                    var integrationEvent = JsonSerializer.Deserialize(messageContent, 
                        eventType, 
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                    await Task.Yield();
                    await (Task)concreteType.GetMethod("Handle")
                        .Invoke(handler, new object[] { integrationEvent });
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (ConsumeException e)
            {
                // Consumer errors should generally be ignored (or logged) unless fatal.
                Console.WriteLine($"Consume error: {e.Error.Reason}");

                if (e.Error.IsFatal)
                {
                    // https://github.com/edenhill/librdkafka/blob/master/INTRODUCTION.md#fatal-consumer-errors
                    break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected error: {e}");
                break;
            }
        }
    }
        
    public override void Dispose()
    {
        _kafkaConsumer.Close(); // Commit offsets and leave the group cleanly.
        _kafkaConsumer.Dispose();

        base.Dispose();
    }
}