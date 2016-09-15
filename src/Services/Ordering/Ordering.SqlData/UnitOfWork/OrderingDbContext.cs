using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;

namespace Microsoft.eShopOnContainers.Services.Ordering.SqlData.UnitOfWork
{
    public class OrderingDbContext : DbContext, IUnitOfWork
    {
        public OrderingDbContext(DbContextOptions<OrderingDbContext> options)
            : base(options)
        { }

        public DbSet<Order> Orders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //If running from ASP.NET Core, config is done at StartUp.cs --> ConfigureServices() outside 
            //and injected through DI later on. The following config is used when running Tests or similar contexts
            if (!optionsBuilder.IsConfigured)
            {
                //SQL LocalDB 
                //var connString = @"Server=(localdb)\mssqllocaldb;Database=Microsoft.eShopOnContainers.Services.OrderingDb;Trusted_Connection=True;";

                //SQL SERVER on-premises

                //(Integrated Security)
                //var connString = @"Server=CESARDLBOOKVHD;Database=Microsoft.eShopOnContainers.Services.OrderingDb;Trusted_Connection=True;";

                //(SQL Server Authentication)
                var connString = @"Server=10.0.75.1;Database=Microsoft.eShopOnContainers.Services.OrderingDb;User Id=sa;Password=Pass@word;";

                //SQL LOCALDB
                optionsBuilder.UseSqlServer(connString);

            }

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Add your customizations after calling base.OnModelCreating(builder);

            //Sequence to be used as part of the OrderNumber
            modelBuilder.HasSequence<int>("OrderSequences", schema: "shared")
                        .StartsAt(1001)
                        .IncrementsBy(1);

            modelBuilder.Entity<Order>()
                .Property(o => o.SequenceNumber)
                .HasDefaultValueSql("NEXT VALUE FOR shared.OrderSequences");

        }

        public async Task<int> CommitAsync()
        {
            int changes = 0;

            try
            {
                //(CDLTLL) TBD
                //RemoveOrphanedChilds();

                changes = await base.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                //(CDLTLL) TBD
                //RejectChanges();
                throw ex;
            }

            return changes;
        }
    }
}
