using eShopOnContainers.Core.Models.Orders;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eShopOnContainers.Core.Services.User
{
    public interface IUserService
    {
        Task<Models.User.User> GetUserAsync();
        Task<List<Order>> GetOrdersAsync();
    }
}