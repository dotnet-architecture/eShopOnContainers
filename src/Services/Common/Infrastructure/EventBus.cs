
using Microsoft.eShopOnContainers.Services.Common.Infrastructure.Catalog;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.eShopOnContainers.Services.Common.Infrastructure
{
    public class EventBus : IEventBus
    {
        private readonly Dictionary<string, List<IIntegrationEventHandler>>  _handlers;        
        private readonly Dictionary<string, Tuple<IModel, IConnection>> _listeners;

        public EventBus()
        {
            _handlers = new Dictionary<string, List<IIntegrationEventHandler>>();
            _listeners = new Dictionary<string, Tuple<IModel, IConnection>>();
        }
        public void Publish(IIntegrationEvent @event)
        {
            var factory = new ConnectionFactory() { HostName = "172.20.0.1" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {                
                channel.QueueDeclare(queue: @event.Name,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = ((CatalogPriceChanged)@event).Message;                
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: @event.Name,
                                     basicProperties: null,
                                     body: body);                
            }
            
        }

        public void Subscribe<T>(IIntegrationEventHandler<T> handler) where T : IIntegrationEvent
        {
            var eventName = typeof(T).Name;
            if (_handlers.ContainsKey(eventName))   
            {
                _handlers[eventName].Add(handler);
            }
            else
            {
                var factory = new ConnectionFactory() { HostName = "172.18.0.1" };
                var connection = factory.CreateConnection();
                var channel = connection.CreateModel();
                
                channel.QueueDeclare(queue: eventName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);                    
                };
                channel.BasicConsume(queue: "hello",
                                     noAck: true,
                                     consumer: consumer);
                ;

                _listeners.Add(eventName, new Tuple<IModel, IConnection>(channel, connection));
                _handlers.Add(eventName, new List<IIntegrationEventHandler>());
                _handlers[eventName].Add(handler);
            }
            
        }

        public void Unsubscribe<T>(IIntegrationEventHandler<T> handler) where T : IIntegrationEvent
        {
            var eventName = typeof(T).Name;
            if (_handlers.ContainsKey(eventName) && _handlers[eventName].Contains(handler))
            {
                _handlers[eventName].Remove(handler);

                if (_handlers[eventName].Count == 0)
                {
                    _handlers.Remove(eventName);

                    var connectionItems =_listeners[eventName];
                    _listeners.Remove(eventName);

                    connectionItems.Item1.Close();
                    connectionItems.Item2.Close();
                }
            }
        }
    }
}
