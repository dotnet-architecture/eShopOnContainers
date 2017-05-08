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
        private readonly IConfirmGracePeriodEvent _confirmGracePeriodEvent;

        public SagaManagerService(IOptions<SagaManagerSettings> settings,
            IConfirmGracePeriodEvent confirmGracePeriodEvent)
        {
            _settings = settings.Value;
            _confirmGracePeriodEvent = confirmGracePeriodEvent;
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
                conn.Open();
                orderIds = conn.Query<int>(
                    @"SELECT Id FROM [Microsoft.eShopOnContainers.Services.OrderingDb].[ordering].[orders] 
                        WHERE DATEDIFF(hour, [OrderDate], GETDATE()) >= @GracePeriod
                        AND [OrderStatusId] = 1",
                    new { GracePeriod = _settings.GracePeriod });
            }

            return orderIds;
        }

        private void Publish(int orderId)
        {
            var confirmGracePeriodEvent = new ConfirmGracePeriodIntegrationEvent(orderId);

            // Publish through the Event Bus
           _confirmGracePeriodEvent.PublishThroughEventBus(confirmGracePeriodEvent);
        }
    }
}