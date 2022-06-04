namespace Microsoft.eShopOnContainers.WebMVC.Services;

public interface IIdentityParser<T>
{
    T Parse(IPrincipal principal);
}
