using Microsoft.Extensions.Configuration;

namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBusKafka;

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
    private readonly IProducer<byte[], byte[]> _kafkaProducer;

    public DefaultKafkaPersistentConnection(IConfiguration configuration)
    {
        var producerConfig = new ProducerConfig();
        configuration.GetSection("Kafka:ProducerSettings").Bind(producerConfig);
        _kafkaProducer = new ProducerBuilder<byte[], byte[]>(producerConfig).Build();
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