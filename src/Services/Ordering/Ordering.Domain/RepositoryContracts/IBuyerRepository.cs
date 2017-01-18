using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.BuyerAggregate;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.RepositoryContracts
{
    public interface IBuyerRepository
        :IRepository
    {
        Buyer Add(Buyer buyer);

        Task<Buyer> FindAsync(string name);
    }
}
