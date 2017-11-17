namespace Ordering.BackgroundTasksWebHost.HostedServices
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Threading;
    using System.Threading.Tasks;

    //using Dapper;
    //using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
    //using Microsoft.eShopOnContainers.Services.Ordering.API;
    //using Ordering.API.Application.IntegrationEvents.Events;

    public class GracePeriodManagerService 
           : BackgroundService
    {        
        private readonly ILogger<GracePeriodManagerService> _logger;

        private readonly OrderingBackgroundSettings _settings;

        //private readonly IEventBus _eventBus;

        public GracePeriodManagerService(IOptions<OrderingBackgroundSettings> settings,
                                         //IEventBus eventBus,
                                         ILogger<GracePeriodManagerService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            //_eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));

            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug($"GracePeriodManagerService is starting.");

            stoppingToken.Register(() => _logger.LogDebug($"#1 GracePeriodManagerService background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug($"GracePeriodManagerService background task is doing background work.");

                //CheckConfirmedGracePeriodOrders();

                await Task.Delay(_settings.CheckUpdateTime, stoppingToken);
            }
            
            _logger.LogDebug($"GracePeriodManagerService background task is stopping.");

            await Task.CompletedTask;
        }

        //private void CheckConfirmedGracePeriodOrders()
        //{
        //    _logger.LogDebug($"Checking confirmed grace period orders");

        //    var orderIds = GetConfirmedGracePeriodOrders();

        //    foreach (var orderId in orderIds)
        //    {
        //        var confirmGracePeriodEvent = new GracePeriodConfirmedIntegrationEvent(orderId);
        //        _eventBus.Publish(confirmGracePeriodEvent);
        //    }
        //}

        //private IEnumerable<int> GetConfirmedGracePeriodOrders()
        //{
        //    IEnumerable<int> orderIds = new List<int>();

        //    using (var conn = new SqlConnection(_settings.ConnectionString))
        //    {
        //        try
        //        {
        //            conn.Open();
        //            orderIds = conn.Query<int>(
        //                @"SELECT Id FROM [ordering].[orders] 
        //                    WHERE DATEDIFF(minute, [OrderDate], GETDATE()) >= @GracePeriodTime
        //                    AND [OrderStatusId] = 1",
        //                new { GracePeriodTime = _settings.GracePeriodTime });
        //        }
        //        catch (SqlException exception)
        //        {
        //            _logger.LogCritical($"FATAL ERROR: Database connections could not be opened: {exception.Message}");
        //        }

        //    }

        //    return orderIds;
        //}
    }
}
