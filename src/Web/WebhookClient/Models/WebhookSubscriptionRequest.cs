using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebhookClient.Models
{
    public class WebhookSubscriptionRequest
    {
        public string Url { get; set; }
        public string Token { get; set; }
        public string Event { get; set; }
        public string GrantUrl { get; set; }
    }
}
