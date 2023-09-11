using System.Net.Http.Formatting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebhookClient.Pages
{
    [Authorize]

    public class RegisterWebhookModel : PageModel
    {
        private readonly WebhookClientOptions _options;
        private readonly IHttpClientFactory _httpClientFactory;

        [BindProperty] public string Token { get; set; }

        public int ResponseCode { get; set; }
        public string RequestUrl { get; set; }
        public string GrantUrl { get; set; }
        public string ResponseMessage { get; set; }
        public string RequestBodyJson { get; set; }

        public RegisterWebhookModel(IOptions<WebhookClientOptions> options, IHttpClientFactory httpClientFactory)
        {
            _options = options.Value;
            _httpClientFactory = httpClientFactory;
        }

        public void OnGet()
        {
            ResponseCode = (int)HttpStatusCode.OK;
            Token = _options.Token;
        }

        public async Task<IActionResult> OnPost()
        {
            ResponseCode = (int)HttpStatusCode.OK;
            var protocol = Request.IsHttps ? "https" : "http";
            var selfurl = !string.IsNullOrEmpty(_options.SelfUrl) ? _options.SelfUrl : $"{protocol}://{Request.Host}/{Request.PathBase}";
            if (!selfurl.EndsWith("/"))
            {
                selfurl = selfurl + "/";
            }
            var granturl = $"{selfurl}check";
            var url = $"{selfurl}webhook-received";
            var client = _httpClientFactory.CreateClient("GrantClient");

            var payload = new WebhookSubscriptionRequest()
            {
                Event = "OrderPaid",
                GrantUrl = granturl,
                Url = url,
                Token = Token
            };
            var response = await client.PostAsync(_options.WebhooksUrl + "/api/v1/webhooks", payload, new JsonMediaTypeFormatter());

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("WebhooksList");
            }
            else
            {
                RequestBodyJson = JsonSerializer.Serialize(payload);
                ResponseCode = (int)response.StatusCode;
                ResponseMessage = response.ReasonPhrase;
                GrantUrl = granturl;
                RequestUrl = $"{response.RequestMessage.Method} {response.RequestMessage.RequestUri}";
            }

            return Page();
        }
    }
}
