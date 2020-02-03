using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.eShopOnContainers.Services.Ordering.API;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ordering.FunctionalTests
{
    public class OrderingTestsStartup : Startup
    {
        public OrderingTestsStartup(IConfiguration env) : base(env)
        {
        }

        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Added to avoid the Authorize data annotation in test environment. 
            // Property "SuppressCheckForUnhandledSecurityMetadata" in appsettings.json
            services.Configure<RouteOptions>(Configuration);
            return base.ConfigureServices(services);
        }
        protected override void ConfigureAuth(IApplicationBuilder app)
        {
            if (Configuration["isTest"] == bool.TrueString.ToLowerInvariant())
            {
                app.UseMiddleware<AutoAuthorizeMiddleware>();
            }
            else
            {
                base.ConfigureAuth(app);
            }
        }
    }
}
