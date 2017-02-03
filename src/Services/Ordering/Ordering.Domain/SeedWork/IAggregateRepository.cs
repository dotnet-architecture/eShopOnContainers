namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.Seedwork
{
    public interface IAggregateRepository
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
