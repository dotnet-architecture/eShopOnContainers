using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;

internal static class Extensions
{
    public static IServiceCollection AddReverseProxy(this IServiceCollection services, IConfiguration configuration)
    {
        // REVIEW: This should come from configuration
        var s = new (string, string, string)[]
        {
            ("c-short", "c", "catalog"),
            ("c-long", "catalog-api", "catalog"),

            ("b-short", "b", "basket"),
            ("b-long", "basket-api", "basket"),

            ("o-short", "o", "orders"),
            ("o-long", "ordering-api", "orders"),

            ("h-long", "hub/notificationhub", "signalr")
        };

        var routes = new List<RouteConfig>();
        var clusters = new Dictionary<string, ClusterConfig>();
        var urls = configuration.GetRequiredSection("Urls");

        foreach (var (routeId, prefix, clusterId) in s)
        {
            var destination = urls.GetRequiredValue(clusterId);

            routes.Add(new()
            {
                RouteId = routeId,
                ClusterId = clusterId,
                Match = new()
                {
                    Path = $"/{prefix}/{{**catch-all}}"
                },
                Metadata = new Dictionary<string, string>()
                {
                    ["prefix"] = prefix
                }
            });

            clusters[clusterId] = new()
            {
                ClusterId = clusterId,
                Destinations = new Dictionary<string, DestinationConfig>()
                {
                    { clusterId, new DestinationConfig() { Address = destination } }
                }
            };
        }

        services.AddReverseProxy().LoadFromMemory(routes, clusters.Values.ToList())
                .AddTransforms(builder =>
                {
                    if (builder.Route?.Metadata?["prefix"] is string prefix)
                    {
                        builder.AddPathRemovePrefix($"/{prefix}");
                    }
                });

        return services;
    }

    public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddUrlGroup(_ => new Uri(configuration.GetRequiredValue("CatalogUrlHC")), name: "catalogapi-check", tags: new string[] { "catalogapi" })
            .AddUrlGroup(_ => new Uri(configuration.GetRequiredValue("OrderingUrlHC")), name: "orderingapi-check", tags: new string[] { "orderingapi" })
            .AddUrlGroup(_ => new Uri(configuration.GetRequiredValue("BasketUrlHC")), name: "basketapi-check", tags: new string[] { "basketapi" })
            .AddUrlGroup(_ => new Uri(configuration.GetRequiredValue("IdentityUrlHC")), name: "identityapi-check", tags: new string[] { "identityapi" });

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register delegating handlers
        services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

        // Register http services
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

        services.AddGrpcClient<GrpcOrdering.OrderingGrpc.OrderingGrpcClient>((services, options) =>
        {
            var orderingApi = services.GetRequiredService<IOptions<UrlsConfig>>().Value.GrpcOrdering;
            options.Address = new Uri(orderingApi);
        }).AddInterceptor<GrpcExceptionInterceptor>();

        return services;
    }
}
