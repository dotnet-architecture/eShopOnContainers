namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Queries
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IOrderQueries
    {
        Task<dynamic> GetOrderAsync(int id);

        Task<IEnumerable<dynamic>> GetOrdersAsync();

        Task<IEnumerable<dynamic>> GetCardTypesAsync();
    }
}
