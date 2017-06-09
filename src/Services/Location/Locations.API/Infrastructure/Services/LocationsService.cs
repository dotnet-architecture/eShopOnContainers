namespace Microsoft.eShopOnContainers.Services.Locations.API.Infrastructure.Services
{
    using Microsoft.eShopOnContainers.Services.Locations.API.Infrastructure.Repositories;
    using Microsoft.eShopOnContainers.Services.Locations.API.ViewModel;
    using Microsoft.eShopOnContainers.Services.Locations.API.Model;
    using System;
    using System.Threading.Tasks;
    using System.Linq;
    using Microsoft.eShopOnContainers.Services.Locations.API.Infrastructure.Exceptions;
    using System.Collections.Generic;

    public class LocationsService : ILocationsService
    {
        private ILocationsRepository _locationsRepository;

        public LocationsService(ILocationsRepository locationsRepository)
        {
            _locationsRepository = locationsRepository ?? throw new ArgumentNullException(nameof(locationsRepository));
        }

        public async Task<Locations> GetLocation(string locationId)
        {
            return await _locationsRepository.GetAsync(locationId);
        }

        public async Task<UserLocation> GetUserLocation(int id)
        {
            return await _locationsRepository.GetUserLocationAsync(id);
        }

        public async Task<List<Locations>> GetAllLocation()
        {
            return await _locationsRepository.GetLocationListAsync();
        }

        public async Task<bool> AddOrUpdateUserLocation(string id, LocationRequest currentPosition)
        {
            if (!int.TryParse(id, out int userId))
            {
                throw new ArgumentException("Not valid userId");
            }

            // Get the list of ordered regions the user currently is within
            var currentUserAreaLocationList = await _locationsRepository.GetCurrentUserRegionsListAsync(currentPosition);
                      
            if(currentUserAreaLocationList is null)
            {
                throw new LocationDomainException("User current area not found");
            }

            // If current area found, then update user location
            var locationAncestors = new List<string>();
            var userLocation = await _locationsRepository.GetUserLocationAsync(userId);
            userLocation = userLocation ?? new UserLocation();
            userLocation.UserId = userId;
            userLocation.LocationId = currentUserAreaLocationList[0].Id;
            userLocation.UpdateDate = DateTime.UtcNow;
            await _locationsRepository.UpdateUserLocationAsync(userLocation);

            return true;
        }
    }
}
