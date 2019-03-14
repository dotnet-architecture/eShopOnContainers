using Microsoft.EntityFrameworkCore.Storage; 
using Microsoft.eShopOnContainers.Services.Ordering.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Ordering.API.Extensions;

namespace Ordering.API.Application.IntegrationEvents
{
    public class OrderingIntegrationEventService : IOrderingIntegrationEventService
    {
        private readonly ICapPublisher _eventBus;
        private readonly OrderingContext _orderingContext;
        private readonly ILogger<OrderingIntegrationEventService> _logger;

        public OrderingIntegrationEventService(ICapPublisher eventBus,
            OrderingContext orderingContext,
            ILogger<OrderingIntegrationEventService> logger)
        {
            _orderingContext = orderingContext ?? throw new ArgumentNullException(nameof(orderingContext));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task AddAndSaveEventAsync(object evt)
        {
            _logger.LogInformation("----- Enqueuing integration event to repository ({@IntegrationEvent})", evt);

            _eventBus.Transaction.Begin(_orderingContext.GetCurrentTransaction.GetDbTransaction());
            await _eventBus.PublishAsync(evt.GetGenericTypeName(), evt);
        }
    }
}
