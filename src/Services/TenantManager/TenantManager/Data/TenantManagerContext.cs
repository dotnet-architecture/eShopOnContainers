using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TenantManager.Models;

namespace TenantManager.Database
{
    public class TenantManagerContext : DbContext
    {
        public TenantManagerContext (DbContextOptions<TenantManagerContext> options)
            : base(options)
        {
        }

        public DbSet<TenantManager.Models.Tenant> Tenant { get; set; }

        public DbSet<TenantManager.Models.Method> Method { get; set; }

        public DbSet<TenantManager.Models.Customisation> Customisation { get; set; }
    }

    public class TenantManagerContextDesignFactory : IDesignTimeDbContextFactory<TenantManagerContext>
    {
        public TenantManagerContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TenantManagerContext>()
                            .UseSqlServer("Server=.;Initial Catalog=Microsoft.eShopOnContainers.Services.TenantManagerDb;Integrated Security=true");

            return new TenantManagerContext(optionsBuilder.Options);
        }
    }
}
