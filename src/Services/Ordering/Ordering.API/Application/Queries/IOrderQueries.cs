namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Queries
{
    using System.Threading.Tasks;

    public interface IOrderQueries
    {
        Task<dynamic> GetOrderAsync(int id);

        Task<dynamic> GetOrdersAsync();

        Task<dynamic> GetCardTypesAsync();
    }
}
