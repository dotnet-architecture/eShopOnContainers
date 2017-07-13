using Basket.API.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.eShopOnContainers.Services.Basket.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseFailing(options =>
                {
                    options.ConfigPath = "/Failing";
                    options.EndpointPaths = new List<string>()
                    { "/api/v1/basket/checkout", "/hc" };
                })
                .UseHealthChecks("/hc")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
