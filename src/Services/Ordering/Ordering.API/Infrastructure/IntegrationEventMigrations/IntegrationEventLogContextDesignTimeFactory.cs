using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.eShopOnContainers.BuildingBlocks.IntegrationEventLogEF;

namespace Catalog.API.Infrastructure.IntegrationEventMigrations
{
    public class IntegrationEventLogContextDesignTimeFactory : IDesignTimeDbContextFactory<IntegrationEventLogContext>
    {
        public IntegrationEventLogContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<IntegrationEventLogContext>();

            optionsBuilder.UseSqlServer(".", options => options.MigrationsAssembly(GetType().Assembly.GetName().Name));

            return new IntegrationEventLogContext(optionsBuilder.Options);
        }
    }
}