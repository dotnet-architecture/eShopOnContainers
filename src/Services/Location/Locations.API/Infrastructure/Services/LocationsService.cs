namespace Microsoft.eShopOnContainers.Services.Locations.API.Infrastructure.Services
{
    using Microsoft.eShopOnContainers.Services.Locations.API.Infrastructure.Repositories;
    using Microsoft.eShopOnContainers.Services.Locations.API.ViewModel;
    using Microsoft.eShopOnContainers.Services.Locations.API.Model;
    using System;
    using System.Threading.Tasks;

    public class LocationsService : ILocationsService
    {
        private ILocationsRepository _locationsRepository;

        public LocationsService(ILocationsRepository locationsRepository)
        {
            _locationsRepository = locationsRepository ?? throw new ArgumentNullException(nameof(locationsRepository));
        }

        public async Task<bool> AddOrUpdateUserLocation(string id, LocationRequest currentPosition)
        {
            int.TryParse(id, out int userId);
            var currentUserLocation = await _locationsRepository.GetAsync(userId);
            
            // Get the nearest locations ordered by proximity 
            var nearestLocationList = await _locationsRepository.GetNearestLocationListAsync(currentPosition.Latitude, currentPosition.Longitude);

            // Check out in which region we currently are 
            foreach(var locCandidate in nearestLocationList)
            {
                // Check location's tree and retrive user most specific area
                var findNewLocationResult = locCandidate.GetUserMostSpecificLocation(currentPosition.Latitude, currentPosition.Longitude);
                if (findNewLocationResult.isSuccess)
                {
                    CreateUserLocation(currentUserLocation, findNewLocationResult.location, userId);
                    UpdateUserLocation(currentUserLocation, findNewLocationResult.location);
                    break;
                }
            }
            
            var result = await _locationsRepository.UnitOfWork.SaveChangesAsync();
            return result > 0;
        }

        private void CreateUserLocation(UserLocation currentUserLocation, Locations newLocation, int userId)
        {
            if (currentUserLocation is null)
            {
                currentUserLocation = currentUserLocation ?? new UserLocation(userId);
                currentUserLocation.Location = newLocation;
                _locationsRepository.Add(currentUserLocation);
            }           
        }

        private void UpdateUserLocation(UserLocation currentUserLocation, Locations newLocation)
        {
            if (currentUserLocation != null)
            {
                currentUserLocation.Location = newLocation;
                _locationsRepository.Update(currentUserLocation);
            }            
        }
    }
}
