using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace Basket.FunctionalTests.Base;

public class BasketScenarioBase
{
    private const string ApiUrlBase = "api/v1/basket";

    public TestServer CreateServer()
    {
        var factory = new BasketApplication();
        return factory.Server;
    }

    public static class Get
    {
        public static string GetBasket(int id)
        {
            return $"{ApiUrlBase}/{id}";
        }

        public static string GetBasketByCustomer(string customerId)
        {
            return $"{ApiUrlBase}/{customerId}";
        }
    }

    public static class Post
    {
        public static string Basket = $"{ApiUrlBase}/";
        public static string CheckoutOrder = $"{ApiUrlBase}/checkout";
    }

    private class BasketApplication : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<IStartupFilter, AuthStartupFilter>();
            });

            builder.ConfigureAppConfiguration(c =>
            {
                var directory = Path.GetDirectoryName(typeof(BasketScenarioBase).Assembly.Location)!;

                c.AddJsonFile(Path.Combine(directory, "appsettings.Basket.json"), optional: false);
            });

            return base.CreateHost(builder);
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
}
