using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

//using Microsoft.eShopOnContainers.BuildingBlocks.EventBus;
//using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
//using Microsoft.eShopOnContainers.BuildingBlocks.EventBusRabbitMQ;
//using Microsoft.eShopOnContainers.BuildingBlocks.EventBusServiceBus;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data.Common;

//using Microsoft.ApplicationInsights.ServiceFabric;
//using RabbitMQ.Client;
//using Microsoft.Azure.ServiceBus;
//using Microsoft.Extensions.HealthChecks;

//using Ordering.BackgroundTasksWebHost.Application.IntegrationEvents;
//using Ordering.BackgroundTasksWebHost.Application.IntegrationEvents.Events;

using Ordering.BackgroundTasksWebHost.Infrastructure.AutofacModules;
using Ordering.BackgroundTasksWebHost.HostedServices;

//using Ordering.BackgroundTasksWebHost.HostedServices;


namespace Ordering.BackgroundTasksWebHost
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {            
            //RegisterAppInsights(services);

            // Configure GracePeriodManager Hosted Service
            services.AddSingleton<IHostedService, GracePeriodManagerService>();

            //services.AddTransient<IOrderingIntegrationEventService, OrderingIntegrationEventService>();

            //services.AddHealthChecks(checks =>
            //{
            //    var minutes = 1;
            //    if (int.TryParse(Configuration["HealthCheck:Timeout"], out var minutesParsed))
            //    {
            //        minutes = minutesParsed;
            //    }
            //    checks.AddSqlCheck("OrderingDb", Configuration["ConnectionString"], TimeSpan.FromMinutes(minutes));
            //});

            services.Configure<OrderingBackgroundSettings>(Configuration);


            // Add application services.

            //services.AddTransient<IOrderingIntegrationEventService, OrderingIntegrationEventService>();

            //if (Configuration.GetValue<bool>("AzureServiceBusEnabled"))
            //{
            //    services.AddSingleton<IServiceBusPersisterConnection>(sp =>
            //    {
            //        var logger = sp.GetRequiredService<ILogger<DefaultServiceBusPersisterConnection>>();

            //        var serviceBusConnectionString = Configuration["EventBusConnection"];
            //        var serviceBusConnection = new ServiceBusConnectionStringBuilder(serviceBusConnectionString);

            //        return new DefaultServiceBusPersisterConnection(serviceBusConnection, logger);
            //    });
            //}
            //else
            //{
            //    services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            //    {
            //        var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();


            //        var factory = new ConnectionFactory()
            //        {
            //            HostName = Configuration["EventBusConnection"]
            //        };

            //        if (!string.IsNullOrEmpty(Configuration["EventBusUserName"]))
            //        {
            //            factory.UserName = Configuration["EventBusUserName"];
            //        }

            //        if (!string.IsNullOrEmpty(Configuration["EventBusPassword"]))
            //        {
            //            factory.Password = Configuration["EventBusPassword"];
            //        }

            //        var retryCount = 5;
            //        if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
            //        {
            //            retryCount = int.Parse(Configuration["EventBusRetryCount"]);
            //        }

            //        return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            //    });
            //}

            //RegisterEventBus(services);

            services.AddOptions();

            //Configure autofac
            var container = new ContainerBuilder();
            container.Populate(services);

            container.RegisterModule(new ApplicationModule(Configuration["ConnectionString"]));

            return new AutofacServiceProvider(container.Build());
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            //loggerFactory.AddAzureWebAppDiagnostics();
            //loggerFactory.AddApplicationInsights(app.ApplicationServices, LogLevel.Trace);

            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                loggerFactory.CreateLogger("init").LogDebug($"Using PATH BASE '{pathBase}'");
                app.UsePathBase(pathBase);
            }
            
            //ConfigureEventBus(app);
        }

        //private void RegisterAppInsights(IServiceCollection services)
        //{
        //    services.AddApplicationInsightsTelemetry(Configuration);
        //    var orchestratorType = Configuration.GetValue<string>("OrchestratorType");

        //    if (orchestratorType?.ToUpper() == "K8S")
        //    {
        //        // Enable K8s telemetry initializer
        //        services.EnableKubernetes();
        //    }
        //    if (orchestratorType?.ToUpper() == "SF")
        //    {
        //        // Enable SF telemetry initializer
        //        services.AddSingleton<ITelemetryInitializer>((serviceProvider) =>
        //            new FabricTelemetryInitializer());
        //    }
        //}

        //private void ConfigureEventBus(IApplicationBuilder app)
        //{
        //    var eventBus = app.ApplicationServices.GetRequiredService<BuildingBlocks.EventBus.Abstractions.IEventBus>();

        //    eventBus.Subscribe<UserCheckoutAcceptedIntegrationEvent, IIntegrationEventHandler<UserCheckoutAcceptedIntegrationEvent>>();
        //    eventBus.Subscribe<GracePeriodConfirmedIntegrationEvent, IIntegrationEventHandler<GracePeriodConfirmedIntegrationEvent>>();
        //    eventBus.Subscribe<OrderStockConfirmedIntegrationEvent, IIntegrationEventHandler<OrderStockConfirmedIntegrationEvent>>();
        //    eventBus.Subscribe<OrderStockRejectedIntegrationEvent, IIntegrationEventHandler<OrderStockRejectedIntegrationEvent>>();
        //    eventBus.Subscribe<OrderPaymentFailedIntegrationEvent, IIntegrationEventHandler<OrderPaymentFailedIntegrationEvent>>();
        //    eventBus.Subscribe<OrderPaymentSuccededIntegrationEvent, IIntegrationEventHandler<OrderPaymentSuccededIntegrationEvent>>();
        //}


        //private void RegisterEventBus(IServiceCollection services)
        //{
        //    var subscriptionClientName = Configuration["SubscriptionClientName"];

        //    if (Configuration.GetValue<bool>("AzureServiceBusEnabled"))
        //    {
        //        services.AddSingleton<IEventBus, EventBusServiceBus>(sp =>
        //        {
        //            var serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
        //            var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
        //            var logger = sp.GetRequiredService<ILogger<EventBusServiceBus>>();
        //            var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

        //            return new EventBusServiceBus(serviceBusPersisterConnection, logger,
        //                eventBusSubcriptionsManager, subscriptionClientName, iLifetimeScope);
        //        });
        //    }
        //    else
        //    {
        //        services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
        //        {
        //            var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
        //            var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
        //            var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
        //            var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

        //            var retryCount = 5;
        //            if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
        //            {
        //                retryCount = int.Parse(Configuration["EventBusRetryCount"]);
        //            }

        //            return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
        //        });
        //    }

        //    services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
        //}

    }
}
