var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddGrpc();
builder.Services.AddControllers();
builder.Services.AddProblemDetails();

builder.Services.AddRedis(builder.Configuration);

builder.Services.AddTransient<ProductPriceChangedIntegrationEventHandler>();
builder.Services.AddTransient<OrderStartedIntegrationEventHandler>();

builder.Services.AddTransient<IBasketRepository, RedisBasketRepository>();
builder.Services.AddTransient<IIdentityService, IdentityService>();

var app = builder.Build();

try
{
    await app.CheckHealthAsync();

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
    app.Logger.LogCritical(ex, "Program terminated unexpectedly");
    return 1;
}
