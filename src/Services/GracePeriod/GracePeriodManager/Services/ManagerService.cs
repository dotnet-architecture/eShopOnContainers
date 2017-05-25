namespace GracePeriodManager.Services
{
    using Dapper;
    using GracePeriodManager.IntegrationEvents.Events;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System.Collections.Generic;
    using System.Data.SqlClient;

    public class ManagerService : IManagerService
    {
        private readonly ManagerSettings _settings;
        private readonly IEventBus _eventBus;
        private readonly ILogger<ManagerService> _logger;

        public ManagerService(IOptions<ManagerSettings> settings,
            IEventBus eventBus,
            ILogger<ManagerService> logger)
        {
            _settings = settings.Value;
            _eventBus = eventBus;
            _logger = logger;
        }

        public void CheckConfirmedGracePeriodOrders()
        {
            var orderIds = GetConfirmedGracePeriodOrders();

            foreach (var orderId in orderIds)
            {
                var confirmGracePeriodEvent = new GracePeriodConfirmedIntegrationEvent(orderId);
                _eventBus.Publish(confirmGracePeriodEvent);
            }
        }

        private IEnumerable<int> GetConfirmedGracePeriodOrders()
        {
            IEnumerable<int> orderIds = new List<int>();
            using (var conn = new SqlConnection(_settings.ConnectionString))
            {
                try
                {
                    _logger.LogInformation("Grace Period Manager Client is trying to connect to database server");
                    conn.Open();
                    orderIds = conn.Query<int>(
                        @"SELECT Id FROM [Microsoft.eShopOnContainers.Services.OrderingDb].[ordering].[orders] 
                            WHERE DATEDIFF(minute, [OrderDate], GETDATE()) >= @GracePeriodTime
                            AND [OrderStatusId] = 1",
                        new { GracePeriodTime = _settings.GracePeriodTime });  
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