namespace FunctionalTests.Services.Basket;
using Microsoft.eShopOnContainers.Services.Basket.API;
class BasketTestsStartup : Startup
{
    public BasketTestsStartup(IConfiguration env) : base(env)
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
