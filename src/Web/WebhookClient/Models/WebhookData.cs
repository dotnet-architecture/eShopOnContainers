using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebhookClient.Models
{
    public class WebhookData
    {
        public DateTime When { get; set; }

        public string Payload { get; set; }

        public string Type { get; set; }
    }
}
