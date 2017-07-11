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
                    Name = ".NET Bot Black Hoodie 50% OFF",
                    Description = "Campaign Description 1",
                    From = DateTime.Now,
                    To = DateTime.Now.AddDays(7),
                    PictureUri = "http://externalcatalogbaseurltobereplaced/api/v1/campaigns/1/pic",
                    PictureName = "1.png",
                    Rules = new List<Rule>
                    {
                        new UserLocationRule
                        {
                            Description = "Campaign is only for United States users.",
                            LocationId = 1
                        }
                    }
                },
                new Campaign
                {
                    Name = "Roslyn Red T-Shirt 3x2",
                    Description = "Campaign Description 2",
                    From = DateTime.Now.AddDays(-7),
                    To = DateTime.Now.AddDays(14),
                    PictureUri = "http://externalcatalogbaseurltobereplaced/api/v1/campaigns/2/pic",
                    PictureName = "2.png",
                    Rules = new List<Rule>
                    {
                        new UserLocationRule
                        {
                            Description = "Campaign is only for Seattle users.",
                            LocationId = 3
                        }
                    }
                }
            };
        }
    }
}
