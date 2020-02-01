using Dapper;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ordering.BackgroundTasks.Configuration;
using Ordering.BackgroundTasks.IntegrationEvents;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Ordering.BackgroundTasks.Tasks
{
    public class GracePeriodManagerService
        : BackgroundService
    {
        private readonly ILogger<GracePeriodManagerService> _logger;
        private readonly BackgroundTaskSettings _settings;
        private readonly IEventBus _eventBus;
        private static readonly String identityUrl = @"http://identity.api/";

        public GracePeriodManagerService(
            IOptions<BackgroundTaskSettings> settings,
            IEventBus eventBus,
            ILogger<GracePeriodManagerService> logger)
        {
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("GracePeriodManagerService is starting.");

            stoppingToken.Register(() => _logger.LogDebug("#1 GracePeriodManagerService background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug("GracePeriodManagerService background task is doing background work.");

                CheckConfirmedGracePeriodOrders();

                await Task.Delay(_settings.CheckUpdateTime, stoppingToken);
            }

            _logger.LogInformation("GracePeriodManagerService background task is stopping.");

            await Task.CompletedTask;
        }

        private void CheckConfirmedGracePeriodOrders()
        {
            _logger.LogInformation("Checking confirmed grace period orders");

                var orderIds = GetConfirmedGracePeriodOrders();

            foreach (var orderId in orderIds)
            {
                var confirmGracePeriodEvent = new GracePeriodConfirmedIntegrationEvent(orderId);
                String userName = GetUserName(orderId);
                int tenantId = GetTenantId(userName).Result;
                confirmGracePeriodEvent.TenantId = tenantId;

                _logger.LogInformation(
                    "----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})",
                    confirmGracePeriodEvent.Id, Program.AppName, confirmGracePeriodEvent);

                _eventBus.Publish(confirmGracePeriodEvent);
            }
        }

        private async Task<int> GetTenantId(String userName)
        {
            var builder = new UriBuilder(identityUrl + "api/userid");
            builder.Port = -1;
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["userName"] = userName;
            builder.Query = query.ToString();
            string url = builder.ToString();

            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(url);
                    string result = response.Content.ReadAsStringAsync().Result;

                    return Int32.Parse(result);
                }
                catch (Exception e)
                {
                    return 0;
                }
            }
        }

        private String GetUserName(int orderId)
        {
            String username = "";
            using (var conn = new SqlConnection(_settings.ConnectionString))
            {
                try
                {
                    conn.Open();
                    username = conn.QueryFirst<String>(
                        @"SELECT Name FROM [ordering].[orders] 
                                LEFT JOIN [ordering].buyers
                                ON [ordering].orders.BuyerId = [ordering].buyers.Id
                                WHERE [ordering].orders.Id = @OrderId",
                        new {OrderId = orderId});
                }
                catch (SqlException exception)
                {
                    _logger.LogCritical(exception, "FATAL ERROR: Database connections could not be opened: {Message}",
                        exception.Message);
                }
            }

            return username;
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
                        new {GracePeriodTime = _settings.GracePeriodTime});
                }
                catch (SqlException exception)
                {
                    _logger.LogCritical(exception, "FATAL ERROR: Database connections could not be opened: {Message}",
                        exception.Message);
                }
            }

            return orderIds;
        }
    }
}