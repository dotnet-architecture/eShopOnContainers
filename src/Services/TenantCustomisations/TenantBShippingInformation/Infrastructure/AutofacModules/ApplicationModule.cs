using Autofac;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using System.Reflection;
using TenantAShippingInformation.IntegrationEvents.EventHandling;

namespace TenantAShippingInformation.Infrastructure.AutofacModules
{

    public class ApplicationModule
        :Autofac.Module
    {

        public string QueriesConnectionString { get; }

        public ApplicationModule(string qconstr)
        {
            QueriesConnectionString = qconstr;

        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(OrderStatusChangedToAwaitingValidationIntegrationEventHandler).GetTypeInfo().Assembly)
                        .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));
            
        }
    }
}
