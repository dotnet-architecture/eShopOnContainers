namespace SagaManager
{
    using System.IO;
    using System;
    using System.Threading.Tasks;
    using Autofac.Extensions.DependencyInjection;
    using Autofac;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBusRabbitMQ;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using RabbitMQ.Client;
    using Services;
    using IntegrationEvents;

    public class Program
    {
        public static IConfigurationRoot Configuration { get; set; }

        public static void Main(string[] args) => MainAsync().Wait();
        
        static async Task MainAsync()
        {
            StartUp();

            IServiceCollection services = new ServiceCollection();
            var serviceProvider = ConfigureServices(services);

            var logger = serviceProvider.GetService<ILoggerFactory>();
            Configure(logger);

            var sagaManagerService = serviceProvider
                .GetRequiredService<ISagaManagerService>();

            while (true)
            {
                sagaManagerService.CheckFinishedGracePeriodOrders();
                await Task.Delay(90000);
            }
        }

        public static void StartUp()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public static IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddLogging()
                .AddOptions()
                .Configure<SagaManagerSettings>(Configuration)
                .AddSingleton<ISagaManagerService, SagaManagerService>()
                .AddSingleton<ISagaManagerIntegrationEventService, SagaManagerIntegrationEventService>()

                .AddSingleton<IRabbitMQPersistentConnection>(sp =>
                {
                    var settings = sp.GetRequiredService<IOptions<SagaManagerSettings>>().Value;
                    var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();
                    var factory = new ConnectionFactory()
                    {
                        HostName = settings.EventBusConnection
                    };

                    return new DefaultRabbitMQPersistentConnection(factory, logger);
                });

                RegisterServiceBus(services);

            var container = new ContainerBuilder();
            container.Populate(services);
            return new AutofacServiceProvider(container.Build());
        }

        public static void Configure(ILoggerFactory loggerFactory)
        {
            loggerFactory
                .AddConsole(Configuration.GetSection("Logging"))
                .AddConsole(LogLevel.Debug);
        }

        private static void RegisterServiceBus(IServiceCollection services)
        {
            services.AddSingleton<IEventBus, EventBusRabbitMQ>();
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
        }
    }
}