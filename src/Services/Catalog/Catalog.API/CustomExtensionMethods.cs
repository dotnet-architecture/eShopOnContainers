public static class CustomExtensionMethods
{
    public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
    {
        var hcBuilder = services.AddHealthChecks();

        if (configuration.GetConnectionString("CatalogDB") is string connectionString)
        {
            hcBuilder
                .AddSqlServer(
                    connectionString,
                    name: "CatalogDB-check",
                    tags: new string[] { "catalogdb" });
        }

        var accountName = configuration["AzureStorageAccountName"];
        var accountKey = configuration["AzureStorageAccountKey"];

        if (!string.IsNullOrEmpty(accountName) && !string.IsNullOrEmpty(accountKey))
        {
            hcBuilder
                .AddAzureBlobStorage(
                    $"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={accountKey};EndpointSuffix=core.windows.net",
                    name: "catalog-storage-check",
                    tags: new string[] { "catalogstorage" });
        }

        return services;
    }

    public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEntityFrameworkSqlServer()
            .AddDbContext<CatalogContext>(options =>
            {
                var connectionString = configuration.GetRequiredConnectionString("CatalogDB");

                options.UseSqlServer(connectionString,
                                    sqlServerOptionsAction: sqlOptions =>
                                    {
                                        sqlOptions.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                                        //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                    });
            });

        services.AddDbContext<IntegrationEventLogContext>(options =>
        {
            var connectionString = configuration.GetRequiredConnectionString("CatalogDB");

            options.UseSqlServer(connectionString,
                                    sqlServerOptionsAction: sqlOptions =>
                                    {
                                        sqlOptions.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                                        //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                    });
        });

        return services;
    }

    public static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CatalogSettings>(configuration);
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Instance = context.HttpContext.Request.Path,
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "Please refer to the errors property for additional details."
                };

                return new BadRequestObjectResult(problemDetails)
                {
                    ContentTypes = { "application/problem+json", "application/problem+xml" }
                };
            };
        });

        return services;
    }

    public static IServiceCollection AddIntegrationServices(this IServiceCollection services)
    {
        services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
            sp => (DbConnection c) => new IntegrationEventLogService(c));

        services.AddTransient<ICatalogIntegrationEventService, CatalogIntegrationEventService>();

        return services;
    }

    private static string GetRequiredConnectionString(this IConfiguration configuration, string name) =>
    configuration.GetConnectionString(name) ?? throw new InvalidOperationException($"Configuration missing value for: {(configuration is IConfigurationSection s ? s.Path + ":ConnectionStrings:" + name : "ConnectionStrings:" + name)}");
}
