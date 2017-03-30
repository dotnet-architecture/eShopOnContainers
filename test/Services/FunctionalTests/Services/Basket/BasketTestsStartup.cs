using Microsoft.eShopOnContainers.Services.Basket.API;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using FunctionalTests.Middleware;

namespace FunctionalTests.Services.Basket
{
    public class BasketTestsStartup : Startup
    {
        public BasketTestsStartup(IHostingEnvironment env) : base(env)
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
