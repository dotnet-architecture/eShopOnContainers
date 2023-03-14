var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

AddApplicationInsights(builder);
AddHealthChecks(builder);
AddCustomMvc(builder);
AddHttpClientServices(builder);
AddCustomAuthentication(builder);

builder.WebHost.CaptureStartupErrors(false);
builder.Host.UseSerilog(CreateSerilogLogger(builder.Configuration));

var app = builder.Build();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
}

var pathBase = builder.Configuration["PATH_BASE"];

if (!string.IsNullOrEmpty(pathBase))
{
    app.UsePathBase(pathBase);
}

app.UseStaticFiles();
app.UseSession();

WebContextSeed.Seed(app, app.Environment);

// Fix samesite issue when running eShop from docker-compose locally as by default http protocol is being used
// Refer to https://github.com/dotnet-architecture/eShopOnContainers/issues/1391
app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute("default", "{controller=Catalog}/{action=Index}/{id?}");
app.MapControllerRoute("defaultError", "{controller=Error}/{action=Error}");
app.MapControllers();
app.MapHealthChecks("/liveness", new HealthCheckOptions
{
    Predicate = r => r.Name.Contains("self")
});
app.MapHealthChecks("/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

await app.RunAsync();

Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
{
    var seqServerUrl = configuration["Serilog:SeqServerUrl"];
    var logstashUrl = configuration["Serilog:LogstashgUrl"];
    var cfg = new LoggerConfiguration()
        .ReadFrom.Configuration(configuration)
        .Enrich.WithProperty("ApplicationContext", AppName)
        .Enrich.FromLogContext()
        .WriteTo.Console();
    if (!string.IsNullOrWhiteSpace(seqServerUrl))
    {
        cfg.WriteTo.Seq(seqServerUrl);
    }
    if (!string.IsNullOrWhiteSpace(logstashUrl))
    {
        cfg.WriteTo.Http(logstashUrl,null);
    }
    return cfg.CreateLogger();
}

static void AddApplicationInsights(WebApplicationBuilder builder)
{
    builder.Services.AddApplicationInsightsTelemetry(builder.Configuration);
    builder.Services.AddApplicationInsightsKubernetesEnricher();
}

static void AddHealthChecks(WebApplicationBuilder builder)
{
    builder.Services.AddHealthChecks()
        .AddCheck("self", () => HealthCheckResult.Healthy())
        .AddUrlGroup(new Uri(builder.Configuration["IdentityUrlHC"]), name: "identityapi-check", tags: new string[] { "identityapi" });
}

static void AddCustomMvc(WebApplicationBuilder builder)
{
    builder.Services.AddOptions()
        .Configure<AppSettings>(builder.Configuration)
        .AddSession()
        .AddDistributedMemoryCache();

    if (builder.Configuration.GetValue<string>("IsClusterEnv") == bool.TrueString)
    {
        builder.Services.AddDataProtection(opts =>
        {
            opts.ApplicationDiscriminator = "eshop.webmvc";
        })
        .PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect(builder.Configuration["DPConnectionString"]), "DataProtection-Keys");
    }
}

// Adds all Http client services
static void AddHttpClientServices(WebApplicationBuilder builder)
{
    builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

    //register delegating handlers
    builder.Services.AddTransient<HttpClientAuthorizationDelegatingHandler>()
        .AddTransient<HttpClientRequestIdDelegatingHandler>();

    //set 5 min as the lifetime for each HttpMessageHandler int the pool
    builder.Services.AddHttpClient("extendedhandlerlifetime").SetHandlerLifetime(TimeSpan.FromMinutes(5));

    //add http client services
    builder.Services.AddHttpClient<IBasketService, BasketService>()
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Sample. Default lifetime is 2 minutes
            .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>();

    builder.Services.AddHttpClient<ICatalogService, CatalogService>();

    builder.Services.AddHttpClient<IOrderingService, OrderingService>()
            .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
            .AddHttpMessageHandler<HttpClientRequestIdDelegatingHandler>();


    //add custom application services
    builder.Services.AddTransient<IIdentityParser<ApplicationUser>, IdentityParser>();
}

static void AddCustomAuthentication(WebApplicationBuilder builder)
{
    var identityUrl = builder.Configuration.GetValue<string>("IdentityUrl");
    var callBackUrl = builder.Configuration.GetValue<string>("CallBackUrl");
    var sessionCookieLifetime = builder.Configuration.GetValue("SessionCookieLifetimeMinutes", 60);

    // Add Authentication services          

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie(setup => setup.ExpireTimeSpan = TimeSpan.FromMinutes(sessionCookieLifetime))
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.Authority = identityUrl.ToString();
        options.SignedOutRedirectUri = callBackUrl.ToString();
        options.ClientId = "mvc";
        options.ClientSecret = "secret";
        options.ResponseType = "code";
        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;
        options.RequireHttpsMetadata = false;
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("orders");
        options.Scope.Add("basket");
        options.Scope.Add("webshoppingagg");
        options.Scope.Add("orders.signalrhub");
        options.Scope.Add("webhooks");
    });
}

public partial class Program
{
    public static readonly string AppName = typeof(Program).Assembly.GetName().Name;
}