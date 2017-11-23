namespace Ordering.API.Infrastructure.HostedServices
{
    using Dapper;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
    using Microsoft.eShopOnContainers.Services.Ordering.API;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Ordering.API.Application.IntegrationEvents.Events;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Threading;
    using System.Threading.Tasks;

    public class GracePeriodManagerService : BackgroundService
    {
        private readonly OrderingSettings _settings;
        private readonly ILogger<GracePeriodManagerService> _logger;
        private readonly IEventBus _eventBus;

        public GracePeriodManagerService(IOptions<OrderingSettings> settings,
            IEventBus eventBus,
            ILogger<GracePeriodManagerService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug($"GracePeriod background task is starting.");

            stoppingToken.Register(() => _logger.LogDebug($"#1 GracePeriod background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug($"GracePeriod background task is doing background work.");

                CheckConfirmedGracePeriodOrders();

                await Task.Delay(_settings.CheckUpdateTime, stoppingToken);

                continue;
            }

            _logger.LogDebug($"GracePeriod background task is stopping.");
            
        }

        private void CheckConfirmedGracePeriodOrders()
        {
            _logger.LogDebug($"Checking confirmed grace period orders");

            var orderIds = GetConfirmedGracePeriodOrders();

            _logger.LogDebug($"GracePeriod sent a .");
            foreach (var orderId in orderIds)
            {
                var gracePeriodConfirmedEvent = new GracePeriodConfirmedIntegrationEvent(orderId);
                _eventBus.Publish(gracePeriodConfirmedEvent);
            }
        }

        private IEnumerable<int> GetConfirmedGracePeriodOrders()
        {
            IEnumerable<int> orderIds = new List<int>();

            using (var conn = new SqlConnection(_settings.ConnectionString))
            {
                try
                {
                    conn.Open();
                    orderIds = conn.Query<int>(
                        @"SELECT Id FROM [ordering].[orders] 
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
