using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Services
{
    public class IdentityService : IIdentityService
    {
        private IHttpContextAccessor _context; 

        public IdentityService(IHttpContextAccessor context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public string GetUserIdentity()
        {
            return _context.HttpContext.User.FindFirst("sub").Value;
        }

        public string GetUserName()
        {
            return _context.HttpContext.User.FindFirst(x => x.Type == ClaimTypes.Name).Value;
        }

        public async Task<string> GetUserToken()
        {
            return await _context.HttpContext.GetTokenAsync("access_token");
        }
    }
}
