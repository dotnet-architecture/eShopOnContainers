var appName = "Catalog.API";
var builder = WebApplication.CreateBuilder(new WebApplicationOptions {
    Args = args,
    ApplicationName = typeof(Program).Assembly.FullName,
    ContentRootPath = Directory.GetCurrentDirectory(),
    WebRootPath = "Pics",
});
if (builder.Configuration.GetValue<bool>("UseVault", false)) {
    TokenCredential credential = new ClientSecretCredential(
        builder.Configuration["Vault:TenantId"],
        builder.Configuration["Vault:ClientId"],
        builder.Configuration["Vault:ClientSecret"]);
    //builder.AddAzureKeyVault(new Uri($"https://{builder.Configuration["Vault:Name"]}.vault.azure.net/"), credential);        
}
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.WebHost.UseKestrel(options => {
    var ports = GetDefinedPorts(builder.Configuration);
    options.Listen(IPAddress.Any, ports.httpPort, listenOptions => {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });
    options.Listen(IPAddress.Any, ports.grpcPort, listenOptions => {
        listenOptions.Protocols = HttpProtocols.Http2;
    });

});
builder.Services.AddAppInsight(builder.Configuration);
builder.Services.AddGrpc().Services
    .AddCustomMVC(builder.Configuration)
    .AddCustomDbContext(builder.Configuration)
    .AddCustomOptions(builder.Configuration)
    .AddCustomHealthCheck(builder.Configuration)
    .AddIntegrationServices(builder.Configuration)
    .AddEventBus(builder.Configuration)
    .AddSwagger(builder.Configuration);

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
            .UseSwaggerUI(c => {
                c.SwaggerEndpoint($"{(!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty)}/swagger/v1/swagger.json", "Catalog.API V1");
            });

app.UseRouting();
app.UseCors("CorsPolicy");
app.MapDefaultControllerRoute();
app.MapControllers();
app.UseFileServer(new FileServerOptions {
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "Pics")),
    RequestPath = "/pics"
});
app.UseStaticFiles(new StaticFileOptions {
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "Pics")),
    RequestPath = "/pics"
});
app.MapGet("/_proto/", async ctx => {
    ctx.Response.ContentType = "text/plain";
    using var fs = new FileStream(Path.Combine(app.Environment.ContentRootPath, "Proto", "catalog.proto"), FileMode.Open, FileAccess.Read);
    using var sr = new StreamReader(fs);
    while (!sr.EndOfStream) {
        var line = await sr.ReadLineAsync();
        if (line != "/* >>" || line != "<< */") {
            await ctx.Response.WriteAsync(line);
        }
    }
});
app.MapGrpcService<CatalogService>();
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
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<CatalogContext>();
    var env = app.Services.GetService<IWebHostEnvironment>();
    var settings = app.Services.GetService<IOptions<CatalogSettings>>();
    var logger = app.Services.GetService<ILogger<CatalogContextSeed>>();
    await context.Database.MigrateAsync();

    await new CatalogContextSeed().SeedAsync(context, env, settings, logger);
    var integEventContext = scope.ServiceProvider.GetRequiredService<IntegrationEventLogContext>();
    await integEventContext.Database.MigrateAsync();
    app.Logger.LogInformation("Starting web host ({ApplicationName})...", appName);
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
void ConfigureEventBus(IApplicationBuilder app) {
    var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
    eventBus.Subscribe<OrderStatusChangedToAwaitingValidationIntegrationEvent, OrderStatusChangedToAwaitingValidationIntegrationEventHandler>();
    eventBus.Subscribe<OrderStatusChangedToPaidIntegrationEvent, OrderStatusChangedToPaidIntegrationEventHandler>();
}

(int httpPort, int grpcPort) GetDefinedPorts(IConfiguration config) {
    var grpcPort = config.GetValue("GRPC_PORT", 81);
    var port = config.GetValue("PORT", 80);
    return (port, grpcPort);
}

public partial class Program {
    public static string Namespace = typeof(Program).Assembly.GetName().Name;
    public static string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
}

public static class CustomExtensionMethods {
    public static IServiceCollection AddAppInsight(this IServiceCollection services, IConfiguration configuration) {
        services.AddApplicationInsightsTelemetry(configuration);
        services.AddApplicationInsightsKubernetesEnricher();

        return services;
    }

    public static IServiceCollection AddCustomMVC(this IServiceCollection services, IConfiguration configuration) {
        services.AddControllers(options => {
            options.Filters.Add(typeof(HttpGlobalExceptionFilter));
        })
        .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);

        services.AddCors(options => {
            options.AddPolicy("CorsPolicy",
                builder => builder
                .SetIsOriginAllowed((host) => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
        });

        return services;
    }

    public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration) {
        var accountName = configuration.GetValue<string>("AzureStorageAccountName");
        var accountKey = configuration.GetValue<string>("AzureStorageAccountKey");

        var hcBuilder = services.AddHealthChecks();

        hcBuilder
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddSqlServer(
                configuration["ConnectionString"],
                name: "CatalogDB-check",
                tags: new string[] { "catalogdb" });

        if (!string.IsNullOrEmpty(accountName) && !string.IsNullOrEmpty(accountKey)) {
            hcBuilder
                .AddAzureBlobStorage(
                    $"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={accountKey};EndpointSuffix=core.windows.net",
                    name: "catalog-storage-check",
                    tags: new string[] { "catalogstorage" });
        }

        if (configuration.GetValue<bool>("AzureServiceBusEnabled")) {
            hcBuilder
                .AddAzureServiceBusTopic(
                    configuration["EventBusConnection"],
                    topicName: "eshop_event_bus",
                    name: "catalog-servicebus-check",
                    tags: new string[] { "servicebus" });
        }
        else {
            hcBuilder
                .AddRabbitMQ(
                    $"amqp://{configuration["EventBusConnection"]}",
                    name: "catalog-rabbitmqbus-check",
                    tags: new string[] { "rabbitmqbus" });
        }

        return services;
    }

    public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration) {
        services.AddEntityFrameworkSqlServer()
            .AddDbContext<CatalogContext>(options => {
                options.UseSqlServer(configuration["ConnectionString"],
                                        sqlServerOptionsAction: sqlOptions => {
                                            sqlOptions.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                                            //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                                            sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                        });
            });

        services.AddDbContext<IntegrationEventLogContext>(options => {
            options.UseSqlServer(configuration["ConnectionString"],
                                    sqlServerOptionsAction: sqlOptions => {
                                        sqlOptions.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                                        //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                    });
        });

        return services;
    }

    public static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration configuration) {
        services.Configure<CatalogSettings>(configuration);
        services.Configure<ApiBehaviorOptions>(options => {
            options.InvalidModelStateResponseFactory = context => {
                var problemDetails = new ValidationProblemDetails(context.ModelState) {
                    Instance = context.HttpContext.Request.Path,
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "Please refer to the errors property for additional details."
                };

                return new BadRequestObjectResult(problemDetails) {
                    ContentTypes = { "application/problem+json", "application/problem+xml" }
                };
            };
        });

        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration) {
        services.AddSwaggerGen(options => {
            options.SwaggerDoc("v1", new OpenApiInfo {
                Title = "eShopOnContainers - Catalog HTTP API",
                Version = "v1",
                Description = "The Catalog Microservice HTTP API. This is a Data-Driven/CRUD microservice sample"
            });
        });

        return services;

    }

    public static IServiceCollection AddIntegrationServices(this IServiceCollection services, IConfiguration configuration) {
        services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
            sp => (DbConnection c) => new IntegrationEventLogService(c));

        services.AddTransient<ICatalogIntegrationEventService, CatalogIntegrationEventService>();

        if (configuration.GetValue<bool>("AzureServiceBusEnabled")) {
            services.AddSingleton<IServiceBusPersisterConnection>(sp => {
                var settings = sp.GetRequiredService<IOptions<CatalogSettings>>().Value;
                var serviceBusConnection = settings.EventBusConnection;

                return new DefaultServiceBusPersisterConnection(serviceBusConnection);
            });
        }
        else {
            services.AddSingleton<IRabbitMQPersistentConnection>(sp => {
                var settings = sp.GetRequiredService<IOptions<CatalogSettings>>().Value;
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory() {
                    HostName = configuration["EventBusConnection"],
                    DispatchConsumersAsync = true
                };

                if (!string.IsNullOrEmpty(configuration["EventBusUserName"])) {
                    factory.UserName = configuration["EventBusUserName"];
                }

                if (!string.IsNullOrEmpty(configuration["EventBusPassword"])) {
                    factory.Password = configuration["EventBusPassword"];
                }

                var retryCount = 5;
                if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"])) {
                    retryCount = int.Parse(configuration["EventBusRetryCount"]);
                }

                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });
        }

        return services;
    }

    public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration) {
        if (configuration.GetValue<bool>("AzureServiceBusEnabled")) {
            services.AddSingleton<IEventBus, EventBusServiceBus>(sp => {
                var serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
                var logger = sp.GetRequiredService<ILogger<EventBusServiceBus>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
                string subscriptionName = configuration["SubscriptionClientName"];

                return new EventBusServiceBus(serviceBusPersisterConnection, logger,
                    eventBusSubcriptionsManager, sp, subscriptionName);
            });

        }
        else {
            services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp => {
                var subscriptionClientName = configuration["SubscriptionClientName"];
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                var retryCount = 5;
                if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"])) {
                    retryCount = int.Parse(configuration["EventBusRetryCount"]);
                }

                return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, sp, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
            });
        }

        services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
        services.AddTransient<OrderStatusChangedToAwaitingValidationIntegrationEventHandler>();
        services.AddTransient<OrderStatusChangedToPaidIntegrationEventHandler>();

        return services;
    }
}