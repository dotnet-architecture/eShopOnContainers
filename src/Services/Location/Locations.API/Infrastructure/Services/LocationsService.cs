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
            Locations currentUserAreaLocation = null;

            if (!int.TryParse(id, out int userId))
            {
                throw new ArgumentException("Not valid userId");
            }

            // Get the nearest locations ordered
            var nearestLocationList = await _locationsRepository.GetNearestLocationListAsync(currentPosition.Latitude, currentPosition.Longitude);

            // Check out in which region we currently are 
            foreach(var locationCandidate in nearestLocationList.Where(x=> x.Polygon != null))
            {
                currentUserAreaLocation = await _locationsRepository.GetLocationByCurrentAreaAsync(locationCandidate);
                if(currentUserAreaLocation != null) { break; }
            }
            
            if(currentUserAreaLocation is null)
            {
                throw new LocationDomainException("User current area not found");
            }

            // If current area found, then update user location
            if(currentUserAreaLocation != null)
            {
                var locationAncestors = new List<string>();
                var userLocation = await _locationsRepository.GetUserLocationAsync(userId);
                userLocation = userLocation ?? new UserLocation();
                userLocation.UserId = userId;
                userLocation.LocationId = currentUserAreaLocation.Id;
                userLocation.UpdateDate = DateTime.UtcNow;
                await _locationsRepository.UpdateUserLocationAsync(userLocation);
            }
            
            return true;
        }
    }
}
