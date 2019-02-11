using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using WebhookClient.Models;
using WebhookClient.Services;

namespace WebhookClient.Pages
{

    public class IndexModel : PageModel
    {
        private readonly IHooksRepository _hooksRepository;

        public IndexModel(IHooksRepository hooksRepository)
        {
            _hooksRepository = hooksRepository;
        }

        public IEnumerable<WebHookReceived> WebHooksReceived { get; private set; }

        public async Task OnGet()
        {
            WebHooksReceived = await _hooksRepository.GetAll();
        }
    }
}
