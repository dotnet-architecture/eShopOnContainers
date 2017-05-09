using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SagaManager.Services
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using Microsoft.Extensions.Options;
    using Dapper;
    using IntegrationEvents;
    using IntegrationEvents.Events;

    public class SagaManagerService : ISagaManagerService
    {
        private readonly SagaManagerSettings _settings;
        private readonly ISagaManagingIntegrationEventService _sagaManagingIntegrationEventService;
        private readonly ILogger<SagaManagerService> _logger;

        public SagaManagerService(IOptions<SagaManagerSettings> settings,
            ISagaManagingIntegrationEventService sagaManagingIntegrationEventService,
            ILogger<SagaManagerService> logger)
        {
            _settings = settings.Value;
            _sagaManagingIntegrationEventService = sagaManagingIntegrationEventService;
            _logger = logger;
        }

        public void CheckFinishedGracePeriodOrders()
        {
            var orderIds = GetFinishedGracePeriodOrders();

            foreach (var orderId in orderIds)
            {
                Publish(orderId);
            }
        }

        private IEnumerable<int> GetFinishedGracePeriodOrders()
        {
            IEnumerable<int> orderIds = new List<int>();
            using (var conn = new SqlConnection(_settings.ConnectionString))
            {
                try
                {
                    conn.Open();
                    orderIds = conn.Query<int>(
                        @"SELECT Id FROM [Microsoft.eShopOnContainers.Services.OrderingDb].[ordering].[orders] 
                            WHERE DATEDIFF(hour, [OrderDate], GETDATE()) >= @GracePeriod
                            AND [OrderStatusId] = 1",
                        new { GracePeriod = _settings.GracePeriod });  
                }
                catch (SqlException exception)
                {
                    _logger.LogError(exception.Message);
                }
                
            }

            return orderIds;
        }

        private async Task Publish(int orderId)
        {
            var confirmGracePeriodEvent = new ConfirmGracePeriodCommandMsg(orderId);

            // Publish through the Event Bus
            await _sagaManagingIntegrationEventService.PublishThroughEventBusAsync(confirmGracePeriodEvent);
        }
    }
}