using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using MediatR;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure.Services;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.BuyerAggregate;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
using Microsoft.Extensions.Logging;
using Moq;
using Ordering.API.Application.DomainEventHandlers.BuyerAndPaymentMethodVerified;
using Ordering.API.Application.IntegrationEvents;
using Ordering.Domain.Events;
using Ordering.Infrastructure;
using Xunit;

namespace UnitTest.Ordering.Application
{
    public class DomainEventHandlersTest
    {
        private sealed class FakeBuyer: Buyer
        {
            public FakeBuyer(int id) : base("userId")
            {
                Id = id;
            }
        }

        private sealed class FakeOrder : Order
        {
            public FakeOrder(int id): base("userId", new Address("street", "city", "state", "country", "zipcode"), 0, "cardNumber", "cardSecurityNumber", "cardHolderName", DateTime.Now)
            {
                Id = id;
            }
        }

        private sealed class FakePaymentMethod: PaymentMethod
        {
            public FakePaymentMethod(int id):base(1, "alias", "carNumber", "securityNumber", "cardHolderName", DateTime.Now)
            {
                Id = id;
            }
        }

        private class FakeLogger : ILogger
        {
            private readonly TextWriter _writer;

            public FakeLogger(TextWriter writer)
            {
                _writer = writer;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                _writer.WriteLineAsync(state.ToString());
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return null;
            }
        }

        private static IMediator CreateMediator(Action<ContainerBuilder> containerConfigure)
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();

            builder
                .RegisterAssemblyTypes(typeof(UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(INotificationHandler<>))
                .AsImplementedInterfaces();

            builder.Register<SingleInstanceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

            builder.Register<MultiInstanceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => (IEnumerable<object>)c.Resolve(typeof(IEnumerable<>).MakeGenericType(t));
            });

            containerConfigure(builder);

            var container = builder.Build();
            var mediator = container.Resolve<IMediator>();
            return mediator;
        }

        [Fact]
        public async Task UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler()
        {
            const int orderId = 101;
            const int paymentId = 201;

            var builder = new StringBuilder();
            var writer = new StringWriter(builder);

            var buyer = new Buyer("Id");
            var domainEvent = new BuyerAndPaymentMethodVerifiedDomainEvent(buyer, new FakePaymentMethod(paymentId), orderId);

            var mediator = CreateMediator(containerBuilder =>
            {
                var mockLoggerFactory = new Mock<ILoggerFactory>();
                mockLoggerFactory.Setup(m => m.CreateLogger(It.IsAny<string>())).Returns(new FakeLogger(writer));
                containerBuilder.RegisterInstance(mockLoggerFactory.Object).As<ILoggerFactory>();

                var mockOrderRepository = new Mock<IOrderRepository>();
                mockOrderRepository.Setup(m => m.GetAsync(It.IsAny<int>()))
                    .Returns(Task.FromResult(new Order("userId", new Address("street", "city", "state", "country", "zipcode"), 1, "cardNumber", "cardSecurityNumber", "cardHolderName", DateTime.Now)));
                containerBuilder.RegisterInstance(mockOrderRepository.Object).As<IOrderRepository>();

                var mockOrderingIntegrationEventService = new Mock<IOrderingIntegrationEventService>();
                mockOrderingIntegrationEventService.Setup(m => m.PublishThroughEventBusAsync(It.IsAny<IntegrationEvent>()))
                    .Returns(Task.CompletedTask);
                containerBuilder.RegisterInstance(mockOrderingIntegrationEventService.Object).As<IOrderingIntegrationEventService>();
            });

            await mediator.PublishEvent(domainEvent, CancellationToken.None);

            var result = builder.ToString().Replace(Environment.NewLine, "");
            Assert.Equal($"Order with Id: {orderId} has been successfully updated with a payment method id: { paymentId }", result);
        }


        [Fact]
        public async Task OrderStatusChangedToAwaitingValidationDomainEventHandler()
        {
            const int orderId = 102;
            const int productId = 202;

            var builder = new StringBuilder();
            var writer = new StringWriter(builder);

            var domainEvent = new OrderStatusChangedToAwaitingValidationDomainEvent(orderId, new[] {new OrderItem(productId, "productName", 0, 0, "pictureUrl")});            
            var mediator = CreateMediator(containerBuilder =>
            {
                var mockLoggerFactory = new Mock<ILoggerFactory>();
                mockLoggerFactory.Setup(m => m.CreateLogger(It.IsAny<string>())).Returns(new FakeLogger(writer));
                containerBuilder.RegisterInstance(mockLoggerFactory.Object).As<ILoggerFactory>();

                var mockOrderRepository = new Mock<IOrderRepository>();
                containerBuilder.RegisterInstance(mockOrderRepository.Object).As<IOrderRepository>();

                var mockOrderingIntegrationEventService = new Mock<IOrderingIntegrationEventService>();
                mockOrderingIntegrationEventService.Setup(m => m.PublishThroughEventBusAsync(It.IsAny<IntegrationEvent>()))
                    .Returns(Task.CompletedTask);
                containerBuilder.RegisterInstance(mockOrderingIntegrationEventService.Object).As<IOrderingIntegrationEventService>();
            });

            await mediator.PublishEvent(domainEvent, CancellationToken.None);

            var result = builder.ToString().Replace(Environment.NewLine, "");
            Assert.Equal($"Order with Id: {orderId} has been successfully updated with a status order id: {OrderStatus.AwaitingValidation.Id}", result);
        }

        [Fact]
        public async Task OrderStatusChangedToPaidDomainEventHandler()
        {
            const int orderId = 103;
            const int productId = 203;

            var builder = new StringBuilder();
            var writer = new StringWriter(builder);

            var domainEvent = new OrderStatusChangedToPaidDomainEvent(orderId, new[] {new OrderItem(productId, "productName", 0, 0, "pictureUrl")});
            var mediator = CreateMediator(containerBuilder =>
            {
                var mockLoggerFactory = new Mock<ILoggerFactory>();
                mockLoggerFactory.Setup(m => m.CreateLogger(It.IsAny<string>())).Returns(new FakeLogger(writer));
                containerBuilder.RegisterInstance(mockLoggerFactory.Object).As<ILoggerFactory>();

                var mockOrderRepository = new Mock<IOrderRepository>();
                containerBuilder.RegisterInstance(mockOrderRepository.Object).As<IOrderRepository>();

                var mockOrderingIntegrationEventService = new Mock<IOrderingIntegrationEventService>();
                mockOrderingIntegrationEventService.Setup(m => m.PublishThroughEventBusAsync(It.IsAny<IntegrationEvent>()))
                    .Returns(Task.CompletedTask);
                containerBuilder.RegisterInstance(mockOrderingIntegrationEventService.Object).As<IOrderingIntegrationEventService>();
            });

            await mediator.PublishEvent(domainEvent, CancellationToken.None);

            var result = builder.ToString().Replace(Environment.NewLine, "");
            Assert.Equal($"Order with Id: {orderId} has been successfully updated with a status order id: {OrderStatus.Paid.Id}", result);
        }

        [Fact]
        public async Task ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler()
        {
            const int buyerUpdatedId = 104;
            const int orderId = 204;

            var builder = new StringBuilder();
            var writer = new StringWriter(builder);

            var order = new FakeOrder(orderId);
            var domainEvent = new OrderStartedDomainEvent(order, "userId", 0, "cardNumber", "cardSecurityNumber","cardHolderName", DateTime.Now);

            var mediator = CreateMediator(containerBuilder =>
            {
                var mockLoggerFactory = new Mock<ILoggerFactory>();
                mockLoggerFactory.Setup(m => m.CreateLogger(It.IsAny<string>())).Returns(new FakeLogger(writer));
                containerBuilder.RegisterInstance(mockLoggerFactory.Object).As<ILoggerFactory>().SingleInstance();

                var mockBuyerRepository = new Mock<IBuyerRepository>();
                mockBuyerRepository.Setup(m => m.FindAsync(It.IsAny<string>()))
                    .Returns(Task.FromResult<Buyer>(null));
                mockBuyerRepository.Setup(m => m.Add(It.IsAny<Buyer>())).Returns(new FakeBuyer(buyerUpdatedId));
                                
                mockBuyerRepository.Setup(m => m.UnitOfWork.SaveEntitiesAsync(CancellationToken.None))
                    .Returns(Task.FromResult(true));
                containerBuilder.RegisterInstance(mockBuyerRepository.Object).As<IBuyerRepository>();

                var mockIdentityService = new Mock<IIdentityService>();
                containerBuilder.RegisterInstance(mockIdentityService.Object).As<IIdentityService>();
            });

            await mediator.PublishEvent(domainEvent, CancellationToken.None);

            var result = builder.ToString().Replace(Environment.NewLine, "");
            Assert.Equal($"Buyer {buyerUpdatedId} and related payment method were validated or updated for orderId: {orderId}.", result);
        }

        [Fact]
        public async Task OrderStatusChangedToStockConfirmedDomainEventHandler()
        {
            const int orderId = 105;

            var builder = new StringBuilder();
            var writer = new StringWriter(builder);


            var domainEvent = new OrderStatusChangedToStockConfirmedDomainEvent(orderId);
            var mediator = CreateMediator(containerBuilder =>
            {
                var mockLoggerFactory = new Mock<ILoggerFactory>();
                mockLoggerFactory.Setup(m => m.CreateLogger(It.IsAny<string>())).Returns(new FakeLogger(writer));
                containerBuilder.RegisterInstance(mockLoggerFactory.Object).As<ILoggerFactory>();

                var mockOrderRepository = new Mock<IOrderRepository>();
                containerBuilder.RegisterInstance(mockOrderRepository.Object).As<IOrderRepository>();

                var mockOrderingIntegrationEventService = new Mock<IOrderingIntegrationEventService>();
                mockOrderingIntegrationEventService.Setup(m => m.PublishThroughEventBusAsync(It.IsAny<IntegrationEvent>()))
                    .Returns(Task.CompletedTask);
                containerBuilder.RegisterInstance(mockOrderingIntegrationEventService.Object).As<IOrderingIntegrationEventService>();

            });

            await mediator.PublishEvent(domainEvent, CancellationToken.None);

            var result = builder.ToString().Replace(Environment.NewLine, "");
            Assert.Equal($"Order with Id: {orderId} has been successfully updated with a status order id: {OrderStatus.StockConfirmed.Id}", result);

        }
    }
}