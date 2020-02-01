using Autofac;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBusRabbitMQ
{
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        const string BROKER_NAME = "eshop_event_bus";

        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly ILogger<EventBusRabbitMQ> _logger;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly ILifetimeScope _autofac;
        private readonly string AUTOFAC_SCOPE_NAME = "eshop_event_bus";
        private static readonly String tenantACustomisationUrl = @"http://tenantacustomisation/";
        private static readonly String tenantManagerUrl = @"http://tenantmanager/";
        private readonly int _retryCount;


        private IModel _consumerChannel;
        private string _queueName;

        public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection, ILogger<EventBusRabbitMQ> logger,
            ILifetimeScope autofac, IEventBusSubscriptionsManager subsManager, string queueName = null,
            int retryCount = 5)
        {
            _persistentConnection =
                persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
            _queueName = queueName;
            _consumerChannel = CreateConsumerChannel();
            _autofac = autofac;
            _retryCount = retryCount;
            _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
        }

        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            using (var channel = _persistentConnection.CreateModel())
            {
                channel.QueueUnbind(queue: _queueName,
                    exchange: BROKER_NAME,
                    routingKey: eventName);

                if (_subsManager.IsEmpty)
                {
                    _queueName = string.Empty;
                    _consumerChannel.Close();
                }
            }
        }

        public void Publish(IntegrationEvent @event)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var policy = RetryPolicy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (ex, time) =>
                    {
                        _logger.LogWarning(ex,
                            "Could not publish event: {EventId} after {Timeout}s ({ExceptionMessage})", @event.Id,
                            $"{time.TotalSeconds:n1}", ex.Message);
                    });

            var eventName = @event.GetType().Name;

            _logger.LogWarning("Creating RabbitMQ channel to publish event: {EventId} ({EventName})", @event.Id,
                eventName);

            using (var channel = _persistentConnection.CreateModel())
            {
                _logger.LogWarning("Declaring RabbitMQ exchange to publish event: {EventId}", @event.Id);

                channel.ExchangeDeclare(exchange: BROKER_NAME, type: "direct");

                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);

                policy.Execute(() =>
                {
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 2; // persistent

                    _logger.LogWarning("Publishing event to RabbitMQ: {EventId}", @event.Id);

                    channel.BasicPublish(
                        exchange: BROKER_NAME,
                        routingKey: eventName,
                        mandatory: true,
                        basicProperties: properties,
                        body: body);
                });
            }
        }

        public void SubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler
        {
            _logger.LogInformation("Subscribing to dynamic event {EventName} with {EventHandler}", eventName,
                typeof(TH).GetGenericTypeName());

            DoInternalSubscription(eventName);
            _subsManager.AddDynamicSubscription<TH>(eventName);
            StartBasicConsume();
        }

        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = _subsManager.GetEventKey<T>();
            DoInternalSubscription(eventName);

            _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName,
                typeof(TH).GetGenericTypeName());

            _subsManager.AddSubscription<T, TH>();
            StartBasicConsume();
        }

        private void DoInternalSubscription(string eventName)
        {
            var containsKey = _subsManager.HasSubscriptionsForEvent(eventName);
            if (!containsKey)
            {
                if (!_persistentConnection.IsConnected)
                {
                    _persistentConnection.TryConnect();
                }

                using (var channel = _persistentConnection.CreateModel())
                {
                    channel.QueueBind(queue: _queueName,
                        exchange: BROKER_NAME,
                        routingKey: eventName);
                }
            }
        }

        public void Unsubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = _subsManager.GetEventKey<T>();

            _logger.LogInformation("Unsubscribing from event {EventName}", eventName);

            _subsManager.RemoveSubscription<T, TH>();
        }

        public void UnsubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler
        {
            _subsManager.RemoveDynamicSubscription<TH>(eventName);
        }

        public void Dispose()
        {
            if (_consumerChannel != null)
            {
                _consumerChannel.Dispose();
            }

            _subsManager.Clear();
        }

        private void StartBasicConsume()
        {
            _logger.LogTrace("Starting RabbitMQ basic consume");

            if (_consumerChannel != null)
            {
                var consumer = new AsyncEventingBasicConsumer(_consumerChannel);

                consumer.Received += Consumer_Received;

                _consumerChannel.BasicConsume(
                    queue: _queueName,
                    autoAck: false,
                    consumer: consumer);
            }
            else
            {
                _logger.LogError("StartBasicConsume can't call on _consumerChannel == null");
            }
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            var eventName = eventArgs.RoutingKey;
            var message = Encoding.UTF8.GetString(eventArgs.Body);

            try
            {
                if (message.ToLowerInvariant().Contains("throw-fake-exception"))
                {
                    throw new InvalidOperationException($"Fake exception requested: \"{message}\"");
                }

                await ProcessEvent(eventName, message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "----- ERROR Processing message \"{Message}\"", message);
            }

            // Even on exception we take the message off the queue.
            // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
            // For more information see: https://www.rabbitmq.com/dlx.html
            _consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }

        private IModel CreateConsumerChannel()
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            _logger.LogTrace("Creating RabbitMQ consumer channel");

            var channel = _persistentConnection.CreateModel();

            channel.ExchangeDeclare(exchange: BROKER_NAME,
                type: "direct");

            channel.QueueDeclare(queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            channel.CallbackException += (sender, ea) =>
            {
                _logger.LogWarning(ea.Exception, "Recreating RabbitMQ consumer channel");

                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
                StartBasicConsume();
            };

            return channel;
        }

        private async void SendEventToTenant(String content, String id, String eventName)
        {
            var temp = new SavedEvent();
            temp.Content = content;
            temp.SavedEventId = id;
            temp.EventName = eventName;
            string myJson = JsonConvert.SerializeObject(temp);
            using (var client = new HttpClient())
            {
                try
                {
                    //TODO replace URL with response from tenantmanager
                    var response = await client.PostAsync(
                        tenantACustomisationUrl + "api/SavedEvents",
                        new StringContent(myJson, Encoding.UTF8, "application/json"));
                    response.EnsureSuccessStatusCode();
                    _logger.LogInformation("----- Event sent to tenant{@id} -----", id);
                }

                catch (Exception e)
                {
                    _logger.LogInformation("----- Exception{@e} -- Event{@id} -----", e, @id);
                }
            }
        }

        private async Task<Boolean> IsEventCustomised(String eventName, int tenantId)
        {
            CustomisationInfo customisationInfo = new CustomisationInfo();
            customisationInfo.EventName = eventName;
            customisationInfo.TenantId = tenantId;
            Boolean isCustomised = false;

            var builder = new UriBuilder(tenantManagerUrl + "api/Customisations/IsCustomised");
            builder.Port = -1;
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["eventName"] = eventName;
            query["tenantId"] = tenantId.ToString();
            builder.Query = query.ToString();
            string url = builder.ToString();

            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(
                        url);
                    response.EnsureSuccessStatusCode();
                    isCustomised =
                        JsonConvert.DeserializeObject<Boolean>(response.Content.ReadAsStringAsync().Result);
                }
                catch (Exception e)
                {
                    _logger.LogInformation("----- Exception{@e}", e);
                }
            }

            return isCustomised;
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            _logger.LogWarning("Processing RabbitMQ event: {EventName}", eventName);

            if (_subsManager.HasSubscriptionsForEvent(eventName))
            {
                using (var scope = _autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME))
                {
                    var subscriptions = _subsManager.GetHandlersForEvent(eventName);
                    foreach (var subscription in subscriptions)
                    {
                        if (subscription.IsDynamic)
                        {
                            //TODO check if it is required here aswell
                            var handler =
                                scope.ResolveOptional(subscription.HandlerType) as IDynamicIntegrationEventHandler;
                            if (handler == null) continue;
                            dynamic eventData = JObject.Parse(message);

                            await Task.Yield();
                            await handler.Handle(eventData);
                        }
                        else
                        {
                            var handler = scope.ResolveOptional(subscription.HandlerType);

                            if (handler == null) continue;
                            var eventType = _subsManager.GetEventTypeByName(eventName);
                            var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                            IntegrationEvent evt = (IntegrationEvent) integrationEvent;
                            if (IsEventCustomised(eventName, evt.TenantId).Result) //TODO fix tenantId part of request
                            {
                                //Checking if event should be sent to tenant, or handled normally
                                if (evt.CheckForCustomisation && evt.TenantId == 1)//TODO use tenantId to choose the correct endpoint to send the event to
                                {
                                    SendEventToTenant(message, evt.Id.ToString(), eventName);
                                    break;
                                }
                            }

                            var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                            var handlerName = handler.ToString();
                            //Not tenant specific handler
                            if (!handlerName.Contains(("TenantA")))
                            {
                                await Task.Yield();
                                await (Task) concreteType.GetMethod("Handle")
                                    .Invoke(handler, new object[] {integrationEvent});
                            }
                            //Tenant specific handler, and event belongs to that tenant
                            else if (handlerName.Contains("TenantA") && evt.TenantId == 1)
                            {
                                await Task.Yield();
                                await (Task) concreteType.GetMethod("Handle")
                                    .Invoke(handler, new object[] {integrationEvent});
                            }
                        }
                    }
                }
            }
            else
            {
                _logger.LogWarning("No subscription for RabbitMQ event: {EventName}", eventName);
            }
        }
    }
}

class SavedEvent
{
    public string SavedEventId { get; set; }
    public string Content { get; set; }
    public String EventName { get; set; }
}

class CustomisationInfo
{
    public string EventName { get; set; }
    public int TenantId { get; set; }
}