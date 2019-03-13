using Autofac;
using Autofac.Extensions.DependencyInjection;
using global::Catalog.API.Infrastructure.Filters;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.ServiceFabric;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure;
using Microsoft.eShopOnContainers.Services.Catalog.API.IntegrationEvents.EventHandling;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Microsoft.eShopOnContainers.Services.Catalog.API
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
            services.AddAppInsight(Configuration)
                .AddCustomMVC(Configuration)
                .AddCustomDbContext(Configuration)
                .AddCustomOptions(Configuration)
                .AddIntegrationServices()
                .AddEventBus(Configuration)
                .AddSwagger()
                .AddCustomHealthCheck(Configuration);

            var container = new ContainerBuilder();
            container.Populate(services);
            return new AutofacServiceProvider(container.Build());

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //Configure logs

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

            app.UseMvcWithDefaultRoute();

            app.UseSwagger()
              .UseSwaggerUI(c =>
              {
                  c.SwaggerEndpoint($"{ (!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty) }/swagger/v1/swagger.json", "Catalog.API V1");
              });
        }
    }

    public static class CustomExtensionMethods
    {
        public static IServiceCollection AddAppInsight(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationInsightsTelemetry(configuration);
            var orchestratorType = configuration.GetValue<string>("OrchestratorType");

            if (orchestratorType?.ToUpper() == "K8S")
            {
                // Enable K8s telemetry initializer
                services.AddApplicationInsightsKubernetesEnricher();
            }
            if (orchestratorType?.ToUpper() == "SF")
            {
                // Enable SF telemetry initializer
                services.AddSingleton<ITelemetryInitializer>((serviceProvider) =>
                    new FabricTelemetryInitializer());
            }

            return services;
        }

        public static IServiceCollection AddCustomMVC(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMvc(options =>
                {
                    options.Filters.Add(typeof(HttpGlobalExceptionFilter));
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddControllersAsServices();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .SetIsOriginAllowed((host) => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            return services;
        }

        public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
        {
            var accountName = configuration.GetValue<string>("AzureStorageAccountName");
            var accountKey = configuration.GetValue<string>("AzureStorageAccountKey");

            var hcBuilder = services.AddHealthChecks();

            hcBuilder
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddSqlServer(
                    configuration["ConnectionString"],
                    name: "CatalogDB-check",
                    tags: new string[] { "catalogdb" });

            if (!string.IsNullOrEmpty(accountName) && !string.IsNullOrEmpty(accountKey))
            {
                hcBuilder
                    .AddAzureBlobStorage(
                        $"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={accountKey};EndpointSuffix=core.windows.net",
                        name: "catalog-storage-check",
                        tags: new string[] { "catalogstorage" });
            }

            if (configuration.GetValue<bool>("AzureServiceBusEnabled"))
            {
                hcBuilder
                    .AddAzureServiceBusTopic(
                        configuration["EventBusConnection"],
                        topicName: "eshop_event_bus",
                        name: "catalog-servicebus-check",
                        tags: new string[] { "servicebus" });
            }
            else
            {
                hcBuilder
                    .AddRabbitMQ(
                        $"amqp://{configuration["EventBusConnection"]}",
                        name: "catalog-rabbitmqbus-check",
                        tags: new string[] { "rabbitmqbus" });
            }

            return services;
        }

        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CatalogContext>(options =>
            {
                options.UseSqlServer(configuration["ConnectionString"],
                                     sqlServerOptionsAction: sqlOptions =>
                                     {
                                         sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                                         //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                                         sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                     });

                // Changing default behavior when client evaluation occurs to throw. 
                // Default in EF Core would be to log a warning when client evaluation is performed.
                options.ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning));
                //Check Client vs. Server evaluation: https://docs.microsoft.com/en-us/ef/core/querying/client-eval
            });

            return services;
        }

        public static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CatalogSettings>(configuration);
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Instance = context.HttpContext.Request.Path,
                        Status = StatusCodes.Status400BadRequest,
                        Detail = "Please refer to the errors property for additional details."
                    };

                    return new BadRequestObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json", "application/problem+xml" }
                    };
                };
            });

            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Title = "eShopOnContainers - Catalog HTTP API",
                    Version = "v1",
                    Description = "The Catalog Microservice HTTP API. This is a Data-Driven/CRUD microservice sample",
                    TermsOfService = "Terms Of Service"
                });
            });

            return services;

        }

        public static IServiceCollection AddIntegrationServices(this IServiceCollection services)
        {
            //Handlers
            services.AddTransient<OrderStatusChangedToAwaitingValidationIntegrationEventHandler>();  // OrderStatusChangedToAwaitingValidationIntegrationEvent
            services.AddTransient<OrderStatusChangedToPaidIntegrationEventHandler>(); // OrderStatusChangedToPaidIntegrationEvent

            return services;
        }

        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCap(options =>
            {
                options.UseSqlServer(configuration["ConnectionString"]);

                if (configuration.GetValue<bool>("AzureServiceBusEnabled"))
                {
                    options.UseAzureServiceBus(configuration["EventBusConnection"]);
                }
                else
                {
                    options.UseRabbitMQ(conf =>
                    {
                        conf.HostName = configuration["EventBusConnection"];
                        if (!string.IsNullOrEmpty(configuration["EventBusUserName"]))
                        {
                            conf.UserName = configuration["EventBusUserName"];
                        }
                        if (!string.IsNullOrEmpty(configuration["EventBusPassword"]))
                        {
                            conf.Password = configuration["EventBusPassword"];
                        }
                    });
                }

                if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
                {
                    options.FailedRetryCount = int.Parse(configuration["EventBusRetryCount"]);
                }

                if (!string.IsNullOrEmpty(configuration["SubscriptionClientName"]))
                {
                    options.DefaultGroup = configuration["SubscriptionClientName"];
                }
            });

            return services;
        }
    }
}
