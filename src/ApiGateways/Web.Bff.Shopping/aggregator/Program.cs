var appName = "Web.Shopping.HttpAggregator";
var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(CreateSerilogLogger(builder.Configuration));
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy())
    .AddUrlGroup(new Uri(builder.Configuration["CatalogUrlHC"]), name: "catalogapi-check", tags: new string[] { "catalogapi" })
    .AddUrlGroup(new Uri(builder.Configuration["OrderingUrlHC"]), name: "orderingapi-check", tags: new string[] { "orderingapi" })
    .AddUrlGroup(new Uri(builder.Configuration["BasketUrlHC"]), name: "basketapi-check", tags: new string[] { "basketapi" })
    .AddUrlGroup(new Uri(builder.Configuration["IdentityUrlHC"]), name: "identityapi-check", tags: new string[] { "identityapi" })
    .AddUrlGroup(new Uri(builder.Configuration["PaymentUrlHC"]), name: "paymentapi-check", tags: new string[] { "paymentapi" });
builder.Services.AddCustomMvc(builder.Configuration)
    .AddCustomAuthentication(builder.Configuration)
    .AddApplicationServices()
    .AddGrpcServices();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}
var pathBase = builder.Configuration["PATH_BASE"];
if (!string.IsNullOrEmpty(pathBase))
{
    app.UsePathBase(pathBase);
}

app.UseHttpsRedirection();

app.UseSwagger().UseSwaggerUI(c =>
{
    c.SwaggerEndpoint($"{(!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty)}/swagger/v1/swagger.json", "Purchase BFF V1");

    c.OAuthClientId("webshoppingaggswaggerui");
    c.OAuthClientSecret(string.Empty);
    c.OAuthRealm(string.Empty);
    c.OAuthAppName("web shopping bff Swagger UI");
});

app.UseRouting();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute();
app.MapControllers();
app.MapHealthChecks("/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecks("/liveness", new HealthCheckOptions
{
    Predicate = r => r.Name.Contains("self")
});

try
{
    Log.Information("Starts Web Application ({ApplicationContext})...", Program.AppName);
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
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}
public partial class Program
{

    public static string Namespace = typeof(Program).Assembly.GetName().Name;
    public static string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

        var identityUrl = configuration.GetValue<string>("urls:identity");
        services.AddAuthentication("Bearer")
        .AddJwtBearer(options =>
        {
            options.Authority = identityUrl;
            options.RequireHttpsMetadata = false;
            options.Audience = "webshoppingagg";
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false
            };
        });

        return services;
    }
    public static IServiceCollection AddCustomMvc(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.Configure<UrlsConfig>(configuration.GetSection("urls"));

        services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Shopping Aggregator for Web Clients",
                Version = "v1",
                Description = "Shopping Aggregator for Web Clients"
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
                            { "webshoppingagg", "Shopping Aggregator for Web Clients" }
                        }
                    }
                }
            });

            options.OperationFilter<AuthorizeCheckOperationFilter>();
        });
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder
                .SetIsOriginAllowed((host) => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
        });

        return services;
    }
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        //register delegating handlers
        services.AddTransient<HttpClientAuthorizationDelegatingHandler>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        //register http services

        services.AddHttpClient<IOrderApiClient, OrderApiClient>()
            .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>();

        return services;
    }

    public static IServiceCollection AddGrpcServices(this IServiceCollection services)
    {
        services.AddTransient<GrpcExceptionInterceptor>();

        services.AddScoped<IBasketService, BasketService>();

        services.AddGrpcClient<Basket.BasketClient>((services, options) =>
        {
            var basketApi = services.GetRequiredService<IOptions<UrlsConfig>>().Value.GrpcBasket;
            options.Address = new Uri(basketApi);
        }).AddInterceptor<GrpcExceptionInterceptor>();

        services.AddScoped<ICatalogService, CatalogService>();

        services.AddGrpcClient<Catalog.CatalogClient>((services, options) =>
        {
            var catalogApi = services.GetRequiredService<IOptions<UrlsConfig>>().Value.GrpcCatalog;
            options.Address = new Uri(catalogApi);
        }).AddInterceptor<GrpcExceptionInterceptor>();

        services.AddScoped<IOrderingService, OrderingService>();

        services.AddGrpcClient<OrderingGrpc.OrderingGrpcClient>((services, options) =>
        {
            var orderingApi = services.GetRequiredService<IOptions<UrlsConfig>>().Value.GrpcOrdering;
            options.Address = new Uri(orderingApi);
        }).AddInterceptor<GrpcExceptionInterceptor>();

        return services;
    }
}
