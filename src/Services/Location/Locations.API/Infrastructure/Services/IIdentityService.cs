using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Services.Locations.API.Infrastructure.Services
{
    public interface IIdentityService
    {
        string GetUserIdentity();
    }
}
