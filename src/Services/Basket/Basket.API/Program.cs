using Autofac.Core;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Extensions.Configuration;

var appName = "Basket.API";
var builder = WebApplication.CreateBuilder(new WebApplicationOptions {
    Args = args,
    ApplicationName = typeof(Program).Assembly.FullName,
    ContentRootPath = Directory.GetCurrentDirectory()
});
if (builder.Configuration.GetValue<bool>("UseVault", false)) {
    TokenCredential credential = new ClientSecretCredential(
           builder.Configuration["Vault:TenantId"],
        builder.Configuration["Vault:ClientId"],
        builder.Configuration["Vault:ClientSecret"]);
    builder.Configuration.AddAzureKeyVault(new Uri($"https://{builder.Configuration["Vault:Name"]}.vault.azure.net/"), credential);
}

builder.Services.AddGrpc(options => {
    options.EnableDetailedErrors = true;
});
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration);
builder.Services.AddApplicationInsightsKubernetesEnricher();
builder.Services.AddControllers(options => {
    options.Filters.Add(typeof(HttpGlobalExceptionFilter));
    options.Filters.Add(typeof(ValidateModelStateFilter));

}) // Added for functional tests
            .AddApplicationPart(typeof(BasketController).Assembly)
            .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);
builder.Services.AddSwaggerGen(options => {
    options.SwaggerDoc("v1", new OpenApiInfo {
        Title = "eShopOnContainers - Basket HTTP API",
        Version = "v1",
        Description = "The Basket Service HTTP API"
    });

    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows() {
            Implicit = new OpenApiOAuthFlow() {
                AuthorizationUrl = new Uri($"{builder.Configuration.GetValue<string>("IdentityUrlExternal")}/connect/authorize"),
                TokenUrl = new Uri($"{builder.Configuration.GetValue<string>("IdentityUrlExternal")}/connect/token"),
                Scopes = new Dictionary<string, string>() { { "basket", "Basket API" } }
            }
        }
    });

    options.OperationFilter<AuthorizeCheckOperationFilter>();
});

// prevent from mapping "sub" claim to nameidentifier.
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

var identityUrl = builder.Configuration.GetValue<string>("IdentityUrl");

builder.Services.AddAuthentication("Bearer").AddJwtBearer(options => {
    options.Authority = identityUrl;
    options.RequireHttpsMetadata = false;
    options.Audience = "basket";
    options.TokenValidationParameters.ValidateAudience = false;
});
builder.Services.AddAuthorization(options => {
    options.AddPolicy("ApiScope", policy => {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "basket");
    });
});

builder.Services.AddCustomHealthCheck(builder.Configuration);

builder.Services.Configure<BasketSettings>(builder.Configuration);

builder.Services.AddSingleton<ConnectionMultiplexer>(sp => {
    var settings = sp.GetRequiredService<IOptions<BasketSettings>>().Value;
    var configuration = ConfigurationOptions.Parse(settings.ConnectionString, true);

    return ConnectionMultiplexer.Connect(configuration);
});


if (builder.Configuration.GetValue<bool>("AzureServiceBusEnabled")) {
    builder.Services.AddSingleton<IServiceBusPersisterConnection>(sp => {
        var serviceBusConnectionString = builder.Configuration["EventBusConnection"];

        return new DefaultServiceBusPersisterConnection(serviceBusConnectionString);
    });
}
else {
    builder.Services.AddSingleton<IRabbitMQPersistentConnection>(sp => {
        var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

        var factory = new ConnectionFactory() {
            HostName = builder.Configuration["EventBusConnection"],
            DispatchConsumersAsync = true
        };

        if (!string.IsNullOrEmpty(builder.Configuration["EventBusUserName"])) {
            factory.UserName = builder.Configuration["EventBusUserName"];
        }

        if (!string.IsNullOrEmpty(builder.Configuration["EventBusPassword"])) {
            factory.Password = builder.Configuration["EventBusPassword"];
        }

        var retryCount = 5;
        if (!string.IsNullOrEmpty(builder.Configuration["EventBusRetryCount"])) {
            retryCount = int.Parse(builder.Configuration["EventBusRetryCount"]);
        }

        return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
    });
}
builder.Services.RegisterEventBus(builder.Configuration);
builder.Services.AddCors(options => {
    options.AddPolicy("CorsPolicy",
        builder => builder
        .SetIsOriginAllowed((host) => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<IBasketRepository, RedisBasketRepository>();
builder.Services.AddTransient<IIdentityService, IdentityService>();

builder.Services.AddOptions();
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();
builder.WebHost.UseKestrel(options => {
    var ports = GetDefinedPorts(builder.Configuration);
    options.Listen(IPAddress.Any, ports.httpPort, listenOptions => {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });

    options.Listen(IPAddress.Any, ports.grpcPort, listenOptions => {
        listenOptions.Protocols = HttpProtocols.Http2;
    });

});
builder.WebHost.CaptureStartupErrors(false);
builder.Host.UseSerilog(CreateSerilogLogger(builder.Configuration));
builder.WebHost.UseFailing(options => {
    options.ConfigPath = "/Failing";
    options.NotFilteredPaths.AddRange(new[] { "/hc", "/liveness" });
});
var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}
else {
    app.UseExceptionHandler("/Home/Error");
}

var pathBase = app.Configuration["PATH_BASE"];
if (!string.IsNullOrEmpty(pathBase)) {
    app.UsePathBase(pathBase);
}

app.UseSwagger()
            .UseSwaggerUI(setup => {
                setup.SwaggerEndpoint($"{(!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty)}/swagger/v1/swagger.json", "Basket.API V1");
                setup.OAuthClientId("basketswaggerui");
                setup.OAuthAppName("Basket Swagger UI");
            });

app.UseRouting();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();


app.MapGrpcService<BasketService>();
app.MapDefaultControllerRoute();
app.MapControllers();
app.MapGet("/_proto/", async ctx => {
    ctx.Response.ContentType = "text/plain";
    using var fs = new FileStream(Path.Combine(app.Environment.ContentRootPath, "Proto", "basket.proto"), FileMode.Open, FileAccess.Read);
    using var sr = new StreamReader(fs);
    while (!sr.EndOfStream) {
        var line = await sr.ReadLineAsync();
        if (line != "/* >>" || line != "<< */") {
            await ctx.Response.WriteAsync(line);
        }
    }
});
app.MapHealthChecks("/hc", new HealthCheckOptions() {
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecks("/liveness", new HealthCheckOptions {
    Predicate = r => r.Name.Contains("self")
});
ConfigureEventBus(app);
try {
    Log.Information("Configuring web host ({ApplicationContext})...", Program.AppName);


    Log.Information("Starting web host ({ApplicationContext})...", Program.AppName);
    await app.RunAsync();

    return 0;
}
catch (Exception ex) {
    Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", Program.AppName);
    return 1;
}
finally {
    Log.CloseAndFlush();
}

Serilog.ILogger CreateSerilogLogger(IConfiguration configuration) {
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

(int httpPort, int grpcPort) GetDefinedPorts(IConfiguration config) {
    var grpcPort = config.GetValue("GRPC_PORT", 5001);
    var port = config.GetValue("PORT", 80);
    return (port, grpcPort);
}
void ConfigureEventBus(IApplicationBuilder app) {
    var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

    eventBus.Subscribe<ProductPriceChangedIntegrationEvent, ProductPriceChangedIntegrationEventHandler>();
    eventBus.Subscribe<OrderStartedIntegrationEvent, OrderStartedIntegrationEventHandler>();
}
public partial class Program {

    public static string Namespace = typeof(Program).Assembly.GetName().Name;
    public static string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
}


public static class CustomExtensionMethods {


    public static IServiceCollection RegisterEventBus(this IServiceCollection services, IConfiguration configuration) {
        if (configuration.GetValue<bool>("AzureServiceBusEnabled")) {
            services.AddSingleton<IEventBus, EventBusServiceBus>(sp => {
                var serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
                var logger = sp.GetRequiredService<ILogger<EventBusServiceBus>>();
                var eventBusSubscriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
                string subscriptionName = configuration["SubscriptionClientName"];

                return new EventBusServiceBus(serviceBusPersisterConnection, logger,
                    eventBusSubscriptionsManager, sp, subscriptionName);
            });
        }
        else {
            services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp => {
                var subscriptionClientName = configuration["SubscriptionClientName"];
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                var eventBusSubscriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                var retryCount = 5;
                if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"])) {
                    retryCount = int.Parse(configuration["EventBusRetryCount"]);
                }

                return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, sp, eventBusSubscriptionsManager, subscriptionClientName, retryCount);
            });
        }

        services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

        services.AddTransient<ProductPriceChangedIntegrationEventHandler>();
        services.AddTransient<OrderStartedIntegrationEventHandler>();
        return services;
    }
}
