using Basket.API.IntegrationEvents.Events;
using Microsoft.eShopOnContainers.Services.Basket.API;
using Microsoft.eShopOnContainers.Services.Basket.API.Model;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Threading.Tasks;
using DotNetCore.CAP;

namespace Basket.API.IntegrationEvents.EventHandling
{
    public class OrderStartedIntegrationEventHandler : ICapSubscribe
    {
        private readonly IBasketRepository _repository;
        private readonly ILogger<OrderStartedIntegrationEventHandler> _logger;

        public OrderStartedIntegrationEventHandler(
            IBasketRepository repository,
            ILogger<OrderStartedIntegrationEventHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [CapSubscribe(nameof(OrderStartedIntegrationEvent))] 
        public async Task Handle(OrderStartedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling integration event: {AppName} - ({@IntegrationEvent})", Program.AppName, @event);

                await _repository.DeleteBasketAsync(@event.UserId.ToString());
            }
        }
    }
}