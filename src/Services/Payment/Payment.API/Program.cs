var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.Configure<PaymentSettings>(builder.Configuration);

builder.Services.AddTransient<OrderStatusChangedToStockConfirmedIntegrationEventHandler>();

var app = builder.Build();

app.UseServiceDefaults();

var eventBus = app.Services.GetRequiredService<IEventBus>();

eventBus.Subscribe<OrderStatusChangedToStockConfirmedIntegrationEvent, OrderStatusChangedToStockConfirmedIntegrationEventHandler>();

await app.RunAsync();
