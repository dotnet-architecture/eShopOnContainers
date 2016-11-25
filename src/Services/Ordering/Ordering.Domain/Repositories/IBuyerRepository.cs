using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.Repositories
{
    public interface IBuyerRepository
        :IRepository
    {
        Buyer Add(Buyer buyer);

        Task<Buyer> FindAsync(string name);
    }
}
