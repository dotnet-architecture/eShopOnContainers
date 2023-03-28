namespace Basket.FunctionalTests.Base;

public class BasketScenarioBase
{
    private const string ApiUrlBase = "api/v1/basket";

    public TestServer CreateServer()
    {
        var path = Assembly.GetAssembly(typeof(BasketScenarioBase))
            .Location;

        var hostBuilder = new WebHostBuilder()
            .UseContentRoot(Path.GetDirectoryName(path))
            .ConfigureAppConfiguration(cb =>
            {
                cb.AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables();
            });

        return new TestServer(hostBuilder);
    }

    public static class Get
    {
        public static string GetBasket(int id)
        {
            return $"{ApiUrlBase}/{id}";
        }
    }

    public static class Post
    {
        public static string Basket = $"{ApiUrlBase}/";
        public static string CheckoutOrder = $"{ApiUrlBase}/checkout";
    }
}
