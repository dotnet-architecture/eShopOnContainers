using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
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

    public class WebhooksContextDesignFactory : IDesignTimeDbContextFactory<WebhooksContext>
    {
        public WebhooksContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<WebhooksContext>()
                .UseSqlServer("Server=.;Initial Catalog=Microsoft.eShopOnContainers.Services.CatalogDb;Integrated Security=true");

            return new WebhooksContext(optionsBuilder.Options);
        }
    }
}
