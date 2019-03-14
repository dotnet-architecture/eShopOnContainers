using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ordering.SignalrHub.AutofacModules;
using Ordering.SignalrHub.IntegrationEvents;
using Ordering.SignalrHub.IntegrationEvents.EventHandling;
using System;
using System.IdentityModel.Tokens.Jwt;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Ordering.SignalrHub
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services
                .AddCustomHealthCheck(Configuration)
                .AddCors(options =>
                {
                    options.AddPolicy("CorsPolicy",
                        builder => builder
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .SetIsOriginAllowed((host) => true)
                        .AllowCredentials());
                });

            if (Configuration.GetValue<string>("IsClusterEnv") == bool.TrueString)
            {
                services
                    .AddSignalR()
                    .AddRedis(Configuration["SignalrStoreConnectionString"]);
            }
            else
            {
                services.AddSignalR();
            }

            services.AddIntegrationEventHandler();
            services.AddCap(options =>
            {
                options.UseInMemoryStorage();
                if (Configuration.GetValue<bool>("AzureServiceBusEnabled"))
                {
                    options.UseAzureServiceBus(Configuration["EventBusConnection"]);
                }
                else
                {
                    options.UseRabbitMQ(conf =>
                    {
                        conf.HostName = Configuration["EventBusConnection"];
                        if (!string.IsNullOrEmpty(Configuration["EventBusUserName"]))
                        {
                            conf.UserName = Configuration["EventBusUserName"];
                        }
                        if (!string.IsNullOrEmpty(Configuration["EventBusPassword"]))
                        {
                            conf.Password = Configuration["EventBusPassword"];
                        }
                    });
                }

                if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
                {
                    options.FailedRetryCount = int.Parse(Configuration["EventBusRetryCount"]);
                }

                if (!string.IsNullOrEmpty(Configuration["SubscriptionClientName"]))
                {
                    options.DefaultGroup = Configuration["SubscriptionClientName"];
                }
            });

            ConfigureAuthService(services);

            services.AddOptions();

            //configure autofac
            var container = new ContainerBuilder();
            container.RegisterModule(new ApplicationModule());
            container.Populate(services);

            return new AutofacServiceProvider(container.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();
            //loggerFactory.AddAzureWebAppDiagnostics();
            //loggerFactory.AddApplicationInsights(app.ApplicationServices, LogLevel.Trace);

            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                loggerFactory.CreateLogger<Startup>().LogDebug("Using PATH BASE '{pathBase}'", pathBase);
                app.UsePathBase(pathBase);
            }

            app.UseHealthChecks("/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseHealthChecks("/liveness", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });

            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            app.UseSignalR(routes =>
            {
                routes.MapHub<NotificationsHub>("/notificationhub", options =>
                    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransports.All);
            }); 
        }

        private void ConfigureAuthService(IServiceCollection services)
        {
            // prevent from mapping "sub" claim to nameidentifier.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

            var identityUrl = Configuration.GetValue<string>("IdentityUrl");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.Authority = identityUrl;
                options.RequireHttpsMetadata = false;
                options.Audience = "orders.signalrhub";
            });
        }
    }

    public static class CustomExtensionMethods
    {
        public static IServiceCollection AddIntegrationEventHandler(this IServiceCollection services)
        {
            services.AddTransient<OrderStatusChangedToAwaitingValidationIntegrationEventHandler>(); //Subscribe for OrderStatusChangedToAwaitingValidationIntegrationEvent
            services.AddTransient<OrderStatusChangedToPaidIntegrationEventHandler>();  //Subscribe for OrderStatusChangedToPaidIntegrationEvent
            services.AddTransient<OrderStatusChangedToStockConfirmedIntegrationEventHandler>(); //Subscribe for OrderStatusChangedToStockConfirmedIntegrationEvent
            services.AddTransient<OrderStatusChangedToShippedIntegrationEventHandler>(); //Subscribe for OrderStatusChangedToShippedIntegrationEvent
            services.AddTransient<OrderStatusChangedToCancelledIntegrationEventHandler>(); //Subscribe for OrderStatusChangedToCancelledIntegrationEvent
            services.AddTransient<OrderStatusChangedToSubmittedIntegrationEventHandler>(); //Subscribe for OrderStatusChangedToSubmittedIntegrationEvent
            return services;
        }

        public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
        {
            var hcBuilder = services.AddHealthChecks();

            hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

            if (configuration.GetValue<bool>("AzureServiceBusEnabled"))
            {
                hcBuilder
                    .AddAzureServiceBusTopic(
                        configuration["EventBusConnection"],
                        topicName: "eshop_event_bus",
                        name: "signalr-servicebus-check",
                        tags: new string[] { "servicebus" });
            }
            else
            {
                hcBuilder
                    .AddRabbitMQ(
                        $"amqp://{configuration["EventBusConnection"]}",
                        name: "signalr-rabbitmqbus-check",
                        tags: new string[] { "rabbitmqbus" });
            }

            return services;
        }
    }
}
