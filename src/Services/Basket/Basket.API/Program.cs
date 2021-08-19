var configuration = GetConfiguration();
Log.Logger = CreateSerilogLogger(configuration);

try
{
    Log.Information("Configuring web host ({ApplicationContext})...", Program.AppName);

    var builder = WebApplication.CreateBuilder(args);
    builder.Configuration.AddConfiguration(configuration);

    ConfigureServices(builder);        
    BuildWebHost(builder);    

    var app = builder.Build();
    ConfigureRequestPipeline(app);    

    Log.Information("Starting web host ({ApplicationContext})...", Program.AppName);

    app.Run();

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

/// <summary>
/// /  Method to help configure services
/// </summary>
void ConfigureServices(WebApplicationBuilder builder)
{
    builder.Services.AddGrpc(options =>
    {
        options.EnableDetailedErrors = true;
    });

    builder.Services.AddApplicationInsightsTelemetry(configuration);
    builder.Services.AddApplicationInsightsKubernetesEnricher();

    builder.Services.AddControllers(options =>
    {
        options.Filters.Add(typeof(HttpGlobalExceptionFilter));
        options.Filters.Add(typeof(ValidateModelStateFilter));

    }).AddApplicationPart(typeof(BasketController).Assembly)
.AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);
    
    RegisterOpenAPIConfig(builder);
    RegisterAuthService(builder);
    RegisterHeathCheckConfigs(builder);
    RegisterRedisDataStore(builder);
    RegisterEventBusConnection(builder);
    RegisterEventBus(builder);            

    builder.Services.Configure<BasketSettings>(configuration);
    builder.Services.AddCors(options =>
    {
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

    builder.Services.AddAutofac(container =>
    {
        container.Populate(builder.Services);
    });
}

/// <summary>
///    Method to configure app request pipeline.
///
/// </summary>
void ConfigureRequestPipeline(WebApplication app)
{
    UsePathBase(app);
    UseOpenAPI(app);

    app.UseRouting();
    app.UseCors("CorsPolicy");

    UseConfiguredAuth(app);

    app.UseStaticFiles();
    UseMappedEndpoints(app);
    UseEventBus(app);
}

void BuildWebHost(WebApplicationBuilder builder)
{
    builder.WebHost.CaptureStartupErrors(false)
        .ConfigureKestrel(options =>
        {
            var ports = GetDefinedPorts(configuration);
            options.Listen(IPAddress.Any, ports.httpPort, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
            });

            options.Listen(IPAddress.Any, ports.grpcPort, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http2;
            });

        })
        .ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
        .UseFailing(options =>
        {
            options.ConfigPath = "/Failing";
            options.NotFilteredPaths.AddRange(new[] { "/hc", "/liveness" });
        })
        .UseContentRoot(Directory.GetCurrentDirectory())
        .UseSerilog();
}

void UseEventBus(IApplicationBuilder app)
{
    var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

    eventBus.Subscribe<ProductPriceChangedIntegrationEvent, ProductPriceChangedIntegrationEventHandler>();
    eventBus.Subscribe<OrderStartedIntegrationEvent, OrderStartedIntegrationEventHandler>();
}

void UseMappedEndpoints(IApplicationBuilder app)
{
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapGrpcService<BasketService>();
        endpoints.MapDefaultControllerRoute();
        endpoints.MapControllers();
        //endpoints.MapGet("/_proto/", async ctx =>
        //{
        //    ctx.Response.ContentType = "text/plain";
        //    using var fs = new FileStream(Path.Combine(env.ContentRootPath, "Proto", "basket.proto"), FileMode.Open, FileAccess.Read);
        //    using var sr = new StreamReader(fs);
        //    while (!sr.EndOfStream)
        //    {
        //        var line = await sr.ReadLineAsync();
        //        if (line != "/* >>" || line != "<< */")
        //        {
        //            await ctx.Response.WriteAsync(line);
        //        }
        //    }
        //});

        endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
        {
            Predicate = r => r.Name.Contains("self")
        });
    });

}

void UsePathBase(IApplicationBuilder app)
{

    var pathBase = configuration["PATH_BASE"];
    if (!string.IsNullOrEmpty(pathBase))
    {
        app.UsePathBase(pathBase);
    }
}

void UseOpenAPI(IApplicationBuilder app)
{
    var pathBase = configuration["PATH_BASE"];

    app.UseSwagger()
        .UseSwaggerUI(setup =>
        {
            setup.SwaggerEndpoint($"{ (!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty) }/swagger/v1/swagger.json", "Basket.API V1");
            setup.OAuthClientId("basketswaggerui");
            setup.OAuthAppName("Basket Swagger UI");
        });
}

void UseConfiguredAuth(IApplicationBuilder app)
{
    app.UseAuthentication();
    app.UseAuthorization();
}

void RegisterOpenAPIConfig(WebApplicationBuilder builder)
{
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "eShopOnContainers - Basket HTTP API",
            Version = "v1",
            Description = "The Basket Service HTTP API"
        });

        options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows()
            {
                Implicit = new OpenApiOAuthFlow()
                {
                    AuthorizationUrl = new Uri($"{configuration.GetValue<string>("IdentityUrlExternal")}/connect/authorize"),
                    TokenUrl = new Uri($"{configuration.GetValue<string>("IdentityUrlExternal")}/connect/token"),
                    Scopes = new Dictionary<string, string>()
                        {
                            { "basket", "Basket API" }
                        }
                }
            }
        });

        options.OperationFilter<AuthorizeCheckOperationFilter>();
    });
}

void RegisterAuthService(WebApplicationBuilder builder)
{
    // prevent from mapping "sub" claim to nameidentifier.
    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

    var identityUrl = configuration.GetValue<string>("IdentityUrl");

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    }).AddJwtBearer(options =>
    {
        options.Authority = identityUrl;
        options.RequireHttpsMetadata = false;
        options.Audience = "basket";
    });

}

void RegisterHeathCheckConfigs(WebApplicationBuilder builder)
{
    var hcBuilder = builder.Services.AddHealthChecks();

    hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

    hcBuilder
        .AddRedis(
            configuration["ConnectionString"],
            name: "redis-check",
            tags: new string[] { "redis" });

    if (configuration.GetValue<bool>("AzureServiceBusEnabled"))
    {
        hcBuilder
            .AddAzureServiceBusTopic(
                configuration["EventBusConnection"],
                topicName: "eshop_event_bus",
                name: "basket-servicebus-check",
                tags: new string[] { "servicebus" });
    }
    else
    {
        hcBuilder
            .AddRabbitMQ(
                $"amqp://{configuration["EventBusConnection"]}",
                name: "basket-rabbitmqbus-check",
                tags: new string[] { "rabbitmqbus" });
    }
}

void RegisterRedisDataStore(WebApplicationBuilder builder)
{
    //By connecting here we are making sure that our service
    //cannot start until redis is ready. This might slow down startup,
    //but given that there is a delay on resolving the ip address
    //and then creating the connection it seems reasonable to move
    //that cost to startup instead of having the first request pay the
    //penalty.
    builder.Services.AddSingleton<ConnectionMultiplexer>(sp =>
    {
        var settings = sp.GetRequiredService<IOptions<BasketSettings>>().Value;
        var configuration = ConfigurationOptions.Parse(settings.ConnectionString, true);

        configuration.ResolveDns = true;

        return ConnectionMultiplexer.Connect(configuration);
    });

}

void RegisterEventBusConnection(WebApplicationBuilder builder)
{
    if (configuration.GetValue<bool>("AzureServiceBusEnabled"))
    {
        builder.Services.AddSingleton<IServiceBusPersisterConnection>(sp =>
        {
            var serviceBusConnectionString = configuration["EventBusConnection"];
            var serviceBusConnection = new ServiceBusConnectionStringBuilder(serviceBusConnectionString);

            var subscriptionClientName = configuration["SubscriptionClientName"];
            return new DefaultServiceBusPersisterConnection(serviceBusConnection, subscriptionClientName);
        });
    }
    else
    {
        builder.Services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

            var factory = new ConnectionFactory()
            {
                HostName = configuration["EventBusConnection"],
                DispatchConsumersAsync = true
            };

            if (!string.IsNullOrEmpty(configuration["EventBusUserName"]))
            {
                factory.UserName = configuration["EventBusUserName"];
            }

            if (!string.IsNullOrEmpty(configuration["EventBusPassword"]))
            {
                factory.Password = configuration["EventBusPassword"];
            }

            var retryCount = 5;
            if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
            {
                retryCount = int.Parse(configuration["EventBusRetryCount"]);
            }

            return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
        });
    }
}

void RegisterEventBus(WebApplicationBuilder builder)
{
    if (configuration.GetValue<bool>("AzureServiceBusEnabled"))
    {
        builder.Services.AddSingleton<IEventBus, EventBusServiceBus>(sp =>
        {
            var serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
            var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
            var logger = sp.GetRequiredService<ILogger<EventBusServiceBus>>();
            var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

            return new EventBusServiceBus(serviceBusPersisterConnection, logger,
                eventBusSubcriptionsManager, iLifetimeScope);
        });
    }
    else
    {
        builder.Services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
        {
            var subscriptionClientName = configuration["SubscriptionClientName"];
            var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
            //var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
            var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
            var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

            var retryCount = 5;
            if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
            {
                retryCount = int.Parse(configuration["EventBusRetryCount"]);
            }

            //return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
            // TO DO
            return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, null, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
        });
    }

    builder.Services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

    builder.Services.AddTransient<ProductPriceChangedIntegrationEventHandler>();
    builder.Services.AddTransient<OrderStartedIntegrationEventHandler>();
}

Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
{
    var seqServerUrl = configuration["Serilog:SeqServerUrl"];
    var logstashUrl = configuration["Serilog:LogstashgUrl"];
    return new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .Enrich.WithProperty("ApplicationContext", Program.AppName)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.Seq(string.IsNullOrWhiteSpace(seqServerUrl) ? "http://seq" : seqServerUrl)
        .WriteTo.Http(string.IsNullOrWhiteSpace(logstashUrl) ? "http://logstash:8080" : logstashUrl)
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}

IConfiguration GetConfiguration()
{
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

    var config = builder.Build();

    if (config.GetValue<bool>("UseVault", false))
    {
        TokenCredential credential = new ClientSecretCredential(
            config["Vault:TenantId"],
            config["Vault:ClientId"],
            config["Vault:ClientSecret"]);
        builder.AddAzureKeyVault(new Uri($"https://{config["Vault:Name"]}.vault.azure.net/"), credential);
    }

    return builder.Build();
}

(int httpPort, int grpcPort) GetDefinedPorts(IConfiguration config)
{
    var grpcPort = config.GetValue("GRPC_PORT", 5001);
    var port = config.GetValue("PORT", 80);
    return (port, grpcPort);
}

public class Program
{    
    public static string Namespace = typeof(Program).Namespace;
    public static string AppName = "Basket.API";
}
