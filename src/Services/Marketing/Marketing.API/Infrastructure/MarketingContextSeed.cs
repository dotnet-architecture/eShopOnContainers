namespace Microsoft.eShopOnContainers.Services.Marketing.API.Infrastructure
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using System.Threading.Tasks;

    public class MarketingContextSeed
    {
        public static async Task SeedAsync(IApplicationBuilder applicationBuilder, ILoggerFactory loggerFactory, int? retry = 0)
        {
            var context = (MarketingContext)applicationBuilder
                .ApplicationServices.GetService(typeof(MarketingContext));

            context.Database.Migrate();

            //TODO: add model seeding
        }
    }
}
