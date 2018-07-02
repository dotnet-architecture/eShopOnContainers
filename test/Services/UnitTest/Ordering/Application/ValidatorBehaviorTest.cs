using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;
using Moq;
using Ordering.API.Infrastructure.Behaviors;
using Ordering.Domain.Exceptions;
using Xunit;

namespace UnitTest.Ordering.Application
{
    public class ValidatorBehaviorTest
    {
        private ContainerBuilder PrepareContainerBuilder()
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
                .AsImplementedInterfaces();

            builder.Register<ServiceFactory>(context =>
            {
                var componentContext = context.Resolve<IComponentContext>();
                return t => { object o; return componentContext.TryResolve(t, out o) ? o : null; };
            });

            builder.RegisterGeneric(typeof(ValidatorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            return builder;
        }

        [Fact]
        public async Task Handler_throws_when_validation_fails()
        {
            //Arrange
            var command = new CreateOrderCommand();
            var validator = new Mock<IValidator<CreateOrderCommand>>();
            validator.Setup(x => x.Validate(It.IsAny<CreateOrderCommand>()))
                .Returns(new ValidationResult(new[] {new ValidationFailure("property", "error")}));
            
            var builder = PrepareContainerBuilder();
            builder.RegisterInstance(validator.Object).AsImplementedInterfaces();
            var container = builder.Build();

            var mediator = container.Resolve<IMediator>();
            
            //Assert
            await Assert.ThrowsAsync<OrderingDomainException>(async () => await mediator.Send(command));
        }
    }
}