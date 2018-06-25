using Microsoft.AspNetCore.Builder;
using Microsoft.eShopOnContainers.Services.Marketing.API;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketing.FunctionalTests.Base
{
    public class MarketingTestsStartup : Startup
    {
        public MarketingTestsStartup(IConfiguration configuration) : base(configuration)
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
