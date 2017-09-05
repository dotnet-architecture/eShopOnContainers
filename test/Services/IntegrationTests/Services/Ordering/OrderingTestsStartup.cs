using Microsoft.eShopOnContainers.Services.Ordering.API;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using IntegrationTests.Middleware;
using Microsoft.Extensions.Configuration;

namespace IntegrationTests.Services.Ordering
{
    public class OrderingTestsStartup : Startup
    {
        public OrderingTestsStartup(IConfiguration env) : base(env)
        {
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
