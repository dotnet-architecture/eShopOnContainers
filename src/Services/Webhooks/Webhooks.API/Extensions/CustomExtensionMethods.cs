internal static class CustomExtensionMethods
{
    public static IServiceCollection AddDbContexts(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<WebhooksContext>(options =>
        {
            options.UseSqlServer(configuration.GetRequiredConnectionString("WebHooksDb"),
                                sqlServerOptionsAction: sqlOptions =>
                                {
                                    sqlOptions.MigrationsAssembly(typeof(Program).Assembly.FullName);

                                    //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                });
        });

        return services;
    }

    public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var hcBuilder = services.AddHealthChecks();

        hcBuilder
            .AddSqlServer(_ =>
                configuration.GetRequiredConnectionString("WebHooksDb"),
                name: "WebhooksApiDb-check",
                tags: new string[] { "ready" });

        return services;
    }

    public static IServiceCollection AddHttpClientServices(this IServiceCollection services)
    {
        // Add http client services
        services.AddHttpClient("GrantClient")
                .SetHandlerLifetime(TimeSpan.FromMinutes(5));

        return services;
    }

    public static IServiceCollection AddIntegrationServices(this IServiceCollection services)
    {
        return services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
                sp => (DbConnection c) => new IntegrationEventLogService(c));
    }
}
