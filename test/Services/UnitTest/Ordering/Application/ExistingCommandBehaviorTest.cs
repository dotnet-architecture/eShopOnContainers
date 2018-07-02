using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using MediatR;
using Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;
using Microsoft.eShopOnContainers.Services.Ordering.Infrastructure.Idempotency;
using Moq;
using Ordering.API.Infrastructure.Behaviors;
using Xunit;

namespace UnitTest.Ordering.Application
{
    public class ExistingCommandBehaviorTest
    {
        private readonly Mock<IRequestManager> _requestManager;
        private readonly Mock<IRequestHandler<CreateOrderCommand, bool>> _commandHandler;

        public ExistingCommandBehaviorTest()
        {
            _requestManager = new Mock<IRequestManager>();
            _commandHandler = new Mock<IRequestHandler<CreateOrderCommand, bool>>();
        }

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
           
            builder.RegisterInstance(_requestManager.Object).AsImplementedInterfaces();
            builder.RegisterInstance(_commandHandler.Object).AsImplementedInterfaces();

            builder.RegisterGeneric(typeof(ExistingCommandBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            return builder;
        }

        [Fact]
        public async Task Handler_sends_command_when_no_order_exists()
        {
            //Arrange
            var command = new CreateOrderCommand();
            _requestManager.Setup(x => x.ExistAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(false));
            
            var builder = PrepareContainerBuilder();
            var container = builder.Build();

            var mediator = container.Resolve<IMediator>();

            //Act
            await mediator.Send(command);

            //Assert
            _commandHandler.Verify(x => x.Handle(It.IsAny<CreateOrderCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handler_sends_no_command_and_returns_defined_response_when_order_already_exists()
        {
            //Arrange
            var command = new CreateOrderCommand();
            var existingCommandResponse = new Mock<IRequestHandler<ExistingCommandResponse<CreateOrderCommand, bool>.ExistingCommand, bool>>();
            existingCommandResponse.Setup(x => x.Handle(It.IsAny<ExistingCommandResponse<CreateOrderCommand, bool>.ExistingCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));

            _requestManager.Setup(x => x.ExistAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(true));

            var builder = PrepareContainerBuilder();
            builder.RegisterInstance(existingCommandResponse.Object).AsImplementedInterfaces();
            var container = builder.Build();

            var mediator = container.Resolve<IMediator>();

            //Act
            var response = await mediator.Send(command);

            //Assert
            Assert.True(response);
            existingCommandResponse.Verify(x => x.Handle(It.IsAny<ExistingCommandResponse<CreateOrderCommand, bool>.ExistingCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _commandHandler.Verify(x => x.Handle(It.IsAny<CreateOrderCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
