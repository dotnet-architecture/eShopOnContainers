namespace WebhookClient.Services;

public class WebhooksClient : IWebhooksClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly WebhookClientOptions _options;
    public WebhooksClient(IHttpClientFactory httpClientFactory, IOptions<WebhookClientOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
    }
    public async Task<IEnumerable<WebhookResponse>> LoadWebhooks()
    {
        var client = _httpClientFactory.CreateClient("GrantClient");
        var response = await client.GetAsync(_options.WebhooksUrl + "/api/v1/webhooks");
        var json = await response.Content.ReadAsStringAsync();
        var subscriptions = JsonSerializer.Deserialize<IEnumerable<WebhookResponse>>(json, JsonDefaults.CaseInsensitiveOptions);
        return subscriptions;
    }
}
