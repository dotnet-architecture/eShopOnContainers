using FunctionalTests.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.eShopOnContainers.Services.Ordering.API;

namespace FunctionalTests.Services.Marketing
{
    public class MarketingTestsStartup : Startup
    {
        public MarketingTestsStartup(IHostingEnvironment env) : base(env)
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
