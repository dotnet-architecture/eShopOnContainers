using Catalog.API.Model;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure.Services
{
    public interface IStorage
    {
        Task SaveAsync(Payload payload);
    }
}
