using SagaManager.IntegrationEvents;

namespace SagaManager
{
    using System.IO;
    using System;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBusRabbitMQ;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using RabbitMQ.Client;
    using Services;

    public class Program
    {
        public static IConfigurationRoot Configuration { get; set; }

        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddOptions()
                .Configure<SagaManagerSettings>(Configuration)
                .AddSingleton<ISagaManagerService, SagaManagerService>()
                .AddSingleton<IConfirmGracePeriodEvent, ConfirmGracePeriodEvent>()
                .AddSingleton<IRabbitMQPersisterConnection>(sp =>
                {
                    var settings = sp.GetRequiredService<IOptions<SagaManagerSettings>>().Value;
                    var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersisterConnection>>();
                    var factory = new ConnectionFactory()
                    {
                        HostName = settings.EventBusConnection
                    };

                    return new DefaultRabbitMQPersisterConnection(factory, logger);
                })
                .AddSingleton<IEventBus, EventBusRabbitMQ>()
                .BuildServiceProvider();

            //configure console logging
            serviceProvider
                .GetService<ILoggerFactory>()
                .AddConsole(Configuration.GetSection("Logging"))
                .AddConsole(LogLevel.Debug);

            var sagaManagerService = serviceProvider
                .GetRequiredService<ISagaManagerService>();

            while (true)
            {
                sagaManagerService.CheckFinishedGracePeriodOrders();
                System.Threading.Thread.Sleep(30000);
            }
        }
    }
}