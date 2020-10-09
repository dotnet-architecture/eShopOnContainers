namespace Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure.Services
{
    public interface ISpecification<T>
    {
        bool IsSatisfiedBy(T file);
    }
}
