namespace Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure.Services
{
    public interface IIdentityService
    {
        string GetUserIdentity();

        string GetUserName();
    }
}
