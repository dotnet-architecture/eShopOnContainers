var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddKeyVault();

builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
});

builder.Services.AddApplicationInsightsTelemetry(builder.Configuration);
builder.Services.AddApplicationInsightsKubernetesEnricher();

builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(HttpGlobalExceptionFilter));
    options.Filters.Add(typeof(ValidateModelStateFilter));
});

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
                AuthorizationUrl = new Uri($"{builder.Configuration["IdentityUrlExternal"]}/connect/authorize"),
                TokenUrl = new Uri($"{builder.Configuration["IdentityUrlExternal"]}/connect/token"),
                Scopes = new Dictionary<string, string>() { { "basket", "Basket API" } }
            }
        }
    });

    options.OperationFilter<AuthorizeCheckOperationFilter>();
});

// prevent from mapping "sub" claim to nameidentifier.
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

var identityUrl = builder.Configuration["IdentityUrl"];

builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.Authority = identityUrl;
    options.RequireHttpsMetadata = false;
    options.Audience = "basket";
    options.TokenValidationParameters.ValidateAudience = false;
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "basket");
    });
});

builder.Services.AddCustomHealthCheck(builder.Configuration);

builder.Services.Configure<BasketSettings>(builder.Configuration);

builder.Services.AddRedis();

builder.Services.AddEventBus(builder.Configuration);

builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<IBasketRepository, RedisBasketRepository>();
builder.Services.AddTransient<IIdentityService, IdentityService>();

builder.WebHost.UseKestrel(options =>
{
    var ports = GetDefinedPorts(builder.Configuration);
    options.Listen(IPAddress.Any, ports.httpPort, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });

    options.Listen(IPAddress.Any, ports.grpcPort, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

builder.Host.UseSerilog(CreateSerilogLogger(builder.Configuration));
builder.WebHost.UseFailing(options =>
{
    options.ConfigPath = "/Failing";
    options.NotFilteredPaths.AddRange(new[] { "/hc", "/liveness" });
});

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

app.UseSwagger();

app.UseSwaggerUI(setup =>
{
    setup.SwaggerEndpoint($"{(!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty)}/swagger/v1/swagger.json", "Basket.API V1");
    setup.OAuthClientId("basketswaggerui");
    setup.OAuthAppName("Basket Swagger UI");
});

app.MapGrpcService<BasketService>();
app.MapControllers();

app.MapGet("/_proto/", async ctx =>
{
    ctx.Response.ContentType = "text/plain";
    using var fs = new FileStream(Path.Combine(app.Environment.ContentRootPath, "Proto", "basket.proto"), FileMode.Open, FileAccess.Read);
    using var sr = new StreamReader(fs);
    while (!sr.EndOfStream)
    {
        var line = await sr.ReadLineAsync();
        if (line != "/* >>" || line != "<< */")
        {
            await ctx.Response.WriteAsync(line);
        }
    }
});

app.MapHealthChecks("/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/liveness", new HealthCheckOptions
{
    Predicate = r => r.Name.Contains("self")
});

ConfigureEventBus(app);

try
{
    Log.Information("Configuring web host ({ApplicationContext})...", Program.AppName);


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
        .WriteTo.Http(string.IsNullOrWhiteSpace(logstashUrl) ? "http://logstash:8080" : logstashUrl, null)
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}

(int httpPort, int grpcPort) GetDefinedPorts(IConfiguration config)
{
    var grpcPort = config.GetValue("GRPC_PORT", 5001);
    var port = config.GetValue("PORT", 80);
    return (port, grpcPort);
}

void ConfigureEventBus(IApplicationBuilder app)
{
    var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

    eventBus.Subscribe<ProductPriceChangedIntegrationEvent, ProductPriceChangedIntegrationEventHandler>();
    eventBus.Subscribe<OrderStartedIntegrationEvent, OrderStartedIntegrationEventHandler>();
}

public partial class Program
{
    private static string Namespace = typeof(Program).Assembly.GetName().Name;
    public static string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
}
