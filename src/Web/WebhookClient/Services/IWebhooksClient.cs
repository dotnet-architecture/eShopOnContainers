using System.Collections.Generic;
using System.Threading.Tasks;
using WebhookClient.Models;

namespace WebhookClient.Services
{
    public interface IWebhooksClient
    {
        Task<IEnumerable<WebhookResponse>> LoadWebhooks();
    }
}
