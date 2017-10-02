namespace Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure
{
    using AspNetCore.Builder;
    using global::Ordering.API.Extensions;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.BuyerAggregate;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Ordering.Infrastructure;
    using Polly;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class OrderingContextSeed
    {
        public  async Task SeedAsync(OrderingContext context, IHostingEnvironment env,IOptions<OrderingSettings> settings, ILogger<OrderingContextSeed> logger)
        {
            var policy = CreatePolicy(logger, nameof(OrderingContextSeed));

            await policy.ExecuteAsync(async () =>
            {

                var useCustomizationData = settings.Value
                .UseCustomizationData;

                var contentRootPath = env.ContentRootPath;


                using (context)
                {
                    context.Database.Migrate();

                    if (!context.CardTypes.Any())
                    {
                        context.CardTypes.AddRange(useCustomizationData
                                                ? GetCardTypesFromFile(contentRootPath, logger)
                                                : GetPredefinedCardTypes());

                        await context.SaveChangesAsync();
                    }

                    if (!context.OrderStatus.Any())
                    {
                        context.OrderStatus.AddRange(useCustomizationData
                                                ? GetOrderStatusFromFile(contentRootPath, logger)
                                                : GetPredefinedOrderStatus());
                    }

                    await context.SaveChangesAsync();
                }
            });
        }

        private IEnumerable<CardType> GetCardTypesFromFile(string contentRootPath, ILogger<OrderingContextSeed> log)
        {
            string csvFileCardTypes = Path.Combine(contentRootPath, "Setup", "CardTypes.csv");

            if (!File.Exists(csvFileCardTypes))
            {
                return GetPredefinedCardTypes();
            }

            string[] csvheaders;
            try
            {
                string[] requiredHeaders = { "CardType" };
                csvheaders = GetHeaders(requiredHeaders, csvFileCardTypes);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                return GetPredefinedCardTypes();
            }

            int id = 1;
            return File.ReadAllLines(csvFileCardTypes)
                                        .Skip(1) // skip header column
                                        .SelectTry(x => CreateCardType(x, ref id))
                                        .OnCaughtException(ex => { log.LogError(ex.Message); return null; })
                                        .Where(x => x != null);
        }

        private CardType CreateCardType(string value, ref int id)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw new Exception("Orderstatus is null or empty");
            }

            return new CardType(id++, value.Trim('"').Trim());
        }

        private  IEnumerable<CardType> GetPredefinedCardTypes()
        {
            return new List<CardType>()
            {
                CardType.Amex,
                CardType.Visa,
                CardType.MasterCard
            };
        }

        private IEnumerable<OrderStatus> GetOrderStatusFromFile(string contentRootPath, ILogger<OrderingContextSeed> log)
        {
            string csvFileOrderStatus = Path.Combine(contentRootPath, "Setup", "OrderStatus.csv");

            if (!File.Exists(csvFileOrderStatus))
            {
                return GetPredefinedOrderStatus();
            }

            string[] csvheaders;
            try
            {
                string[] requiredHeaders = { "OrderStatus" };
                csvheaders = GetHeaders(requiredHeaders, csvFileOrderStatus);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                return GetPredefinedOrderStatus();
            }

            int id = 1;
            return File.ReadAllLines(csvFileOrderStatus)
                                        .Skip(1) // skip header row
                                        .SelectTry(x => CreateOrderStatus(x, ref id))
                                        .OnCaughtException(ex => { log.LogError(ex.Message); return null; })
                                        .Where(x => x != null);
        }

        private OrderStatus CreateOrderStatus(string value, ref int id)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw new Exception("Orderstatus is null or empty");
            }

            return new OrderStatus(id++, value.Trim('"').Trim().ToLowerInvariant());
        }

        private IEnumerable<OrderStatus> GetPredefinedOrderStatus()
        {
            return new List<OrderStatus>()
            {
                OrderStatus.Submitted,
                OrderStatus.AwaitingValidation,
                OrderStatus.StockConfirmed,
                OrderStatus.Paid,
                OrderStatus.Shipped,
                OrderStatus.Cancelled
            };
        }

        private string[] GetHeaders(string[] requiredHeaders, string csvfile)
        {
            string[] csvheaders = File.ReadLines(csvfile).First().ToLowerInvariant().Split(',');

            if (csvheaders.Count() != requiredHeaders.Count())
            {
                throw new Exception($"requiredHeader count '{ requiredHeaders.Count()}' is different then read header '{csvheaders.Count()}'");
            }

            foreach (var requiredHeader in requiredHeaders)
            {
                if (!csvheaders.Contains(requiredHeader))
                {
                    throw new Exception($"does not contain required header '{requiredHeader}'");
                }
            }

            return csvheaders;
        }

     
        private Policy CreatePolicy( ILogger<OrderingContextSeed> logger, string prefix, int retries =3)
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
