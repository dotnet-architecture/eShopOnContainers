using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel;

namespace Microsoft.eShopOnContainers.Services.Ordering.SqlData.UnitOfWork
{
    public class OrderingDbContext : DbContext
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
                //SQL LOCALDB
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Microsoft.eShopOnContainers.Services.OrderingDb;Trusted_Connection=True;");

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
    }
}
