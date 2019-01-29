using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebhookClient.Models
{
    public class WebhookData
    {
        public DateTime When { get; }

        public string Payload { get; }

        public string Type { get; }
    }
}
