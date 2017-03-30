namespace Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure
{
    using EntityFrameworkCore.Metadata.Builders;
    using Microsoft.EntityFrameworkCore;
    using Model;
    using Microsoft.eShopOnContainers.BuildingBlocks.IntegrationEventLogEF;

    public class CatalogContext : DbContext
    {
        public CatalogContext(DbContextOptions<CatalogContext> options) : base(options)
        {
        }
        public DbSet<CatalogItem> CatalogItems { get; set; }
        public DbSet<CatalogBrand> CatalogBrands { get; set; }
        public DbSet<CatalogType> CatalogTypes { get; set; }
        //public DbSet<IntegrationEventLogEntry> IntegrationEventLog { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CatalogBrand>(ConfigureCatalogBrand);
            builder.Entity<CatalogType>(ConfigureCatalogType);
            builder.Entity<CatalogItem>(ConfigureCatalogItem);
            //builder.Entity<IntegrationEventLogEntry>(ConfigureIntegrationEventLogEntry);
        }     

        void ConfigureCatalogItem(EntityTypeBuilder<CatalogItem> builder)
        {
            builder.ToTable("Catalog");

            builder.Property(ci => ci.Id)
                .ForSqlServerUseSequenceHiLo("catalog_hilo")
                .IsRequired();

            builder.Property(ci => ci.Name)
                .IsRequired(true)
                .HasMaxLength(50);

            builder.Property(ci => ci.Price)
                .IsRequired(true);

            builder.Property(ci => ci.PictureUri)
                .IsRequired(false);

            builder.HasOne(ci => ci.CatalogBrand)
                .WithMany()
                .HasForeignKey(ci => ci.CatalogBrandId);

            builder.HasOne(ci => ci.CatalogType)
                .WithMany()
                .HasForeignKey(ci => ci.CatalogTypeId);
        }

        void ConfigureCatalogBrand(EntityTypeBuilder<CatalogBrand> builder)
        {
            builder.ToTable("CatalogBrand");

            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.Id)
               .ForSqlServerUseSequenceHiLo("catalog_brand_hilo")
               .IsRequired();

            builder.Property(cb => cb.Brand)
                .IsRequired()
                .HasMaxLength(100);
        }

        void ConfigureCatalogType(EntityTypeBuilder<CatalogType> builder)
        {
            builder.ToTable("CatalogType");

            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.Id)
               .ForSqlServerUseSequenceHiLo("catalog_type_hilo")
               .IsRequired();

            builder.Property(cb => cb.Type)
                .IsRequired()
                .HasMaxLength(100);
        }

        //void ConfigureIntegrationEventLogEntry(EntityTypeBuilder<IntegrationEventLogEntry> builder)
        //{
        //    builder.ToTable("IntegrationEventLog");

        //    builder.HasKey(e => e.EventId);

        //    builder.Property(e => e.EventId)
        //        .IsRequired();

        //    builder.Property(e => e.Content)
        //        .IsRequired();

        //    builder.Property(e => e.CreationTime)
        //        .IsRequired();

        //    builder.Property(e => e.State)
        //        .IsRequired();

        //    builder.Property(e => e.TimesSent)
        //        .IsRequired();

        //    builder.Property(e => e.EventTypeName)
        //        .IsRequired();

        //}
    }
}
