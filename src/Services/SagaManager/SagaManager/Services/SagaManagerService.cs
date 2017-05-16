namespace SagaManager.Services
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Logging;
    using Dapper;
    using IntegrationEvents;
    using IntegrationEvents.Events;

    public class SagaManagerService : ISagaManagerService
    {
        private readonly SagaManagerSettings _settings;
        private readonly ISagaManagerIntegrationEventService _sagaManagerIntegrationEventService;
        private readonly ILogger<SagaManagerService> _logger;

        public SagaManagerService(IOptions<SagaManagerSettings> settings,
            ISagaManagerIntegrationEventService sagaManagerIntegrationEventService,
            ILogger<SagaManagerService> logger)
        {
            _settings = settings.Value;
            _sagaManagerIntegrationEventService = sagaManagerIntegrationEventService;
            _logger = logger;
        }

        public void CheckFinishedGracePeriodOrders()
        {
            var orderIds = GetFinishedGracePeriodOrders();

            foreach (var orderId in orderIds)
            {
                var confirmGracePeriodEvent = new ConfirmGracePeriodCommandMsg(orderId);

                _sagaManagerIntegrationEventService.PublishThroughEventBus(confirmGracePeriodEvent);
            }
        }

        private IEnumerable<int> GetFinishedGracePeriodOrders()
        {
            IEnumerable<int> orderIds = new List<int>();
            using (var conn = new SqlConnection(_settings.ConnectionString))
            {
                try
                {
                    _logger.LogInformation("SagaManager Client is trying to connect to database server");
                    conn.Open();
                    orderIds = conn.Query<int>(
                        @"SELECT Id FROM [Microsoft.eShopOnContainers.Services.OrderingDb].[ordering].[orders] 
                            WHERE DATEDIFF(hour, [OrderDate], GETDATE()) >= @GracePeriod
                            AND [OrderStatusId] = 1",
                        new { GracePeriod = _settings.GracePeriod });  
                }
                catch (SqlException exception)
                {
                    _logger.LogCritical($"FATAL ERROR: Database connections could not be opened: {exception.Message}");
                }
                
            }

            return orderIds;
        }
    }
}