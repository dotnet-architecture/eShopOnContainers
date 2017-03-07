using FunctionalTests.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.eShopOnContainers.Services.Ordering.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionalTests.Services.Ordering
{
    public class OrderingTestsStartup : Startup
    {
        public OrderingTestsStartup(IHostingEnvironment env) : base(env)
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
