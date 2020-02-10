using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TenantAShippingInformation.Models;

namespace TenantAShippingInformation.Database
{
    public class TenantAContext : DbContext
    {
        public TenantAContext(DbContextOptions<TenantAContext> options)
            : base(options)
        {
        }

        public DbSet<ShippingInformation> ShippingInformation { get; set; }
        

    }
    public class TenantAContextDesignFactory : IDesignTimeDbContextFactory<TenantAContext>
    {
        public TenantAContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TenantAContext>()
                            .UseSqlServer("Server=.;Initial Catalog=Microsoft.eShopOnContainers.Services.TenantAShippingInformationDb;Integrated Security=true");

            return new TenantAContext(optionsBuilder.Options);
        }
    }
}
