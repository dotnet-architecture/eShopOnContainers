namespace Microsoft.eShopOnContainers.Services.Marketing.API.Infrastructure
{
    using Microsoft.eShopOnContainers.Services.Marketing.API.Model;
    using Microsoft.Extensions.Options;
    using MongoDB.Driver;

    public class MarketingReadDataContext
    {
        private readonly IMongoDatabase _database = null;

        public MarketingReadDataContext(IOptions<MarketingSettings> settings)
        {
            var client = new MongoClient(settings.Value.MongoConnectionString);

            if (client != null)
            {
                _database = client.GetDatabase(settings.Value.MongoDatabase);
            }
        }

        public IMongoCollection<MarketingData> MarketingData
        {
            get
            {
                return _database.GetCollection<MarketingData>("MarketingReadDataModel");
            }
        }        
    }
}
