namespace Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure.AutofacModules
{
    using Application.Commands;
    using Application.Decorators;
    using Autofac;
    using Autofac.Core;
    using MediatR;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class MediatorModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(typeof(NewOrderRequest).GetTypeInfo().Assembly)
                .As(o => o.GetInterfaces()
                    .Where(i => i.IsClosedTypeOf(typeof(IAsyncRequestHandler<,>)))
                    .Select(i => new KeyedService("IAsyncRequestHandler", i)));

            builder.Register<SingleInstanceFactory>(context =>
            {
                var componentContext = context.Resolve<IComponentContext>();

                return t => componentContext.Resolve(t);
            });

            builder.Register<MultiInstanceFactory>(context =>
            {
                var componentContext = context.Resolve<IComponentContext>();

                return t => (IEnumerable<object>)componentContext.Resolve(typeof(IEnumerable<>).MakeGenericType(t));
            });

            builder.RegisterGenericDecorator(typeof(LogDecorator<,>),
                    typeof(IAsyncRequestHandler<,>),
                    "IAsyncRequestHandler");
        }
    }
}
