namespace Microsoft.eShopOnContainers.Services.Locations.API.Infrastructure.Services
{
    using Microsoft.eShopOnContainers.Services.Locations.API.Model;
    using Microsoft.eShopOnContainers.Services.Locations.API.ViewModel;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ILocationsService
    {
        Task<Locations> GetLocationAsync(int locationId);

        Task<UserLocation> GetUserLocationAsync(string id);

        Task<List<Locations>> GetAllLocationAsync();

        Task<bool> AddOrUpdateUserLocationAsync(string userId, LocationRequest locRequest);
    }
}
