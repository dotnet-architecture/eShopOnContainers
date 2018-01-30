namespace Microsoft.eShopOnContainers.Services.Marketing.API.Infrastructure
{
    using Microsoft.eShopOnContainers.Services.Marketing.API.Model;
    using Microsoft.Extensions.Logging;
    using Polly;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;

    public class MarketingContextSeed
    {
        public async Task SeedAsync(MarketingContext context,ILogger<MarketingContextSeed> logger,int retries = 3)
        {
            var policy = CreatePolicy(retries, logger, nameof(MarketingContextSeed));

            await policy.ExecuteAsync(async () =>
            {
                if (!context.Campaigns.Any())
                {
                    await context.Campaigns.AddRangeAsync(
                        GetPreconfiguredMarketings());

                    await context.SaveChangesAsync();
                }
            });
        }

        private List<Campaign> GetPreconfiguredMarketings()
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
     
        private Policy CreatePolicy(int retries, ILogger<MarketingContextSeed> logger, string prefix)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogTrace($"[{prefix}] Exception {exception.GetType().Name} with message ${exception.Message} detected on attempt {retry} of {retries}");
                    }
                );
        }
    }
}
