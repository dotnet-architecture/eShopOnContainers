using Microsoft.AspNetCore.Builder;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.Builder
{
    public static class EventBusBuilderExtensions
    {
        public static IApplicationBuilder UseEventBus(this IApplicationBuilder builder, Action<IEventBus> configure = null)
        {

            var eventBus = builder.ApplicationServices.GetRequiredService<IEventBus>();

            configure?.Invoke(eventBus);

            eventBus.Start();

            return builder;

        }
    }
}
