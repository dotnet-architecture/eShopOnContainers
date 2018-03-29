using Microsoft.eShopOnContainers.Services.Marketing.API.Model;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Services.Marketing.API.Infrastructure.Repositories
{
    public class MarketingDataRepository
        : IMarketingDataRepository
    {
        private readonly MarketingReadDataContext _context;

        public MarketingDataRepository(IOptions<MarketingSettings> settings)
        {
            _context = new MarketingReadDataContext(settings);
        }

        public Task<MarketingData> GetAsync(string userId)
        {
            var filter = Builders<MarketingData>.Filter.Eq("UserId", userId);
            return _context.MarketingData
                                 .Find(filter)
                                 .FirstOrDefaultAsync();
        }

        public Task UpdateLocationAsync(MarketingData marketingData)
        {
            var filter = Builders<MarketingData>.Filter.Eq("UserId", marketingData.UserId);
            var update = Builders<MarketingData>.Update
                .Set("Locations", marketingData.Locations)
                .CurrentDate("UpdateDate");

            return _context.MarketingData
                .UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
        }
    }
}
