namespace Microsoft.eShopOnContainers.Services.Locations.API.Infrastructure.Repositories
{
    using Microsoft.eShopOnContainers.Services.Locations.API.Model;
    using MongoDB.Bson;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ILocationsRepository
    {        
        Task<Locations> GetAsync(ObjectId locationId);

        Task<List<Locations>> GetLocationListAsync();

        Task<UserLocation> GetUserLocationAsync(int userId);

        Task<List<Locations>> GetNearestLocationListAsync(double lat, double lon);

        Task<Locations> GetLocationByCurrentAreaAsync(Locations location);

        Task AddUserLocationAsync(UserLocation location);

        Task UpdateUserLocationAsync(UserLocation userLocation);

    }
}
