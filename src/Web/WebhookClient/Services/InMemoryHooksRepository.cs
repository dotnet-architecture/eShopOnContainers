using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebhookClient.Models;

namespace WebhookClient.Services
{
    public class InMemoryHooksRepository : IHooksRepository
    {
        private readonly List<WebHookReceived> _data;

        public InMemoryHooksRepository() => _data = new List<WebHookReceived>();

        public Task AddNew(WebHookReceived hook)
        {
            _data.Add(hook);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<WebHookReceived>> GetAll()
        {
            return Task.FromResult(_data.AsEnumerable());
        }
    }
}
