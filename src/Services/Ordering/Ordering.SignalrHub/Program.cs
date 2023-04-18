var appName = "Ordering.SignalrHub";
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ApplicationName = typeof(Program).Assembly.FullName,
    ContentRootPath = Directory.GetCurrentDirectory()
});
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();
builder.WebHost.CaptureStartupErrors(false);
builder.Host.UseSerilog(CreateSerilogLogger(builder.Configuration));

builder.Services
            .AddCustomHealthCheck(builder.Configuration)
            .AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed((host) => true)
                    .AllowCredentials());
            });
if (builder.Configuration.GetValue<string>("IsClusterEnv") == bool.TrueString)
{
    builder.Services
        .AddSignalR()
        .AddStackExchangeRedis(builder.Configuration["SignalrStoreConnectionString"]);
}
else
{
    builder.Services.AddSignalR();
}

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
ConfigureAuthService(builder.Services, builder.Configuration);
RegisterEventBus(builder.Services, builder.Configuration);
builder.Services.AddOptions();
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(conBuilder => conBuilder.RegisterModule(new ApplicationModule()));
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}
var pathBase = builder.Configuration["PATH_BASE"];
if (!string.IsNullOrEmpty(pathBase))
{
    app.UsePathBase(pathBase);
}
app.UseRouting();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecks("/liveness", new HealthCheckOptions
{
    Predicate = r => r.Name.Contains("self")
});
app.MapHub<NotificationsHub>("/hub/notificationhub");

ConfigureEventBus(app);
try
{
    Log.Information("Starting web host ({ApplicationContext})...", Program.AppName);
    await app.RunAsync();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", Program.AppName);
    return 1;
}
finally
{
    Log.CloseAndFlush();
}
void ConfigureEventBus(IApplicationBuilder app)
{
    var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

    eventBus.Subscribe<OrderStatusChangedToAwaitingValidationIntegrationEvent, OrderStatusChangedToAwaitingValidationIntegrationEventHandler>();
    eventBus.Subscribe<OrderStatusChangedToPaidIntegrationEvent, OrderStatusChangedToPaidIntegrationEventHandler>();
    eventBus.Subscribe<OrderStatusChangedToStockConfirmedIntegrationEvent, OrderStatusChangedToStockConfirmedIntegrationEventHandler>();
    eventBus.Subscribe<OrderStatusChangedToShippedIntegrationEvent, OrderStatusChangedToShippedIntegrationEventHandler>();
    eventBus.Subscribe<OrderStatusChangedToCancelledIntegrationEvent, OrderStatusChangedToCancelledIntegrationEventHandler>();
    eventBus.Subscribe<OrderStatusChangedToSubmittedIntegrationEvent, OrderStatusChangedToSubmittedIntegrationEventHandler>();
}
void ConfigureAuthService(IServiceCollection services, IConfiguration configuration)
{
    // prevent from mapping "sub" claim to nameidentifier.
    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

    var identityUrl = configuration.GetValue<string>("IdentityUrl");

    services.AddAuthentication("Bearer").AddJwtBearer(options =>
    {
        options.Authority = identityUrl;
        options.RequireHttpsMetadata = false;
        options.Audience = "orders.signalrhub";
        options.TokenValidationParameters.ValidateAudience = false;
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/hub/notificationhub")))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });
    services.AddAuthorization(options =>
    {
        options.AddPolicy("ApiScope", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim("scope", "orders.signalrhub");
        });
    });
}
void RegisterEventBus(IServiceCollection services, IConfiguration configuration)
{
    if (configuration.GetValue<bool>("AzureServiceBusEnabled"))
    {
        services.AddSingleton<IEventBus, EventBusServiceBus>(sp =>
        {
            var serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
            var logger = sp.GetRequiredService<ILogger<EventBusServiceBus>>();
            var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
            string subscriptionName = configuration["SubscriptionClientName"];

            return new EventBusServiceBus(serviceBusPersisterConnection, logger,
                eventBusSubcriptionsManager, sp, subscriptionName);
        });
    }
    else
    {
        services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
        {
            var subscriptionClientName = configuration["SubscriptionClientName"];
            var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
            var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
            var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

            var retryCount = 5;
            if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
            {
                retryCount = int.Parse(configuration["EventBusRetryCount"]);
            }

            return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, sp, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
        });
    }

    services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
}
static Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
{
    var seqServerUrl = configuration["Serilog:SeqServerUrl"];
    var logstashUrl = configuration["Serilog:LogstashgUrl"];
    return new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .Enrich.WithProperty("ApplicationContext", Program.AppName)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.Seq(string.IsNullOrWhiteSpace(seqServerUrl) ? "http://seq" : seqServerUrl)
        .WriteTo.Http(string.IsNullOrWhiteSpace(logstashUrl) ? "http://logstash:8080" : logstashUrl, null)
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}
public partial class Program
{
    public static string Namespace = typeof(Program).Assembly.GetName().Name;
    public static string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
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
