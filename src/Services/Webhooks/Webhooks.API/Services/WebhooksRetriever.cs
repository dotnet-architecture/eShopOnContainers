namespace Webhooks.API.Services;

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
