namespace Microsoft.eShopOnContainers.Services.Locations.API.Infrastructure.Services
{
    using Microsoft.eShopOnContainers.Services.Locations.API.ViewModel;
    using System.Threading.Tasks;

    public interface ILocationsService
    {
        Task<bool> AddOrUpdateUserLocation(string userId, LocationRequest locRequest);
    }
}
