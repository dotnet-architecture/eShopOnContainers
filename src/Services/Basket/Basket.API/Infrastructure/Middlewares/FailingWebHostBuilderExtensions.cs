using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Basket.API.Infrastructure.Middlewares
{
    public static class WebHostBuildertExtensions
    {
        public static IWebHostBuilder UseFailing(this IWebHostBuilder builder, Action<FailingOptions> options)
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<IStartupFilter>(new FailingStartupFilter(options));
            });
            return builder;
        }
    }
}
