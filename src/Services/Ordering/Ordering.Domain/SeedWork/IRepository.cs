namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.Seedwork
{
    public interface IRepository
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
