namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork
{
    public interface IRepository
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
