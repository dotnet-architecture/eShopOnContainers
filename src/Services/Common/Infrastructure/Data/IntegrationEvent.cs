using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Microsoft.eShopOnContainers.Services.Common.Infrastructure.Data
{
    public class IntegrationEvent
    {
        public IntegrationEvent(IntegrationEventBase @event)
        {
            EventId = @event.Id;
            CreationTime = DateTime.UtcNow;
            EventTypeName = @event.GetType().FullName;
            Content = JsonConvert.SerializeObject(@event);
            State = EventStateEnum.NotSend;
            TimesSent = 0;
        }
        public Guid EventId { get; private set; }
        public string EventTypeName { get; private set; }
        public EventStateEnum State { get; set; }
        public int TimesSent { get; set; }
        public DateTime CreationTime { get; private set; }
        public string Content { get; private set; }
    }
}
