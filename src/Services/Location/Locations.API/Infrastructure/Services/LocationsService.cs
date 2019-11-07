namespace Microsoft.eShopOnContainers.Services.Locations.API.Infrastructure.Services
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
    using Microsoft.eShopOnContainers.Services.Locations.API.Infrastructure.Exceptions;
    using Microsoft.eShopOnContainers.Services.Locations.API.Infrastructure.Repositories;
    using Microsoft.eShopOnContainers.Services.Locations.API.IntegrationEvents.Events;
    using Microsoft.eShopOnContainers.Services.Locations.API.Model;
    using Microsoft.eShopOnContainers.Services.Locations.API.ViewModel;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class LocationsService : ILocationsService
    {
        private readonly ILocationsRepository _locationsRepository;
        private readonly IEventBus _eventBus;
        private readonly ILogger<LocationsService> _logger;

        public LocationsService(
            ILocationsRepository locationsRepository,
            IEventBus eventBus,
            ILogger<LocationsService> logger)
        {
            _locationsRepository = locationsRepository ?? throw new ArgumentNullException(nameof(locationsRepository));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Locations> GetLocationAsync(int locationId)
        {
            return await _locationsRepository.GetAsync(locationId);
        }

        public async Task<UserLocation> GetUserLocationAsync(string userId)
        {
            return await _locationsRepository.GetUserLocationAsync(userId);
        }

        public async Task<List<Locations>> GetAllLocationAsync()
        {
            return await _locationsRepository.GetLocationListAsync();
        }

        public async Task<bool> AddOrUpdateUserLocationAsync(string userId, LocationRequest currentPosition)
        {
            // Get the list of ordered regions the user currently is within
            var currentUserAreaLocationList = await _locationsRepository.GetCurrentUserRegionsListAsync(currentPosition);

            if (currentUserAreaLocationList is null)
            {
                throw new LocationDomainException("User current area not found");
            }

            // If current area found, then update user location
            var locationAncestors = new List<string>();
            var userLocation = await _locationsRepository.GetUserLocationAsync(userId);
            userLocation = userLocation ?? new UserLocation();
            userLocation.UserId = userId;
            userLocation.LocationId = currentUserAreaLocationList[0].LocationId;
            userLocation.UpdateDate = DateTime.UtcNow;
            await _locationsRepository.UpdateUserLocationAsync(userLocation);

            // Publish integration event to update marketing read data model
            // with the new locations updated
            PublishNewUserLocationPositionIntegrationEvent(userId, currentUserAreaLocationList);

            return true;
        }

        private void PublishNewUserLocationPositionIntegrationEvent(string userId, List<Locations> newLocations)
        {
            var newUserLocations = MapUserLocationDetails(newLocations);
            var @event = new UserLocationUpdatedIntegrationEvent(userId, newUserLocations);

            _logger.LogInformation("----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

            _eventBus.Publish(@event);
        }

        private List<UserLocationDetails> MapUserLocationDetails(List<Locations> newLocations)
        {
            var result = new List<UserLocationDetails>();
            newLocations.ForEach(location =>
            {
                result.Add(new UserLocationDetails()
                {
                    LocationId = location.LocationId,
                    Code = location.Code,
                    Description = location.Description
                });
            });

            return result;
        }
    }
}
