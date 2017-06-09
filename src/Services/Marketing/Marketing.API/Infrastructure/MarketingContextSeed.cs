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

    public class MarketingContextSeed
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
                    Description = "Campaign1",
                    From = DateTime.Now,
                    To = DateTime.Now.AddDays(7),
                    Url = "http://CampaignUrl.test/12f09ed3cef54187123f500ad",
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
                    Description = "Campaign2",
                    From = DateTime.Now.AddDays(7),
                    To = DateTime.Now.AddDays(14),
                    Url = "http://CampaignUrl.test/02a59eda65f241871239000ff",
                    Rules = new List<Rule>
                    {
                        new UserLocationRule
                        {
                            Description = "UserLocationRule2",
                            LocationId = 3
                        }
                    }
                }
            };
        }
    }
}
