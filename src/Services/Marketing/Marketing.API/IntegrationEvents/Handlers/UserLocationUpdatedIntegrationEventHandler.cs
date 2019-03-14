using DotNetCore.CAP;

namespace Microsoft.eShopOnContainers.Services.Marketing.API.IntegrationEvents.Handlers
{
    using Marketing.API.IntegrationEvents.Events;
    using Marketing.API.Model;
    using Microsoft.eShopOnContainers.Services.Marketing.API.Infrastructure.Repositories;
    using Microsoft.Extensions.Logging;
    using Serilog.Context;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class UserLocationUpdatedIntegrationEventHandler : ICapSubscribe
    {
        private readonly IMarketingDataRepository _marketingDataRepository;
        private readonly ILogger<UserLocationUpdatedIntegrationEventHandler> _logger;

        public UserLocationUpdatedIntegrationEventHandler(
            IMarketingDataRepository repository,
            ILogger<UserLocationUpdatedIntegrationEventHandler> logger)
        {
            _marketingDataRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        //TODO: [CapSubscribe(nameof(UserLocationUpdatedIntegrationEvent))]
        public async Task Handle(UserLocationUpdatedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling integration event: {AppName} - ({@IntegrationEvent})", Program.AppName, @event);

                var userMarketingData = await _marketingDataRepository.GetAsync(@event.UserId);
                userMarketingData = userMarketingData ??
                    new MarketingData() { UserId = @event.UserId };

                userMarketingData.Locations = MapUpdatedUserLocations(@event.LocationList);
                await _marketingDataRepository.UpdateLocationAsync(userMarketingData);
            }
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
