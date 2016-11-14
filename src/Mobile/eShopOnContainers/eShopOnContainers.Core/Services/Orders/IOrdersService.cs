using eShopOnContainers.Core.Models.Orders;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace eShopOnContainers.Core.Services.Orders
{
    public interface IOrdersService
    {
        Task<ObservableCollection<Order>> GetOrdersAsync();

        Task<Order> GetCartAsync();
    }
}