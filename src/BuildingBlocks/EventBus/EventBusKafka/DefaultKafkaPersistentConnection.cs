namespace EventBusKafka;


/// <summary>
/// Class for making sure we do not open new producer context (expensive)
/// everytime a service publishes an event.
/// On startup each service creates an singleton instance of this class,
/// which is then used when publishing any events.
///
/// based on https://github.com/confluentinc/confluent-kafka-dotnet/blob/master/examples/Web/KafkaClientHandle.cs
///  
/// </summary>
public class DefaultKafkaPersistentConnection
    : IKafkaPersistentConnection
{

    private readonly ILogger<DefaultKafkaPersistentConnection> _logger;
    IProducer<byte[], byte[]> _kafkaProducer;

    public DefaultKafkaPersistentConnection(String brokerList,
        ILogger<DefaultKafkaPersistentConnection> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        // TODO: fix configuration passing for producer
        // for now just assume we give  "localhost:9092" as argument
        var conf = new ProducerConfig { BootstrapServers = brokerList };
        
        // TODO maybe we need to retry this? -> as it could fail
        _kafkaProducer = new ProducerBuilder<byte[], byte[]>(conf).Build();
    }

    public Handle Handle => _kafkaProducer.Handle;

    public void Dispose()
    {
        // Block until all outstanding produce requests have completed (with or
        // without error).
        _kafkaProducer.Flush();
        _kafkaProducer.Dispose();
    }
}