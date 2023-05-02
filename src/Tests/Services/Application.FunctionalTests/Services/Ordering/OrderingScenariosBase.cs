using FunctionalTests.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace FunctionalTests.Services.Ordering;

public class OrderingScenariosBase : WebApplicationFactory<OrderingProgram>
{
    public TestServer CreateServer()
    {
        Services
            .MigrateDbContext<OrderingContext>((context, services) =>
            {
                var env = services.GetService<IWebHostEnvironment>();
                var settings = services.GetService<IOptions<OrderingSettings>>();
                var logger = services.GetService<ILogger<OrderingContextSeed>>();

                new OrderingContextSeed()
                    .SeedAsync(context, env, settings, logger)
                    .Wait();
            })
            .MigrateDbContext<IntegrationEventLogContext>((_, __) => { });

        return Server;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(servies =>
        {
            servies.AddSingleton<IStartupFilter, AuthStartupFilter>();
        });

        builder.ConfigureAppConfiguration(c =>
        {
            var directory = Path.GetDirectoryName(typeof(OrderingScenariosBase).Assembly.Location)!;

            c.AddJsonFile(Path.Combine(directory, "Services/Ordering/appsettings.json"), optional: false);
        });

        return base.CreateHost(builder);
    }

    public static class Get
    {
        public static string Orders = "api/v1/orders";

        public static string OrderBy(int id)
        {
            return $"api/v1/orders/{id}";
        }
    }

    public static class Post
    {
        public static string AddNewOrder = "api/v1/orders/new";
    }

    public static class Put
    {
        public static string CancelOrder = "api/v1/orders/cancel";
    }

    public static class Delete
    {
        public static string OrderBy(int id)
        {
            return $"api/v1/orders/{id}";
        }
    }

    private class AuthStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                app.UseMiddleware<AutoAuthorizeMiddleware>();

                next(app);
            };
        }
    }
}
