
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {
        private IHttpContextAccessor _context; 

        public IdentityService(IHttpContextAccessor context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _context = context;
        }

        public string GetUserIdentity()
        {
            return _context.HttpContext.User.FindFirst("sub").Value;
        }
    }
}
