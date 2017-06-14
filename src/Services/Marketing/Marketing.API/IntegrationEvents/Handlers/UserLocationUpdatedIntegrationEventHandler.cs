namespace Microsoft.eShopOnContainers.Services.Marketing.API.IntegrationEvents.Handlers
{
    using BuildingBlocks.EventBus.Abstractions;
    using System.Threading.Tasks;
    using Events;
    using System;
    using Infrastructure.Repositories;
    using Model;
    using System.Collections.Generic;

    public class UserLocationUpdatedIntegrationEventHandler 
        : IIntegrationEventHandler<UserLocationUpdatedIntegrationEvent>
    {
        private readonly IMarketingDataRepository _marketingDataRepository;

        public UserLocationUpdatedIntegrationEventHandler(IMarketingDataRepository repository)
        {
            _marketingDataRepository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task Handle(UserLocationUpdatedIntegrationEvent @event)
        {
            var userMarketingData = await _marketingDataRepository.GetAsync(@event.UserId);
            userMarketingData = userMarketingData ?? 
                new MarketingData() { UserId = @event.UserId };

            userMarketingData.Locations = MapUpdatedUserLocations(@event.LocationList);
            await _marketingDataRepository.UpdateLocationAsync(userMarketingData);
        }

        private List<Location> MapUpdatedUserLocations(List<UserLocationDetails> newUserLocations)
        {
            var result = new List<Location>();
            newUserLocations.ForEach(location => {
                result.Add(new Location()
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
