namespace Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure.Services;

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

    public string GetUserName()
    {
        return "Dummy User Name";
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
//
//     public string GetUserName()
//     {
//         return _context.HttpContext.User.Identity.Name;
//     }
// }
