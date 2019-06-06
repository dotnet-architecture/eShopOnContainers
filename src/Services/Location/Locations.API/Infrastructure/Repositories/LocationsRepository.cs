namespace Microsoft.eShopOnContainers.Services.Locations.API.Infrastructure.Repositories
{
    using Microsoft.eShopOnContainers.Services.Locations.API.Model;
    using Microsoft.Extensions.Options;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.GeoJsonObjectModel;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ViewModel;

    public class LocationsRepository
        : ILocationsRepository
    {
        private readonly LocationsContext _context;       

        public LocationsRepository(IOptions<LocationSettings> settings)
        {
            _context = new LocationsContext(settings);
        }        
        
        public async Task<Locations> GetAsync(int locationId)
        {
            var filter = Builders<Locations>.Filter.Eq("LocationId", locationId);
            return await _context.Locations
                                 .Find(filter)
                                 .FirstOrDefaultAsync();
        }

        public async Task<UserLocation> GetUserLocationAsync(string userId)
        {
            var filter = Builders<UserLocation>.Filter.Eq("UserId", userId);
            return await _context.UserLocation
                                 .Find(filter)
                                 .FirstOrDefaultAsync();
        }

        public async Task<List<Locations>> GetLocationListAsync()
        {
            return await _context.Locations.Find(new BsonDocument()).ToListAsync();
        }       

        public async Task<List<Locations>> GetCurrentUserRegionsListAsync(LocationRequest currentPosition)
        {
            var point = GeoJson.Point(GeoJson.Geographic(currentPosition.Longitude, currentPosition.Latitude));
            var orderByDistanceQuery = new FilterDefinitionBuilder<Locations>().Near(x => x.Location, point);
            var withinAreaQuery = new FilterDefinitionBuilder<Locations>().GeoIntersects("Polygon", point);
            var filter = Builders<Locations>.Filter.And(orderByDistanceQuery, withinAreaQuery);
            return await _context.Locations.Find(filter).ToListAsync(); 
        }        

        public async Task AddUserLocationAsync(UserLocation location)
        {
            await _context.UserLocation.InsertOneAsync(location);
        }

        public async Task UpdateUserLocationAsync(UserLocation userLocation)
        {
            await _context.UserLocation.ReplaceOneAsync(
                doc => doc.UserId == userLocation.UserId,
                userLocation,
                new UpdateOptions { IsUpsert = true });
        }
    }
}
