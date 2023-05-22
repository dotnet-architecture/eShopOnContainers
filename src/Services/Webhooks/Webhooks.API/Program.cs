var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllers();
builder.Services.AddDbContexts(builder.Configuration);
builder.Services.AddHealthChecks(builder.Configuration);
builder.Services.AddHttpClientServices();
builder.Services.AddIntegrationServices();

builder.Services.AddTransient<IIdentityService, IdentityService>();
builder.Services.AddTransient<IGrantUrlTesterService, GrantUrlTesterService>();
builder.Services.AddTransient<IWebhooksRetriever, WebhooksRetriever>();
builder.Services.AddTransient<IWebhooksSender, WebhooksSender>();

builder.Services.AddTransient<ProductPriceChangedIntegrationEventHandler>();
builder.Services.AddTransient<OrderStatusChangedToShippedIntegrationEventHandler>();
builder.Services.AddTransient<OrderStatusChangedToPaidIntegrationEventHandler>();

var app = builder.Build();

app.UseServiceDefaults();

app.MapControllers();

var eventBus = app.Services.GetRequiredService<IEventBus>();

eventBus.Subscribe<ProductPriceChangedIntegrationEvent, ProductPriceChangedIntegrationEventHandler>();
eventBus.Subscribe<OrderStatusChangedToShippedIntegrationEvent, OrderStatusChangedToShippedIntegrationEventHandler>();
eventBus.Subscribe<OrderStatusChangedToPaidIntegrationEvent, OrderStatusChangedToPaidIntegrationEventHandler>();

app.Services.MigrateDbContext<WebhooksContext>((_, __) => { });

await app.RunAsync();
