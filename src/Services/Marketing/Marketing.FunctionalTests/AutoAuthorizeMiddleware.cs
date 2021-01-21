using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Marketing.FunctionalTests
{
    class AutoAuthorizeMiddleware
    {
        private readonly RequestDelegate _next;

        public AutoAuthorizeMiddleware(RequestDelegate rd)
        {
            _next = rd;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var identity = new ClaimsIdentity("cookies");
            identity.AddClaim(new Claim("sub", "1234"));

            httpContext.User.AddIdentity(identity);

            await _next.Invoke(httpContext);
        }
    }
}
