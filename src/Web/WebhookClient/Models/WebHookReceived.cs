using System;

namespace WebhookClient.Models
{
    public class WebHookReceived
    {
        public DateTime When { get; set; }
        
        public string Data { get; set; }

        public string Token { get; set; }
    }
}
