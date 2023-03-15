using Confluent.Kafka.Admin;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBusKafka;

public class KafkaAdminBackgroundService : BackgroundService
{
    private const string TopicName = "eshop_event_bus";
    private readonly ILogger<KafkaAdminBackgroundService> _logger;
    private readonly IConfiguration _configuration;

    public KafkaAdminBackgroundService(
        IConfiguration configuration,
        ILogger<KafkaAdminBackgroundService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(CreateTopics, stoppingToken);
    }

    private async Task CreateTopics()
    {
        var adminConfig = new AdminClientConfig();
        _configuration.GetSection("Kafka:AdminSettings").Bind(adminConfig);
        using (var adminClient = new AdminClientBuilder(adminConfig).Build())
        {
            try
            {
                await adminClient.CreateTopicsAsync(new TopicSpecification[] {
                    new TopicSpecification { Name = TopicName, ReplicationFactor = 1, NumPartitions = 1 } });
            }
            catch (CreateTopicsException e)
            {
                Console.WriteLine($"An error occured creating topic {e.Results[0].Topic}: {e.Results[0].Error.Reason}");
            }
        }
    }
}
