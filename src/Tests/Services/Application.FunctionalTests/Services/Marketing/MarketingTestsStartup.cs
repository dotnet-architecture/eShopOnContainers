namespace FunctionalTests.Services.Marketing
{
    using FunctionalTests.Middleware;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.eShopOnContainers.Services.Marketing.API;
    using Microsoft.Extensions.Configuration;

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
                app.UseAuthorization();
            }
            else
            {
                base.ConfigureAuth(app);
            }
        }
    }
}
