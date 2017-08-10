namespace Microsoft.eShopOnContainers.Services.Locations.API.Infrastructure.Services
{
    using Microsoft.eShopOnContainers.Services.Locations.API.Model;
    using Microsoft.eShopOnContainers.Services.Locations.API.ViewModel;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ILocationsService
    {
        Task<Locations> GetLocation(int locationId);

        Task<UserLocation> GetUserLocation(string id);

        Task<List<Locations>> GetAllLocation();

        Task<bool> AddOrUpdateUserLocation(string userId, LocationRequest locRequest);
    }
}
