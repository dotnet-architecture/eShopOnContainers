using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Ordering.API.Infrastructure.Middlewares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Hosting
{
    public static class WebHostBuildertExtensions
    {
        public static IWebHostBuilder UseFailing(this IWebHostBuilder builder, string path)
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<IStartupFilter>(new FailingStartupFilter());
            });
            return builder;
        }

    }
}
