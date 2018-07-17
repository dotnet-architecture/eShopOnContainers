namespace IntegrationTests.Services.Locations
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.eShopOnContainers.Services.Locations.API;
    using Microsoft.Extensions.Configuration;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class LocationsTestsStartup : Startup
    {
        public LocationsTestsStartup(IConfiguration env) : base(env)
        {
        }

        protected override void ConfigureAuth(IApplicationBuilder app)
        {
            if (Configuration["isTest"] == bool.TrueString.ToLowerInvariant())
            {
                app.UseMiddleware<LocationAuthorizeMiddleware>();
            }
            else
            {
                base.ConfigureAuth(app);
            }
        }

        class LocationAuthorizeMiddleware
        {
            private readonly RequestDelegate _next;
            public LocationAuthorizeMiddleware(RequestDelegate rd)
            {
                _next = rd;
            }

            public async Task Invoke(HttpContext httpContext)
            {
                var identity = new ClaimsIdentity("cookies");
                identity.AddClaim(new Claim("sub", "4611ce3f-380d-4db5-8d76-87a8689058ed"));
                httpContext.User.AddIdentity(identity);
                await _next.Invoke(httpContext);
            }
        }
    }
}
