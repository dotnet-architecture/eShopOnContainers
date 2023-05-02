using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Azure.Identity;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
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
        builder.Host.UseDefaultSerilog(builder.Configuration, builder.Environment.ApplicationName);

        // Configure the default ports for this service (http and grpc ports read from configuration)
        builder.WebHost.UseDefaultPorts(builder.Configuration);

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
        app.UseSwagger();
        app.UseSwaggerUI(setup =>
        {
            var pathBase = configuration["PATH_BASE"];
            var openApiSection = configuration.GetRequiredSection("OpenApi");
            var authSection = openApiSection.GetRequiredSection("Auth");
            var endpointSection = openApiSection.GetRequiredSection("Endpoint");

            var swaggerUrl = endpointSection["Url"] ?? $"{(!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty)}/swagger/v1/swagger.json";

            setup.SwaggerEndpoint(swaggerUrl, endpointSection.GetRequiredValue("Name"));
            setup.OAuthClientId(authSection.GetRequiredValue("ClientId"));
            setup.OAuthAppName(authSection.GetRequiredValue("AppName"));
        });

        return app;
    }

    public static IServiceCollection AddDefaultOpenApi(this IServiceCollection services, IConfiguration configuration) =>
        services.AddSwaggerGen(options =>
        {
            var openApi = configuration.GetRequiredSection("OpenApi");

            /// {
            ///   "OpenApi": {
            ///     "Endpoint: {
            ///         "Name": 
            ///     },
            ///     "Document": {
            ///         "Title": ..
            ///         "Version": ..
            ///         "Description": ..
            ///     },
            ///     "Auth": {
            ///         "ClientId": ..,
            ///         "AppName": ..
            ///     }
            ///   }
            /// }

            var version = openApi.GetRequiredValue("Version") ?? "v1";

            options.SwaggerDoc(version, new OpenApiInfo
            {
                Title = openApi.GetRequiredValue("Title"),
                Version = version,
                Description = openApi.GetRequiredValue("Description")
            });

            var identityUrlExternal = configuration.GetRequiredValue("IdentityUrlExternal");
            var scopes = openApi.GetRequiredSection("Scopes").AsEnumerable().ToDictionary(p => p.Key, p => p.Value);

            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows()
                {
                    Implicit = new OpenApiOAuthFlow()
                    {
                        AuthorizationUrl = new Uri($"{identityUrlExternal}/connect/authorize"),
                        TokenUrl = new Uri($"{identityUrlExternal}/connect/token"),
                        Scopes = openApi.GetRequiredSection("Scopes").AsEnumerable().ToDictionary(p => p.Key, p => p.Value),
                    }
                }
            });

            options.OperationFilter<AuthorizeCheckOperationFilter>();
        });

    public static IServiceCollection AddDefaultAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // prevent from mapping "sub" claim to nameidentifier.
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

        services.AddAuthentication().AddJwtBearer(options =>
        {
            var identityUrl = configuration.GetRequiredValue("IdentityUrl");
            var audience = configuration.GetRequiredValue("Audience");

            options.Authority = identityUrl;
            options.RequireHttpsMetadata = false;
            options.Audience = audience;
            options.TokenValidationParameters.ValidateAudience = false;
        });

        services.AddAuthorization(options =>
        {
            var scope = configuration.GetRequiredValue("Scope");

            options.AddPolicy("ApiScope", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", scope);
            });
        });

        return services;
    }

    public static IWebHostBuilder UseDefaultPorts(this IWebHostBuilder builder, IConfiguration configuration)
    {
        builder.UseKestrel(options =>
        {
            var (httpPort, grpcPort) = GetDefinedPorts(configuration);

            options.Listen(IPAddress.Any, httpPort, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
            });

            options.Listen(IPAddress.Any, grpcPort, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http2;
            });
        });

        return builder;
    }

    public static ConfigurationManager AddKeyVault(this ConfigurationManager configuration)
    {
        if (configuration.GetValue("UseVault", false))
        {
            // {
            //   "Vault": {
            //     "Name": "myvault",
            //     "TenantId": "mytenantid",
            //     "ClientId": "myclientid",
            //    }
            // }

            var vaultSection = configuration.GetRequiredSection("Vault");

            var credential = new ClientSecretCredential(
                vaultSection.GetRequiredValue("TenantId"),
                vaultSection.GetRequiredValue("ClientId"),
                vaultSection.GetRequiredValue("ClientSecret"));

            var name = vaultSection.GetRequiredValue("Name");

            configuration.AddAzureKeyVault(new Uri($"https://{name}.vault.azure.net/"), credential);
        }

        return configuration;
    }

    public static IServiceCollection AddApplicationInsights(this IServiceCollection services, IConfiguration configuration)
    {
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
        //     "ConnectionString": "Endpoint=sb://eshop-eventbus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=..."
        //   }
        // }

        var eventBusSection = configuration.GetRequiredSection("EventBus");
        var eventBusConnectionString = eventBusSection.GetRequiredValue("ConnectionString");

        return eventBusSection.GetRequiredValue("ProviderName").ToLowerInvariant() switch
        {
            "servicebus" => hcBuilder.AddAzureServiceBusTopic(
                    eventBusConnectionString,
                    topicName: "eshop_event_bus",
                    name: "servicebus-check",
                    tags: new string[] { "servicebus" }),

            _ => hcBuilder.AddRabbitMQ(
                    $"amqp://{eventBusConnectionString}",
                    name: "rabbitmqbus-check",
                    tags: new string[] { "rabbitmqbus" })
        };
    }

    public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        // {
        //   "EventBus": {
        //     "ProviderName": "ServiceBus | RabbitMQ",
        //     "ConnectionString": "...",
        //     ...
        //   }
        // }

        // {
        //   "EventBus": {
        //     "ProviderName": "ServiceBus",
        //     "ConnectionString": "Endpoint=sb://eshop-eventbus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=..."
        //     "SubscriptionClientName": "eshop_event_bus"
        //   }
        // }

        // {
        //   "EventBus": {
        //     "ProviderName": "RabbitMQ",
        //     "ConnectionString": "...",
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
                var serviceBusConnectionString = eventBusSection.GetRequiredValue("ConnectionString");

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
                    HostName = eventBusSection.GetRequiredValue("ConnectionString"),
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
            return new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.WithProperty("ApplicationContext", name)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq(string.IsNullOrWhiteSpace(seqServerUrl) ? "http://seq" : seqServerUrl)
                .WriteTo.Http(string.IsNullOrWhiteSpace(logstashUrl) ? "http://logstash:8080" : logstashUrl, null)
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
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

    static (int httpPort, int grpcPort) GetDefinedPorts(IConfiguration config)
    {
        var grpcPort = config.GetValue("GRPC_PORT", 5001);
        var port = config.GetValue("PORT", 80);
        return (port, grpcPort);
    }

    private static string GetRequiredValue(this IConfiguration configuration, string name) =>
        configuration[name] ?? throw new InvalidOperationException($"Configuration missing value for: {(configuration is IConfigurationSection s ? s.Key + ":" + name : name)}");
}
