using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebhookClient.Models;

namespace WebhookClient.Services
{
    public class WebhooksClient : IWebhooksClient
    {
        public async Task<IEnumerable<WebhookResponse>> LoadWebhooks()
        {
            return new[]{
                new WebhookResponse()
                {
                    Date = DateTime.Now,
                    DestUrl = "http://aaaaa.me",
                    Token = "3282832as2"
                },
                new WebhookResponse()
                {
                    Date = DateTime.Now.Subtract(TimeSpan.FromSeconds(392)),
                    DestUrl = "http://bbbbb.me",
                    Token = "ds2"
                }
            };

        }
    }
}
