using Autofac;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using System.Reflection;
using TenantAShippingInformation.ExternalServices;
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
            builder.RegisterAssemblyTypes(typeof(OrderStatusChangedToSubmittedIntegrationEventHandler).GetTypeInfo().Assembly)
                        .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));
            
            builder.RegisterType<MockedShippingService>()
                .As<IShippingService>()
                .InstancePerLifetimeScope();
        }
    }
}
