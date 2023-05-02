namespace Microsoft.eShopOnContainers.Services.Basket.API;

public static class CustomExtensionMethods
{
    public static IServiceCollection AddRedis(this IServiceCollection services)
    {
        // {
        //  "ConnectionString": "..."
        // }

        return services.AddSingleton(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<BasketSettings>>().Value;
            var configuration = ConfigurationOptions.Parse(settings.ConnectionString, true);

            return ConnectionMultiplexer.Connect(configuration);
        });
    }
}
