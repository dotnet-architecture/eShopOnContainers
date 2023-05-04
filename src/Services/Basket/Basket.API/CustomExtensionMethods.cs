namespace Microsoft.eShopOnContainers.Services.Basket.API;

public static class CustomExtensionMethods
{
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddRedis(_ =>
            {
                return configuration.GetConnectionString("redis");
            },
            "redis", tags: new[] { "ready", "liveness" });

        return services.AddSingleton(sp =>
        {
            var redisConfig = ConfigurationOptions.Parse(configuration.GetConnectionString("redis"), true);

            return ConnectionMultiplexer.Connect(redisConfig);
        });
    }
}
