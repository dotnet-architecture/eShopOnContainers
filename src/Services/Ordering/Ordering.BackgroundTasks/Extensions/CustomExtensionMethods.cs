namespace Ordering.BackgroundTasks.Extensions;

public static class CustomExtensionMethods
{
    public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var hcBuilder = services.AddHealthChecks();

        hcBuilder.AddSqlServer(_ =>
                configuration.GetRequiredConnectionString("OrderingDB"),
                name: "OrderingTaskDB-check",
                tags: new string[] { "live", "ready" });

        return services;
    }

    public static IServiceCollection AddApplicationOptions(this IServiceCollection services, IConfiguration configuration)
    {
        return services.Configure<BackgroundTaskSettings>(configuration)
                .Configure<BackgroundTaskSettings>(o =>
        {
            o.ConnectionString = configuration.GetRequiredConnectionString("OrderingDB");
        });
    }
}
