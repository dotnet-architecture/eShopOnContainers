using eShopOnContainers.Core.Models.Products;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace eShopOnContainers.Core.Services.Products
{
    public interface IProductsService
    {
        Task<ObservableCollection<Product>> GetProductsAsync();
    }
}
