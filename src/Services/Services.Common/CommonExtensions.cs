using System.IdentityModel.Tokens.Jwt;
using Azure.Identity;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBusRabbitMQ;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBusServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using Serilog;

namespace Services.Common;

public static class CommonExtensions
{
    public static WebApplicationBuilder AddServiceDefaults(this WebApplicationBuilder builder)
    {
        // Shared configuration via key vault
        builder.Configuration.AddKeyVault();

        // Shared app insights configuration
        builder.Services.AddApplicationInsights(builder.Configuration);

        // Default health checks assume the event bus and self health checks
        builder.Services.AddDefaultHealthChecks(builder.Configuration);

        // Configure the default logging for this application
        // builder.Host.UseDefaultSerilog(builder.Configuration, builder.Environment.ApplicationName);

        // Customizations for this application

        // Add the event bus
        builder.Services.AddEventBus(builder.Configuration);

        builder.Services.AddDefaultAuthentication(builder.Configuration);

        builder.Services.AddDefaultOpenApi(builder.Configuration);

        // Add the accessor
        builder.Services.AddHttpContextAccessor();

        return builder;
    }

    public static WebApplication UseServiceDefaults(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
        }

        var pathBase = app.Configuration["PATH_BASE"];

        if (!string.IsNullOrEmpty(pathBase))
        {
            app.UsePathBase(pathBase);
        }

        app.UseDefaultOpenApi(app.Configuration);

        app.MapDefaultHealthChecks();

        return app;
    }

    public static IApplicationBuilder UseDefaultOpenApi(this IApplicationBuilder app, IConfiguration configuration)
    {
        var openApiSection = configuration.GetSection("OpenApi");

        if (!openApiSection.Exists())
        {
            return app;
        }

        app.UseSwagger();
        app.UseSwaggerUI(setup =>
        {
            /// {
            ///   "OpenApi": {
            ///     "Endpoint: {
            ///         "Name": 
            ///     },
            ///     "Auth": {
            ///         "ClientId": ..,
            ///         "AppName": ..
            ///     }
            ///   }
            /// }

            var pathBase = configuration["PATH_BASE"];
            var authSection = openApiSection.GetSection("Auth");
            var endpointSection = openApiSection.GetRequiredSection("Endpoint");

            var swaggerUrl = endpointSection["Url"] ?? $"{(!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty)}/swagger/v1/swagger.json";

            setup.SwaggerEndpoint(swaggerUrl, endpointSection.GetRequiredValue("Name"));

            if (authSection.Exists())
            {
                setup.OAuthClientId(authSection.GetRequiredValue("ClientId"));
                setup.OAuthAppName(authSection.GetRequiredValue("AppName"));
            }
        });

        return app;
    }

    public static IServiceCollection AddDefaultOpenApi(this IServiceCollection services, IConfiguration configuration)
    {
        var openApi = configuration.GetSection("OpenApi");

        if (!openApi.Exists())
        {
            return services;
        }

        return services.AddSwaggerGen(options =>
        {
            /// {
            ///   "OpenApi": {
            ///     "Document": {
            ///         "Title": ..
            ///         "Version": ..
            ///         "Description": ..
            ///     }
            ///   }
            /// }
            var document = openApi.GetRequiredSection("Document");

            var version = document.GetRequiredValue("Version") ?? "v1";

            options.SwaggerDoc(version, new OpenApiInfo
            {
                Title = document.GetRequiredValue("Title"),
                Version = version,
                Description = document.GetRequiredValue("Description")
            });

            var identitySection = configuration.GetSection("Identity");

            if (!identitySection.Exists())
            {
                // No identity section, so no authentication open api definition
                return;
            }

            // {
            //   "Identity": {
            //     "ExternalUrl": "http://identity",
            //     "Scopes": {
            //         "basket": "Basket API"
            //      }
            //    }
            // }

            var identityUrlExternal = identitySection.GetRequiredValue("ExternalUrl");
            var scopes = identitySection.GetRequiredSection("Scopes").GetChildren().ToDictionary(p => p.Key, p => p.Value);

            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows()
                {
                    Implicit = new OpenApiOAuthFlow()
                    {
                        AuthorizationUrl = new Uri($"{identityUrlExternal}/connect/authorize"),
                        TokenUrl = new Uri($"{identityUrlExternal}/connect/token"),
                        Scopes = scopes,
                    }
                }
            });

            options.OperationFilter<AuthorizeCheckOperationFilter>();
        });
    }

    public static IServiceCollection AddDefaultAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // {
        //   "Identity": {
        //     "Url": "http://identity",
        //     "Audience": "basket"
        //    }
        // }

        var identitySection = configuration.GetSection("Identity");

        if (!identitySection.Exists())
        {
            // No identity section, so no authentication
            return services;
        }

        // prevent from mapping "sub" claim to nameidentifier.
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

        services.AddAuthentication().AddJwtBearer(options =>
        {
            var identityUrl = identitySection.GetRequiredValue("Url");
            var audience = identitySection.GetRequiredValue("Audience");

            options.Authority = identityUrl;
            options.RequireHttpsMetadata = false;
            options.Audience = audience;
            options.TokenValidationParameters.ValidateAudience = false;
        });

        return services;
    }

    public static ConfigurationManager AddKeyVault(this ConfigurationManager configuration)
    {
        // {
        //   "Vault": {
        //     "Name": "myvault",
        //     "TenantId": "mytenantid",
        //     "ClientId": "myclientid",
        //    }
        // }

        var vaultSection = configuration.GetSection("Vault");

        if (!vaultSection.Exists())
        {
            return configuration;
        }

        var credential = new ClientSecretCredential(
            vaultSection.GetRequiredValue("TenantId"),
            vaultSection.GetRequiredValue("ClientId"),
            vaultSection.GetRequiredValue("ClientSecret"));

        var name = vaultSection.GetRequiredValue("Name");

        configuration.AddAzureKeyVault(new Uri($"https://{name}.vault.azure.net/"), credential);

        return configuration;
    }

    public static IServiceCollection AddApplicationInsights(this IServiceCollection services, IConfiguration configuration)
    {
        var appInsightsSection = configuration.GetSection("ApplicationInsights");

        // No instrumentation key, so no application insights
        if (string.IsNullOrEmpty(appInsightsSection["InstrumentationKey"]))
        {
            return services;
        }

        services.AddApplicationInsightsTelemetry(configuration);
        services.AddApplicationInsightsKubernetesEnricher();
        return services;
    }

    public static IHealthChecksBuilder AddDefaultHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var hcBuilder = services.AddHealthChecks();

        // Health check for the application itself
        hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

        // {
        //   "EventBus": {
        //     "ProviderName": "ServiceBus | RabbitMQ",

        //   }
        // }

        var eventBusSection = configuration.GetRequiredSection("EventBus");
        var eventBusConnectionString = configuration.GetRequiredConnectionString("EventBus");

        return eventBusSection["ProviderName"]?.ToLowerInvariant() switch
        {
            "servicebus" => hcBuilder.AddAzureServiceBusTopic(
                    eventBusConnectionString,
                    topicName: "eshop_event_bus",
                    name: "servicebus",
                    tags: new string[] { "ready" }),

            _ => hcBuilder.AddRabbitMQ(
                    $"amqp://{eventBusConnectionString}",
                    name: "rabbitmq",
                    tags: new string[] { "ready" })
        };
    }

    public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        //  {
        //    "ConnectionStrings": {
        //      "EventBus": "..."
        //    },

        // {
        //   "EventBus": {
        //     "ProviderName": "ServiceBus | RabbitMQ",
        //     ...
        //   }
        // }

        // {
        //   "EventBus": {
        //     "ProviderName": "ServiceBus",
        //     "SubscriptionClientName": "eshop_event_bus"
        //   }
        // }

        // {
        //   "EventBus": {
        //     "ProviderName": "RabbitMQ",
        //     "SubscriptionClientName": "...",
        //     "UserName": "...",
        //     "Password": "...",
        //     "RetryCount": 1
        //   }
        // }

        var eventBusSection = configuration.GetRequiredSection("EventBus");
        if (string.Equals(eventBusSection["ProviderName"], "ServiceBus", StringComparison.OrdinalIgnoreCase))
        {
            services.AddSingleton<IServiceBusPersisterConnection>(sp =>
            {
                var serviceBusConnectionString = configuration.GetRequiredConnectionString("EventBus");

                return new DefaultServiceBusPersisterConnection(serviceBusConnectionString);
            });

            services.AddSingleton<IEventBus, EventBusServiceBus>(sp =>
            {
                var serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
                var logger = sp.GetRequiredService<ILogger<EventBusServiceBus>>();
                var eventBusSubscriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
                string subscriptionName = eventBusSection.GetRequiredValue("SubscriptionClientName");

                return new EventBusServiceBus(serviceBusPersisterConnection, logger,
                    eventBusSubscriptionsManager, sp, subscriptionName);
            });
        }
        else
        {
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = configuration.GetRequiredConnectionString("EventBus"),
                    DispatchConsumersAsync = true
                };

                if (!string.IsNullOrEmpty(eventBusSection["UserName"]))
                {
                    factory.UserName = eventBusSection["UserName"];
                }

                if (!string.IsNullOrEmpty(eventBusSection["Password"]))
                {
                    factory.Password = eventBusSection["Password"];
                }

                var retryCount = eventBusSection.GetValue("RetryCount", 5);

                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });

            services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
            {
                var subscriptionClientName = eventBusSection.GetRequiredValue("SubscriptionClientName");
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                var eventBusSubscriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
                var retryCount = eventBusSection.GetValue("RetryCount", 5);

                return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, sp, eventBusSubscriptionsManager, subscriptionClientName, retryCount);
            });
        }

        services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
        return services;
    }

    public static void UseDefaultSerilog(this IHostBuilder builder, IConfiguration configuration, string name)
    {
        builder.UseSerilog(CreateSerilogLogger(configuration));

        Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            var seqServerUrl = configuration["Serilog:SeqServerUrl"];
            var logstashUrl = configuration["Serilog:LogstashgUrl"];

            var loggingConfiguration = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.WithProperty("ApplicationContext", name)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .ReadFrom.Configuration(configuration);

            if (!string.IsNullOrEmpty(seqServerUrl))
            {
                loggingConfiguration.WriteTo.Seq(seqServerUrl);
            }

            if (!string.IsNullOrEmpty(logstashUrl))
            {
                loggingConfiguration.WriteTo.Http(logstashUrl, null);
            }

            return loggingConfiguration.CreateLogger();
        }
    }

    public static void MapDefaultHealthChecks(this IEndpointRouteBuilder routes)
    {
        routes.MapHealthChecks("/hc", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        routes.MapHealthChecks("/liveness", new HealthCheckOptions
        {
            Predicate = r => r.Name.Contains("self")
        });
    }

    private static string GetRequiredValue(this IConfiguration configuration, string name) =>
        configuration[name] ?? throw new InvalidOperationException($"Configuration missing value for: {(configuration is IConfigurationSection s ? s.Path + ":" + name : name)}");

    private static string GetRequiredConnectionString(this IConfiguration configuration, string name) =>
        configuration.GetConnectionString(name) ?? throw new InvalidOperationException($"Configuration missing value for: {(configuration is IConfigurationSection s ? s.Path + ":ConnectionStrings:" + name : "ConnectionStrings:" + name)}");
}
