using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webhooks.API.Infrastructure;
using Webhooks.API.Model;

namespace Webhooks.API.Services
{
    public class WebhooksRetriever : IWebhooksRetriever
    {
        private readonly WebhooksContext _db;
        public WebhooksRetriever(WebhooksContext db)
        {
            _db = db;
        }
        public async Task<IEnumerable<WebhookSubscription>> GetSubscriptionsOfType(WebhookType type)
        {
            var data = await _db.Subscriptions.Where(s => s.Type == type).ToListAsync();
            return data;
        }
    }
}
