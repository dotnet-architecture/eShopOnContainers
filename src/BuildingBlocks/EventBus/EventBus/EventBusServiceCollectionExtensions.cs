using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBus
{
    public static class EventBusServiceCollectionExtensions
    {
        public static IServiceCollection AddEventHandlers(this IServiceCollection services, System.Reflection.Assembly assembly)
        {
            foreach (var typeInfo in assembly.GetTypes()
               .Where(t => typeof(IIntegrationEventHandler).IsAssignableFrom(t)
                    && t.IsClass
                    && !t.IsAbstract
                    && !t.ContainsGenericParameters
                    && t.GetConstructors(BindingFlags.Public | BindingFlags.Instance)?.Length != 0))
            {
                services.AddScoped(typeInfo);
            }

            return services;
        }
    }
}
