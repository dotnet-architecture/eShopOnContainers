using Autofac;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using System.Reflection;
using TenantACustomisations.ExternalServices;
using TenantACustomisations.IntegrationEvents.Events;

namespace Microsoft.eShopOnContainers.Services.TenantACustomisations.Infrastructure.AutofacModules
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
            /*builder.RegisterAssemblyTypes(typeof(UserCheckoutAcceptedIntegrationEvent).GetTypeInfo().Assembly)
                        .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));*/
            
        }
    }
}
