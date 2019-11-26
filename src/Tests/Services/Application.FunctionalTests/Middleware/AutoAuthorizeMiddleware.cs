using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FunctionalTests.Middleware
{
    class AutoAuthorizeMiddleware
    {
        public const string IDENTITY_ID = "9e3163b9-1ae6-4652-9dc6-7898ab7b7a00";

        private readonly RequestDelegate _next;


        public AutoAuthorizeMiddleware(RequestDelegate rd)
        {
            _next = rd;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var identity = new ClaimsIdentity("cookies");

            identity.AddClaim(new Claim("sub", IDENTITY_ID));
            identity.AddClaim(new Claim("unique_name", IDENTITY_ID));

            httpContext.User.AddIdentity(identity);
            await _next.Invoke(httpContext);
        }
    }
}
