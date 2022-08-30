namespace Microsoft.eShopOnContainers.WebUI.Services;

public interface IIdentityParser<T>
{
    T Parse(IPrincipal principal);
}
