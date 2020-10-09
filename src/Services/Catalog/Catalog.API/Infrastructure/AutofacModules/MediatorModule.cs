using Autofac;
using MediatR;
using Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure.Commands;
using System.Reflection;

namespace Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure.AutofacModules
{
    public class MediatorModule: Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
                .AsImplementedInterfaces();

            builder.Register<ServiceFactory>(context =>
            {
                var componentContext = context.Resolve<IComponentContext>();
                return t => { object o; return componentContext.TryResolve(t, out o) ? o : null; };
            });

            builder.RegisterAssemblyTypes(typeof(UpdatePicCommand).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IRequestHandler<,>));
        }
    }
}
