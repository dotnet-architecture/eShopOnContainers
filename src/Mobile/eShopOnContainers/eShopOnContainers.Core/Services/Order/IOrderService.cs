using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace eShopOnContainers.Core.Services.Order
{
    public interface IOrderService
    {
        Task CreateOrderAsync(Models.Orders.Order newOrder);
        Task<ObservableCollection<Models.Orders.Order>> GetOrdersAsync();
        Task<Models.Orders.Order> GetOrderAsync(int orderId);
        Task<ObservableCollection<Models.Orders.CardType>> GetCardTypesAsync();
    }
}