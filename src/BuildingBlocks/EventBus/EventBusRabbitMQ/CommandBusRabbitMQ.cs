//using Microsoft.eShopOnContainers.BuildingBlocks.CommandBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

/*
namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBusRabbitMQ
{
    public class CommandBusRabbitMQ : ICommandBus, IDisposable
    {
        const string BROKER_NAME = "eshop_command_bus";

        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly ILogger<CommandBusRabbitMQ> _logger;

        private IModel _consumerChannel;
        private string _queueName;

        private readonly Dictionary<string, IIntegrationCommandHandler> _handlers;
        private readonly Dictionary<string, Type> _typeMappings;

        public CommandBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection, 
            ILogger<CommandBusRabbitMQ> logger)
        {
            _logger = logger;
            _persistentConnection = persistentConnection;
            _handlers = new Dictionary<string, IIntegrationCommandHandler>();
            _typeMappings = new Dictionary<string, Type>();
        }

        public void Send<T>(string name, T data)
        {
            Send(new IntegrationCommand<T>(name, data));
        }

        public void Handle<TC>(string name, IIntegrationCommandHandler<TC> handler)
        {
            _handlers.Add(name, handler);
            _typeMappings.Add(name, typeof(TC));
        }

        public void Handle(string name, IIntegrationCommandHandler handler)
        {
            _handlers.Add(name, handler);
        }
        public void Handle<TI, TC>(TI handler) where TI : IIntegrationCommandHandler<TC>
        {
            var name = typeof(TI).Name;
            _handlers.Add(name, handler);
            _typeMappings.Add(name, typeof(TC));
        }

        private void Send<T>(IntegrationCommand<T> command)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var policy = RetryPolicy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                    {
                        _logger.LogWarning(ex.ToString());
                    });

            using (var channel = _persistentConnection.CreateModel())
            {
                var commandName = command.Name;
                channel.ExchangeDeclare(exchange: BROKER_NAME, type: "direct");
                var message = JsonConvert.SerializeObject(command);
                var body = Encoding.UTF8.GetBytes(message);
                policy.Execute(() =>
                {
                    channel.BasicPublish(exchange: BROKER_NAME,
                                     routingKey: commandName,
                                     basicProperties: null,
                                     body: body);
                });
            }
        }

        private IModel CreateConsumerChannel()
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var channel = _persistentConnection.CreateModel();

            channel.ExchangeDeclare(exchange: BROKER_NAME, type: "direct");
            _queueName = channel.QueueDeclare().QueueName;
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var commandName = ea.RoutingKey;
                var message = Encoding.UTF8.GetString(ea.Body);
                await InvokeHandler(commandName, message);
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

        private Task InvokeHandler(string commandName, string message)
        {
            if (_handlers.ContainsKey(commandName))
            {

            }

        }

        public void Dispose()
        {
            if (_consumerChannel != null)
            {
                _consumerChannel.Dispose();
            }
        }



    }
}
*/