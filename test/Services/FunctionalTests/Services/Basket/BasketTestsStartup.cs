using FunctionalTests.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.eShopOnContainers.Services.Basket.API;
using Microsoft.Extensions.Configuration;

namespace FunctionalTests.Services.Basket
{
    public class BasketTestsStartup : Startup
    {
        public BasketTestsStartup(IConfiguration configuration) : base(configuration)
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
