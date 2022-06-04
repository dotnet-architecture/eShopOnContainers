namespace WebhookClient.Services;

public interface IHooksRepository
{
    Task<IEnumerable<WebHookReceived>> GetAll();
    Task AddNew(WebHookReceived hook);
}
