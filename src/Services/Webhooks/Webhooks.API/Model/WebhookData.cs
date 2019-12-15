using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webhooks.API.Model
{
    public class WebhookData
    {
        public DateTime When { get; }

        public string Payload { get; }

        public string Type { get;  }

        public WebhookData(WebhookType hookType, object data)
        {
            When = DateTime.UtcNow;
            Type = hookType.ToString();
            Payload = JsonConvert.SerializeObject(data);
        }


    }
}
