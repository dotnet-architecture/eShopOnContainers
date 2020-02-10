using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TenantBShippingInformation.Models;

namespace TenantBShippingInformation.Database
{
    public class TenantBContext : DbContext
    {
        public TenantBContext(DbContextOptions<TenantBContext> options)
            : base(options)
        {
        }

        public DbSet<ShippingInformation> ShippingInformation { get; set; }
        

    }
    public class TenantBContextDesignFactory : IDesignTimeDbContextFactory<TenantBContext>
    {
        public TenantBContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TenantBContext>()
                            .UseSqlServer("Server=.;Initial Catalog=Microsoft.eShopOnContainers.Services.TenantBShippingInformationDb;Integrated Security=true");

            return new TenantBContext(optionsBuilder.Options);
        }
    }
}
