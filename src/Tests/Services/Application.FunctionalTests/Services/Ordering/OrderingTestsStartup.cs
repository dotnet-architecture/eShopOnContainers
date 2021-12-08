namespace FunctionalTests.Services.Ordering;
using Microsoft.eShopOnContainers.Services.Ordering.API;

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
            app.UseAuthorization();
        }
        else
        {
            base.ConfigureAuth(app);
        }
    }
}
