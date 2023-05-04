var builder = WebApplication.CreateBuilder(args);
if (builder.Configuration.GetValue<bool>("UseVault", false))
{
    TokenCredential credential = new ClientSecretCredential(
        builder.Configuration["Vault:TenantId"],
        builder.Configuration["Vault:ClientId"],
        builder.Configuration["Vault:ClientSecret"]);
    builder.Configuration.AddAzureKeyVault(new Uri($"https://{builder.Configuration["Vault:Name"]}.vault.azure.net/"), credential);
}
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();
builder.WebHost.CaptureStartupErrors(false);
builder.Services
    .AddCustomHealthCheck(builder.Configuration);
builder.Services.Configure<PaymentSettings>(builder.Configuration);
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration);
if (builder.Configuration.GetValue<bool>("AzureServiceBusEnabled"))
{
    builder.Services.AddSingleton<IServiceBusPersisterConnection>(sp =>
    {
        var serviceBusConnectionString = builder.Configuration["EventBusConnection"];
        var subscriptionClientName = builder.Configuration["SubscriptionClientName"];

        return new DefaultServiceBusPersisterConnection(serviceBusConnectionString);
    });
}
else
{
    builder.Services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
    {
        var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();
        var factory = new ConnectionFactory()
        {
            HostName = builder.Configuration["EventBusConnection"],
            DispatchConsumersAsync = true
        };

        if (!string.IsNullOrEmpty(builder.Configuration["EventBusUserName"]))
        {
            factory.UserName = builder.Configuration["EventBusUserName"];
        }

        if (!string.IsNullOrEmpty(builder.Configuration["EventBusPassword"]))
        {
            factory.Password = builder.Configuration["EventBusPassword"];
        }

        var retryCount = 5;
        if (!string.IsNullOrEmpty(builder.Configuration["EventBusRetryCount"]))
        {
            retryCount = int.Parse(builder.Configuration["EventBusRetryCount"]);
        }

        return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
    });
}
RegisterEventBus(builder.Services);
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}
var pathBase = app.Configuration["PATH_BASE"];
if (!string.IsNullOrEmpty(pathBase))
{
    app.UsePathBase(pathBase);
}
ConfigureEventBus(app);

app.UseRouting();
app.MapHealthChecks("/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecks("/liveness", new HealthCheckOptions
{
    Predicate = r => r.Name.Contains("self")
});

await app.RunAsync();

void RegisterEventBus(IServiceCollection services)
{
    if (builder.Configuration.GetValue<bool>("AzureServiceBusEnabled"))
    {
        services.AddSingleton<IEventBus, EventBusServiceBus>(sp =>
        {
            var serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
            var logger = sp.GetRequiredService<ILogger<EventBusServiceBus>>();
            var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
            string subscriptionName = builder.Configuration["SubscriptionClientName"];

            return new EventBusServiceBus(serviceBusPersisterConnection, logger,
                eventBusSubcriptionsManager, sp, subscriptionName);
        });
    }
    else
    {
        services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
        {
            var subscriptionClientName = builder.Configuration["SubscriptionClientName"];
            var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
            var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
            var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

            var retryCount = 5;
            if (!string.IsNullOrEmpty(builder.Configuration["EventBusRetryCount"]))
            {
                retryCount = int.Parse(builder.Configuration["EventBusRetryCount"]);
            }

            return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, sp, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
        });
    }

    services.AddTransient<OrderStatusChangedToStockConfirmedIntegrationEventHandler>();
    services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
}

void ConfigureEventBus(IApplicationBuilder app)
{
    var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
    eventBus.Subscribe<OrderStatusChangedToStockConfirmedIntegrationEvent, OrderStatusChangedToStockConfirmedIntegrationEventHandler>();
}

public static class CustomExtensionMethods
{
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
                    name: "payment-servicebus-check",
                    tags: new string[] { "servicebus" });
        }
        else
        {
            hcBuilder
                .AddRabbitMQ(
                    $"amqp://{configuration["EventBusConnection"]}",
                    name: "payment-rabbitmqbus-check",
                    tags: new string[] { "rabbitmqbus" });
        }

        return services;
    }
}
