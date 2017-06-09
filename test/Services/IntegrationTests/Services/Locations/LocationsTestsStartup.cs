namespace IntegrationTests.Services.Locations
{
    using IntegrationTests.Middleware;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.eShopOnContainers.Services.Locations.API;

    public class LocationsTestsStartup : Startup
    {
        public LocationsTestsStartup(IHostingEnvironment env) : base(env)
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
