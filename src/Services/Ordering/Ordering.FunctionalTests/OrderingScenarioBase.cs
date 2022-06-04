namespace Ordering.FunctionalTests;

public class OrderingScenarioBase
{
    public TestServer CreateServer()
    {
        var path = Assembly.GetAssembly(typeof(OrderingScenarioBase))
            .Location;

        var hostBuilder = new WebHostBuilder()
            .UseContentRoot(Path.GetDirectoryName(path))
            .ConfigureAppConfiguration(cb =>
            {
                cb.AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables();
            }).UseStartup<OrderingTestsStartup>();

        var testServer = new TestServer(hostBuilder);

        testServer.Host
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

        return testServer;
    }

    public static class Get
    {
        public static string Orders = "api/v1/orders";

        public static string OrderBy(int id)
        {
            return $"api/v1/orders/{id}";
        }
    }

    public static class Put
    {
        public static string CancelOrder = "api/v1/orders/cancel";
        public static string ShipOrder = "api/v1/orders/ship";
    }
}
