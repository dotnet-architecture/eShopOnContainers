namespace Microsoft.eShopOnContainers.Services.Marketing.API.Infrastructure.Repositories
{
    using Model;
    using System.Threading.Tasks;

    public interface IMarketingDataRepository
    {
        Task<MarketingData> GetAsync(string userId);
        Task UpdateLocationAsync(MarketingData marketingData);
    }
}
