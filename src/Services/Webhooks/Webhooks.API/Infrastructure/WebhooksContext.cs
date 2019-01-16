using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webhooks.API.Model;

namespace Webhooks.API.Infrastructure
{
    public class WebhooksContext : DbContext
    {

        public WebhooksContext(DbContextOptions<WebhooksContext> options) : base(options)
        {
        }
        public DbSet<WebhookSubscription> Subscriptions { get; set; }
    }
}
