
namespace Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure
{
    using AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Ordering.Infrastructure;
    using System.Threading.Tasks;


    public class OrderingContextSeed
    {
        public static async Task SeedAsync(IApplicationBuilder applicationBuilder)
        {
            var context = (OrderingContext)applicationBuilder
                .ApplicationServices.GetService(typeof(OrderingContext));

            using (context)
            {
                context.Database.Migrate();

                await context.SaveChangesAsync();
            }
        }

    }
}
