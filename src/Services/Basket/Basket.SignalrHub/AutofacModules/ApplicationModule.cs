using Autofac;
using Basket.SignalrHub.IntegrationEvents;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Basket.SignalrHub.AutofacModules
{
    public class ApplicationModule
        : Autofac.Module
    {

        public string QueriesConnectionString { get; }

        public ApplicationModule()
        {
        }

        protected override void Load(ContainerBuilder builder)
        {
            // New integration event registrations go here

            // builder.RegisterAssemblyTypes(typeof(NewIntegrationEvent).GetTypeInfo().Assembly)
            //    .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));
            builder.RegisterAssemblyTypes(typeof(UserAddedCartItemToBasketIntegrationEvent).GetTypeInfo().Assembly)
             .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));
        }
    }
}
