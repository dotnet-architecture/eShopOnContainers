namespace Microsoft.eShopOnContainers.Services.Marketing.API.Infrastructure
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.eShopOnContainers.Services.Marketing.API.Model;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public static class MarketingContextSeed
    {
        public static async Task SeedAsync(IApplicationBuilder applicationBuilder, ILoggerFactory loggerFactory, int? retry = 0)
        {
            var context = (MarketingContext)applicationBuilder
                .ApplicationServices.GetService(typeof(MarketingContext));

            context.Database.Migrate();

            if (!context.Campaigns.Any())
            {
                context.Campaigns.AddRange(
                    GetPreconfiguredMarketings());

                await context.SaveChangesAsync();
            }
        }

        static List<Campaign> GetPreconfiguredMarketings()
        {
            return new List<Campaign>
            {
                new Campaign
                {
                    Name = "Campaign Name 1",
                    Description = "Campaign Description 1",
                    From = DateTime.Now,
                    To = DateTime.Now.AddDays(7),
                    PictureUri = "http://externalcatalogbaseurltobereplaced/api/v1/campaigns/1/pic",
                    Rules = new List<Rule>
                    {
                        new UserLocationRule
                        {
                            Description = "UserLocationRule1",
                            LocationId = 1
                        }
                    }
                },
                new Campaign
                {
                    Name = "Campaign Name 2",
                    Description = "Campaign Description 2",
                    From = DateTime.Now.AddDays(7),
                    To = DateTime.Now.AddDays(14),
                    PictureUri = "http://externalcatalogbaseurltobereplaced/api/v1/campaigns/2/pic",
                    Rules = new List<Rule>
                    {
                        new UserLocationRule
                        {
                            Description = "UserLocationRule2",
                            LocationId = 6
                        }
                    }
                }
            };
        }
    }
}
