var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddGrpc();
builder.Services.AddControllers();

builder.Services.AddHealthChecks(builder.Configuration);
builder.Services.AddDbContexts(builder.Configuration);
builder.Services.AddApplicationOptions(builder.Configuration);
builder.Services.AddIntegrationServices();

var services = builder.Services;

services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining(typeof(Program));

    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
    cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
});

// Register the command validators for the validator behavior (validators based on FluentValidation library)
services.AddSingleton<IValidator<CancelOrderCommand>, CancelOrderCommandValidator>();
services.AddSingleton<IValidator<CreateOrderCommand>, CreateOrderCommandValidator>();
services.AddSingleton<IValidator<IdentifiedCommand<CreateOrderCommand, bool>>, IdentifiedCommandValidator>();
services.AddSingleton<IValidator<ShipOrderCommand>, ShipOrderCommandValidator>();

services.AddScoped<IOrderQueries>(sp => new OrderQueries(builder.Configuration.GetConnectionString("OrderingDB")));
services.AddScoped<IBuyerRepository, BuyerRepository>();
services.AddScoped<IOrderRepository, OrderRepository>();
services.AddScoped<IRequestManager, RequestManager>();

// Add integration event handlers.
services.AddTransient<IIntegrationEventHandler<GracePeriodConfirmedIntegrationEvent>, GracePeriodConfirmedIntegrationEventHandler>();
services.AddTransient<IIntegrationEventHandler<OrderPaymentFailedIntegrationEvent>, OrderPaymentFailedIntegrationEventHandler>();
services.AddTransient<IIntegrationEventHandler<OrderPaymentSucceededIntegrationEvent>, OrderPaymentSucceededIntegrationEventHandler>();
services.AddTransient<IIntegrationEventHandler<OrderStockConfirmedIntegrationEvent>, OrderStockConfirmedIntegrationEventHandler>();
services.AddTransient<IIntegrationEventHandler<OrderStockRejectedIntegrationEvent>, OrderStockRejectedIntegrationEventHandler>();
services.AddTransient<IIntegrationEventHandler<UserCheckoutAcceptedIntegrationEvent>, UserCheckoutAcceptedIntegrationEventHandler>();

var app = builder.Build();

app.UseServiceDefaults();

app.MapGrpcService<OrderingService>();
app.MapControllers();

var eventBus = app.Services.GetRequiredService<IEventBus>();

eventBus.Subscribe<UserCheckoutAcceptedIntegrationEvent, IIntegrationEventHandler<UserCheckoutAcceptedIntegrationEvent>>();
eventBus.Subscribe<GracePeriodConfirmedIntegrationEvent, IIntegrationEventHandler<GracePeriodConfirmedIntegrationEvent>>();
eventBus.Subscribe<OrderStockConfirmedIntegrationEvent, IIntegrationEventHandler<OrderStockConfirmedIntegrationEvent>>();
eventBus.Subscribe<OrderStockRejectedIntegrationEvent, IIntegrationEventHandler<OrderStockRejectedIntegrationEvent>>();
eventBus.Subscribe<OrderPaymentFailedIntegrationEvent, IIntegrationEventHandler<OrderPaymentFailedIntegrationEvent>>();
eventBus.Subscribe<OrderPaymentSucceededIntegrationEvent, IIntegrationEventHandler<OrderPaymentSucceededIntegrationEvent>>();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<OrderingContext>();
    var env = app.Services.GetService<IWebHostEnvironment>();
    var settings = app.Services.GetService<IOptions<OrderingSettings>>();
    var logger = app.Services.GetService<ILogger<OrderingContextSeed>>();
    await context.Database.MigrateAsync();

    await new OrderingContextSeed().SeedAsync(context, env, settings, logger);
    var integEventContext = scope.ServiceProvider.GetRequiredService<IntegrationEventLogContext>();
    await integEventContext.Database.MigrateAsync();
}

await app.RunAsync();
