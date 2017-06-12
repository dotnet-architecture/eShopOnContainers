namespace Microsoft.eShopOnContainers.Services.Locations.API.Infrastructure.Repositories
{
    using Microsoft.eShopOnContainers.Services.Locations.API.Model;
    using ViewModel;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ILocationsRepository
    {        
        Task<Locations> GetAsync(string locationId);

        Task<List<Locations>> GetLocationListAsync();

        Task<UserLocation> GetUserLocationAsync(int userId);

        Task<List<Locations>> GetCurrentUserRegionsListAsync(LocationRequest currentPosition);

        Task AddUserLocationAsync(UserLocation location);

        Task UpdateUserLocationAsync(UserLocation userLocation);

    }
}
