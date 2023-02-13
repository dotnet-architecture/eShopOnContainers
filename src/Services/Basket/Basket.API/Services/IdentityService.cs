namespace Microsoft.eShopOnContainers.Services.Basket.API.Services;

public class IdentityServiceFake : IIdentityService
{
    private IHttpContextAccessor _context;

    public IdentityServiceFake(IHttpContextAccessor context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public string GetUserIdentity()
    {
        if (_context.HttpContext
            .Request
            .Headers
            .TryGetValue("user-id", out var value))
        {
            return value.Single();
        }

        return null;    
    }
}

// HACK: no auth 
// public class IdentityService : IIdentityService
// {
//     private IHttpContextAccessor _context;
//
//     public IdentityService(IHttpContextAccessor context)
//     {
//         _context = context ?? throw new ArgumentNullException(nameof(context));
//     }
//
//     public string GetUserIdentity()
//     {
//         return _context.HttpContext.User.FindFirst("sub").Value;
//     }
// }

