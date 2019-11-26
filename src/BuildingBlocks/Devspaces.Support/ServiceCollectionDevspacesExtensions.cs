using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Devspaces.Support
{
    public static class ServiceCollectionDevspacesExtensions
    {
        public static IServiceCollection AddDevspaces(this IServiceCollection services)
        {
            services.AddTransient<DevspacesMessageHandler>();
            return services;
        }
    }
}
