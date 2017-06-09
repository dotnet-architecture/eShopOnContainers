namespace Microsoft.eShopOnContainers.Services.Locations.API.Infrastructure.Repositories
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.eShopOnContainers.Services.Locations.API.Model;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Extensions.Options;
    using MongoDB.Driver;
    using MongoDB.Driver.GeoJsonObjectModel;
    using MongoDB.Driver.Builders;
    using MongoDB.Bson;

    public class LocationsRepository
        : ILocationsRepository
    {
        private readonly LocationsContext _context;       

        public LocationsRepository(IOptions<LocationSettings> settings)
        {
            _context = new LocationsContext(settings);
        }        
        
        public async Task<Locations> GetAsync(ObjectId locationId)
        {
            var filter = Builders<Locations>.Filter.Eq("Id", locationId);
            return await _context.Locations
                                 .Find(filter)
                                 .FirstOrDefaultAsync();
        }

        public async Task<UserLocation> GetUserLocationAsync(int userId)
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

        public async Task<List<Locations>> GetNearestLocationListAsync(double lat, double lon)
        {
            var point = GeoJson.Point(GeoJson.Geographic(lon, lat));
            var query = new FilterDefinitionBuilder<Locations>().Near(x => x.Location, point);
            return await _context.Locations.Find(query).ToListAsync(); 
        }

        public async Task<Locations> GetLocationByCurrentAreaAsync(Locations location)
        {
            var query = new FilterDefinitionBuilder<Locations>().GeoIntersects("Location", location.Polygon);
            return await _context.Locations.Find(query).FirstOrDefaultAsync();
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
