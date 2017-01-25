namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Queries
{
    using System.Threading.Tasks;

    public interface IOrderQueries
    {
        Task<dynamic> GetOrder(int id);

        Task<dynamic> GetOrders();

        Task<dynamic> GetCardTypes();
    }
}
