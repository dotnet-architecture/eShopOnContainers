namespace eShopOnContainers.Core.Services.Identity
{
    public interface IIdentityService
    {
        string CreateAuthorizeRequest();
        string DecodeToken(string token);
    }
}
