using Services.Common;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var services = new (string, string, string)[]
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
var urls = builder.Configuration.GetRequiredSection("Urls");

foreach (var (routeId, prefix, clusterId) in services)
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

builder.Services.AddReverseProxy()
    .LoadFromMemory(routes, clusters.Values.ToList())
    .AddTransforms(builder =>
    {
        if (builder.Route?.Metadata?["prefix"] is string prefix)
        {
            builder.AddPathRemovePrefix($"/{prefix}");
        }
    });

var app = builder.Build();

app.UseServiceDefaults();

app.MapReverseProxy();

app.Run();
