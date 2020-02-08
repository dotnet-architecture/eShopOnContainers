using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.ServiceBus;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBusRabbitMQ;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBusServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ordering.BackgroundTasks.Configuration;
using Ordering.BackgroundTasks.Tasks;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Ordering.BackgroundTasks
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //add health check for this service
            services.AddCustomHealthCheck(Configuration);

            //configure settings

            services.Configure<BackgroundTaskSettings>(Configuration);
            services.AddOptions();

            //configure background task

            services.AddSingleton<IHostedService, GracePeriodManagerService>();

            //configure event bus related services
            
            services.AddSingleton<IMultiRabbitMQPersistentConnections>(sp =>
            {
                IMultiRabbitMQPersistentConnections connections = new MultiRabbitMQPersistentConnections();
                connections.AddConnection(GenerateConnection("TenantA", sp));
                connections.AddConnection(GenerateConnection("TenantB", sp));

                return connections;
            });
            

            RegisterEventBus(services);

            //create autofac based service provider
            var container = new ContainerBuilder();
            container.Populate(services);


            return new AutofacServiceProvider(container.Build());
        }

        private IRabbitMQPersistentConnection GenerateConnection(String vHost, IServiceProvider sp)
        {
            var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

            var factory = new ConnectionFactory()
            {
                HostName = Configuration["EventBusConnection"],
                DispatchConsumersAsync = true
            };

            if (!string.IsNullOrEmpty(Configuration["EventBusUserName"]))
            {
                factory.UserName = Configuration["EventBusUserName"];
            }

            if (!string.IsNullOrEmpty(Configuration["EventBusPassword"]))
            {
                factory.Password = Configuration["EventBusPassword"];
            }

            factory.VirtualHost = vHost;

            var retryCount = 5;
            if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
            {
                retryCount = int.Parse(Configuration["EventBusRetryCount"]);
            }

            return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseHealthChecks("/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseHealthChecks("/liveness", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });
        }


        private void RegisterEventBus(IServiceCollection services)
        {
            var subscriptionClientName = Configuration["SubscriptionClientName"];
            
            services.AddSingleton<IMultiEventBus, MultiEventBusRabbitMQ>(sp =>
            {
                var multiRabbitMqPersistentConnections = sp.GetRequiredService<IMultiRabbitMQPersistentConnections>();
                var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                var retryCount = 5;
                if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(Configuration["EventBusRetryCount"]);
                }

                List<IEventBus> eventBuses = new List<IEventBus>();

                eventBuses.Add(new EventBusRabbitMQ(multiRabbitMqPersistentConnections.GetConnections()[0], logger,
                    iLifetimeScope, eventBusSubcriptionsManager, "TenantA", subscriptionClientName, retryCount));
                eventBuses.Add(new EventBusRabbitMQ(multiRabbitMqPersistentConnections.GetConnections()[1], logger,
                    iLifetimeScope, eventBusSubcriptionsManager, "TenantB", subscriptionClientName, retryCount));
                Dictionary<int, String> tenants = new Dictionary<int, string>();
                tenants.Add(1, "TenantA");
                tenants.Add(2, "TenantB");

                return new MultiEventBusRabbitMQ(eventBuses, tenants);
            });
            

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
        }
    }

    public static class CustomExtensionMethods
    {
        public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
        {
            var hcBuilder = services.AddHealthChecks();

            hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

            hcBuilder
                .AddSqlServer(
                    configuration["ConnectionString"],
                    name: "OrderingTaskDB-check",
                    tags: new string[] { "orderingtaskdb" });

            if (configuration.GetValue<bool>("AzureServiceBusEnabled"))
            {
                hcBuilder
                    .AddAzureServiceBusTopic(
                        configuration["EventBusConnection"],
                        topicName: "eshop_event_bus",
                        name: "orderingtask-servicebus-check",
                        tags: new string[] { "servicebus" });
            }
            else
            {
                hcBuilder
                    .AddRabbitMQ(
                        $"amqp://{configuration["EventBusConnection"]}",
                        name: "orderingtask-rabbitmqbus-check",
                        tags: new string[] { "rabbitmqbus" });
            }

            return services;
        }
    }
}
