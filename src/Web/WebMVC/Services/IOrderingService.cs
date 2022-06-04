namespace Microsoft.eShopOnContainers.WebMVC.Services;
using Microsoft.eShopOnContainers.WebMVC.ViewModels;

public interface IOrderingService
{
    Task<List<Order>> GetMyOrders(ApplicationUser user);
    Task<Order> GetOrder(ApplicationUser user, string orderId);
    Task CancelOrder(string orderId);
    Task ShipOrder(string orderId);
    Order MapUserInfoIntoOrder(ApplicationUser user, Order order);
    BasketDTO MapOrderToBasket(Order order);
    void OverrideUserInfoIntoOrder(Order original, Order destination);
}
