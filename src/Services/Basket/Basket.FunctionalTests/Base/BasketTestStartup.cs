

namespace Basket.FunctionalTests.Base
{
    class BasketTestsStartup : Startup
    {
        public BasketTestsStartup(IConfiguration env) : base(env)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            // Added to avoid the Authorize data annotation in test environment. 
            // Property "SuppressCheckForUnhandledSecurityMetadata" in appsettings.json
            services.Configure<RouteOptions>(Configuration);
            base.ConfigureServices(services);
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
