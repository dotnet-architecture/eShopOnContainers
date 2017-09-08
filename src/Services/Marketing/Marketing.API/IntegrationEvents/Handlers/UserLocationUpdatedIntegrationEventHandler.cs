namespace Microsoft.eShopOnContainers.Services.Marketing.API.IntegrationEvents.Handlers
{
    using Marketing.API.IntegrationEvents.Events;
    using Marketing.API.Model;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
    using Microsoft.eShopOnContainers.Services.Marketing.API.Infrastructure.Repositories;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

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
