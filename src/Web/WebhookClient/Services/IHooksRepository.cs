using System.Collections.Generic;
using System.Threading.Tasks;
using WebhookClient.Models;

namespace WebhookClient.Services
{
    public interface IHooksRepository
    {
        Task<IEnumerable<WebHookReceived>> GetAll();
        Task AddNew(WebHookReceived hook);
    }
}
