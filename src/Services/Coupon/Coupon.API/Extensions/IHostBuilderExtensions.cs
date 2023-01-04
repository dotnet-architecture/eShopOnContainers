using System;
using Microsoft.Data.SqlClient;
using Coupon.API.IntegrationEvents.EventHandlers;
using Coupon.API.IntegrationEvents.Events;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;

namespace Coupon.API.Extensions
{
    public static class IHostBuilderExtensions
    {
        public static IHost SeedDatabaseStrategy<TContext>(this IHost host, Action<TContext> seeder)
        {
            using (var scope = host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<TContext>();

                var policy = Policy.Handle<SqlException>()
                    .WaitAndRetry(new TimeSpan[]
                    {
                        TimeSpan.FromSeconds(3),
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(8),
                    });

                policy.Execute(() =>
                {
                    seeder.Invoke(context);
                });
            }

            return host;
        }

        public static IHost SubscribersIntegrationEvents(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();

                eventBus.Subscribe<OrderStatusChangedToAwaitingCouponValidationIntegrationEvent, OrderStatusChangedToAwaitingCouponValidationIntegrationEventHandler>();
            }

            return host;
        }
    }
}
