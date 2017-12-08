using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebMVC.Infrastructure.Middlewares
{
    class ByPassAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private string _currentUserId;
        public ByPassAuthMiddleware(RequestDelegate next)
        {
            _next = next;
            _currentUserId = null;
        }


        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path;
            if (path == "/noauth")
            {
                var userid = context.Request.Query["userid"];
                if (!string.IsNullOrEmpty(userid))
                {
                    _currentUserId = userid;
                }
                context.Response.StatusCode = 200;
                context.Response.ContentType = "text/string";
                await context.Response.WriteAsync($"User set to {_currentUserId}");
            }

            else if (path == "/noauth/reset")
            {
                _currentUserId = null;
                context.Response.StatusCode = 200;
                context.Response.ContentType = "text/string";
                await context.Response.WriteAsync($"User set to none. Token required for protected endpoints.");
            }
            else
            {
                var currentUserId = _currentUserId;

                var authHeader = context.Request.Headers["Authorization"];
                if (authHeader != StringValues.Empty)
                {
                    var header = authHeader.FirstOrDefault();
                    if (!string.IsNullOrEmpty(header) && header.StartsWith("Email ") && header.Length > "Email ".Length)
                    {
                        currentUserId = header.Substring("Email ".Length);
                    }
                }


                if (!string.IsNullOrEmpty(currentUserId))
                {
                    var user = new ClaimsIdentity(new[] {
                    new Claim("emails", currentUserId),
                    new Claim("name", "Test user"),
                    new Claim("nonce", Guid.NewGuid().ToString()),
                    new Claim("ttp://schemas.microsoft.com/identity/claims/identityprovider", "ByPassAuthMiddleware"),
                    new Claim("nonce", Guid.NewGuid().ToString()),
                    new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname","User"),
                    new Claim("sub", "1234"),
                    new Claim("card_expiration", "12/20"),
                    new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname","Microsoft")}
                    , "ByPassAuth");

                    context.User = new ClaimsPrincipal(user);
                }

                await _next.Invoke(context);
            }
        }
    }
}
