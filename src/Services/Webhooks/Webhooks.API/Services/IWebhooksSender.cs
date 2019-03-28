using System.Collections.Generic;
using System.Threading.Tasks;
using Webhooks.API.Model;

namespace Webhooks.API.Services
{
    public interface IWebhooksSender
    {
        Task SendAll(IEnumerable<WebhookSubscription> receivers, WebhookData data);
    }
}
