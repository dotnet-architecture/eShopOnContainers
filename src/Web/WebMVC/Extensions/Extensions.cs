// Fix samesite issue when running eShop from docker-compose locally as by default http protocol is being used
// Refer to https://github.com/dotnet-architecture/eShopOnContainers/issues/1391
internal static class Extensions
{
    public static void AddHealthChecks(this WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            .AddUrlGroup(_ => new Uri(builder.Configuration["IdentityUrlHC"]), name: "identityapi-check", tags: new string[] { "identityapi" });
    }

    public static void AddApplicationSevices(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<AppSettings>(builder.Configuration)
            .AddSession()
            .AddDistributedMemoryCache();

        if (builder.Configuration["DPConnectionString"] is string redisConnection)
        {
            builder.Services.AddDataProtection(opts =>
            {
                opts.ApplicationDiscriminator = "eshop.webmvc";
            })
            .PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect(redisConnection), "DataProtection-Keys");
        }
    }

    // Adds all Http client services
    public static void AddHttpClientServices(this WebApplicationBuilder builder)
    {
        // Register delegating handlers
        builder.Services.AddTransient<HttpClientAuthorizationDelegatingHandler>()
            .AddTransient<HttpClientRequestIdDelegatingHandler>();

        // Add http client services
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

    public static void AddAuthenticationServices(this WebApplicationBuilder builder)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

        var identityUrl = builder.Configuration.GetRequiredValue("IdentityUrl");
        var callBackUrl = builder.Configuration.GetRequiredValue("CallBackUrl");
        var sessionCookieLifetime = builder.Configuration.GetValue("SessionCookieLifetimeMinutes", 60);

        // Add Authentication services

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddCookie(options => options.ExpireTimeSpan = TimeSpan.FromMinutes(sessionCookieLifetime))
        .AddOpenIdConnect(options =>
        {
            options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.Authority = identityUrl;
            options.SignedOutRedirectUri = callBackUrl;
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
}
