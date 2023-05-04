using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(HttpGlobalExceptionFilter));
    options.Filters.Add(typeof(ValidateModelStateFilter));
});

builder.Services.AddRedis(builder.Configuration);

builder.Services.AddTransient<ProductPriceChangedIntegrationEventHandler>();
builder.Services.AddTransient<OrderStartedIntegrationEventHandler>();

builder.Services.AddTransient<IBasketRepository, RedisBasketRepository>();
builder.Services.AddTransient<IIdentityService, IdentityService>();

var app = builder.Build();

try
{
    app.Logger.LogInformation("Running health checks...");

    // Do a health check on startup, this will throw an exception if any of the checks fail
    var report = await app.Services.GetRequiredService<HealthCheckService>().CheckHealthAsync();

    if (report.Status == HealthStatus.Unhealthy)
    {
        app.Logger.LogCritical("Health checks failed!");
        foreach (var entry in report.Entries)
        {
            if (entry.Value.Status == HealthStatus.Unhealthy)
            {
                app.Logger.LogCritical("{Check}: {Status}", entry.Key, entry.Value.Status);
            }
        }
        return 1;
    }

    app.UseServiceDefaults();

    app.MapGet("/", () => Results.Redirect("/swagger"));

    app.MapGrpcService<BasketService>();
    app.MapControllers();

    var eventBus = app.Services.GetRequiredService<IEventBus>();

    eventBus.Subscribe<ProductPriceChangedIntegrationEvent, ProductPriceChangedIntegrationEventHandler>();
    eventBus.Subscribe<OrderStartedIntegrationEvent, OrderStartedIntegrationEventHandler>();

    await app.RunAsync();

    return 0;
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);
    return 1;
}

public partial class Program
{
    private static string Namespace = typeof(Program).Assembly.GetName().Name;
    public static string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
}
