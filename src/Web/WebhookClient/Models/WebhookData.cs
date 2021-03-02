using System;

namespace WebhookClient.Models
{
    public class WebhookData
    {
        public DateTime When { get; set; }

        public string Payload { get; set; }

        public string Type { get; set; }
    }
}
