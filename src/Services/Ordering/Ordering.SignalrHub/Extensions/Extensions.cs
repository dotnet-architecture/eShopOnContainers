internal static class Extensions
{
    public static IServiceCollection AddSignalR(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration.GetConnectionString("redis") is string redisConnection)
        {
            // TODO: Add a redis health check
            services.AddSignalR().AddStackExchangeRedis(redisConnection);
        }
        else
        {
            services.AddSignalR();
        }

        // Configure hub auth (grab the token from the query string)
        return services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];

                    var endpoint = context.HttpContext.GetEndpoint();

                    // Make sure this is a Hub endpoint.
                    if (endpoint?.Metadata.GetMetadata<HubMetadata>() is null)
                    {
                        return Task.CompletedTask;
                    }

                    context.Token = accessToken;

                    return Task.CompletedTask;
                }
            };
        });
    }

}
