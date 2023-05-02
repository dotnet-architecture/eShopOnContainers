using FunctionalTests.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace FunctionalTests.Services.Basket;

public class BasketScenariosBase : WebApplicationFactory<BasketProgram>
{
    private const string ApiUrlBase = "api/v1/basket";

    public TestServer CreateServer()
    {
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
            var directory = Path.GetDirectoryName(typeof(BasketScenariosBase).Assembly.Location)!;

            c.AddJsonFile(Path.Combine(directory, "Services/Basket/appsettings.json"), optional: false);
        });

        return base.CreateHost(builder);
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
        public static string CreateBasket = $"{ApiUrlBase}/";
        public static string CheckoutOrder = $"{ApiUrlBase}/checkout";
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
