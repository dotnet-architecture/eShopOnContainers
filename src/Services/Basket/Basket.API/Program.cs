using Services.Common;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddKeyVault();

builder.Services.AddApplicationInsights(builder.Configuration);

builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(HttpGlobalExceptionFilter));
    options.Filters.Add(typeof(ValidateModelStateFilter));
});

builder.Services.AddDefaultOpenApi(builder.Configuration);

builder.Services.AddDefaultAuthentication(builder.Configuration);

builder.Services.AddDefaultHealthChecks(builder.Configuration);

builder.Host.UseDefaultSerilog(builder.Configuration, AppName);

builder.WebHost.UseDefaultPorts(builder.Configuration);

builder.WebHost.UseFailing(options =>
{
    options.ConfigPath = "/Failing";
    options.NotFilteredPaths.AddRange(new[] { "/hc", "/liveness" });
});

builder.Services.AddEventBus(builder.Configuration);

builder.Services.Configure<BasketSettings>(builder.Configuration);

builder.Services.AddRedis();

builder.Services.AddTransient<ProductPriceChangedIntegrationEventHandler>();
builder.Services.AddTransient<OrderStartedIntegrationEventHandler>();

builder.Services.AddTransient<IBasketRepository, RedisBasketRepository>();
builder.Services.AddTransient<IIdentityService, IdentityService>();

var app = builder.Build();

app.MapGet("hello", () => "hello");

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

var pathBase = app.Configuration["PATH_BASE"];
if (!string.IsNullOrEmpty(pathBase))
{
    app.UsePathBase(pathBase);
}

app.UseDefaultOpenApi(builder.Configuration);

app.MapGrpcService<BasketService>();
app.MapControllers();

app.MapDefaultHealthChecks();

var eventBus = app.Services.GetRequiredService<IEventBus>();

eventBus.Subscribe<ProductPriceChangedIntegrationEvent, ProductPriceChangedIntegrationEventHandler>();
eventBus.Subscribe<OrderStartedIntegrationEvent, OrderStartedIntegrationEventHandler>();

try
{
    Log.Information("Configuring web host ({ApplicationContext})...", AppName);


    Log.Information("Starting web host ({ApplicationContext})...", AppName);
    await app.RunAsync();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program
{
    private static string Namespace = typeof(Program).Assembly.GetName().Name;
    public static string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
}
