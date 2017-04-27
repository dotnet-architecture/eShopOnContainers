using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBusRabbitMQ
{
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        const string BROKER_NAME = "eshop_event_bus";

        private readonly IRabbitMQPersisterConnection _persisterConnection;
        private readonly ILogger<EventBusRabbitMQ> _logger;

        private readonly Dictionary<string, List<IIntegrationEventHandler>> _handlers
            = new Dictionary<string, List<IIntegrationEventHandler>>();

        private readonly List<Type> _eventTypes
            = new List<Type>();

        private IModel _consumerChannel;
        private string _queueName;

        public EventBusRabbitMQ(IRabbitMQPersisterConnection persisterConnection, ILogger<EventBusRabbitMQ> logger)
        {
            _persisterConnection = persisterConnection ?? throw new ArgumentNullException(nameof(persisterConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _consumerChannel = CreateConsumerChannel();
        }


        public void Publish(IntegrationEvent @event)
        {
            if (!_persisterConnection.IsConnected)
            {
                _persisterConnection.TryConnect();
            }

            var policy = RetryPolicy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex.ToString());
                });

            using (var channel = _persisterConnection.CreateModel())
            {
                var eventName = @event.GetType()
                    .Name;

                channel.ExchangeDeclare(exchange: BROKER_NAME,
                                    type: "direct");

                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);

                policy.Execute(() =>
                {
                    channel.BasicPublish(exchange: BROKER_NAME,
                                     routingKey: eventName,
                                     basicProperties: null,
                                     body: body);
                });
            }
        }

        public void Subscribe<T>(IIntegrationEventHandler<T> handler) where T : IntegrationEvent
        {
            var eventName = typeof(T).Name;

            if (_handlers.ContainsKey(eventName))
            {
                _handlers[eventName].Add(handler);
            }
            else
            {
                if (!_persisterConnection.IsConnected)
                {
                    _persisterConnection.TryConnect();
                }

                using (var channel = _persisterConnection.CreateModel())
                {
                    channel.QueueBind(queue: _queueName,
                                      exchange: BROKER_NAME,
                                      routingKey: eventName);

                    _handlers.Add(eventName, new List<IIntegrationEventHandler>());
                    _handlers[eventName].Add(handler);
                    _eventTypes.Add(typeof(T));
                }

            }

        }

        public void Unsubscribe<T>(IIntegrationEventHandler<T> handler) where T : IntegrationEvent
        {
            var eventName = typeof(T).Name;

            if (_handlers.ContainsKey(eventName) && _handlers[eventName].Contains(handler))
            {
                _handlers[eventName].Remove(handler);

                if (_handlers[eventName].Count == 0)
                {
                    _handlers.Remove(eventName);

                    var eventType = _eventTypes.SingleOrDefault(e => e.Name == eventName);

                    if (eventType != null)
                    {
                        _eventTypes.Remove(eventType);

                        if (!_persisterConnection.IsConnected)
                        {
                            _persisterConnection.TryConnect();
                        }

                        using (var channel = _persisterConnection.CreateModel())
                        {
                            channel.QueueUnbind(queue: _queueName,
                                exchange: BROKER_NAME,
                                routingKey: eventName);

                            if (_handlers.Keys.Count == 0)
                            {
                                _queueName = string.Empty;

                                _consumerChannel.Close();
                            }
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            if (_consumerChannel != null)
            {
                _consumerChannel.Dispose();
            }
            
            _handlers.Clear();
        }

        private IModel CreateConsumerChannel()
        {
            if (!_persisterConnection.IsConnected)
            {
                _persisterConnection.TryConnect();
            }

            var channel = _persisterConnection.CreateModel();

            channel.ExchangeDeclare(exchange: BROKER_NAME,
                                 type: "direct");

            _queueName = channel.QueueDeclare().QueueName;

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var eventName = ea.RoutingKey;
                var message = Encoding.UTF8.GetString(ea.Body);

                await ProcessEvent(eventName, message);
            };

            channel.BasicConsume(queue: _queueName,
                                 noAck: true,
                                 consumer: consumer);

            channel.CallbackException += (sender, ea) =>
            {
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
            };

            return channel;
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (_handlers.ContainsKey(eventName))
            {
                Type eventType = _eventTypes.Single(t => t.Name == eventName);
                var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                var handlers = _handlers[eventName];

                foreach (var handler in handlers)
                {
                    await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                }
            }
        }
    }
}
