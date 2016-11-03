namespace Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure
{
    using EntityFrameworkCore.Metadata.Builders;
    using Microsoft.EntityFrameworkCore;
    using Npgsql.EntityFrameworkCore.PostgreSQL;

    public class CatalogContext : DbContext
    {
        public CatalogContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<CatalogItem> CatalogItems { get; set; }

        public DbSet<CatalogBrand> CatalogBrands { get; set; }

        public DbSet<CatalogType> CatalogTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasSequence("idseqcatalog")
                .StartsAt(1)
                .IncrementsBy(1);

            builder.HasSequence("idseqcatalogbrand")
                .StartsAt(1)
                .IncrementsBy(1);

            builder.HasSequence("idseqcatalogtype")
                .StartsAt(1)
                .IncrementsBy(1);

            builder.Entity<CatalogBrand>(ConfigureCatalogBrand);
            builder.Entity<CatalogType>(ConfigureCatalogType);
            builder.Entity<CatalogItem>(ConfigureCatalogItem);
           

            builder.HasPostgresExtension("uuid-ossp");
        }

        void ConfigureCatalogItem(EntityTypeBuilder<CatalogItem> builder)
        {
            builder.ForNpgsqlToTable("catalog");

            builder.Property(ci => ci.Id)
                .HasDefaultValueSql("nextval('idseqcatalog')")
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
            builder.ForNpgsqlToTable("catalogbrand");

            builder.Property(cb => cb.Id)
                .HasDefaultValueSql("nextval('idseqcatalogbrand')")
                .IsRequired();

            builder.Property(cb => cb.Brand)
                .IsRequired()
                .HasMaxLength(100);
        }

        void ConfigureCatalogType(EntityTypeBuilder<CatalogType> builder)
        {
            builder.ForNpgsqlToTable("catalogtype");

            builder.Property(cb => cb.Id)
                .HasDefaultValueSql("nextval('idseqcatalogtype')")
                .IsRequired();

            builder.Property(cb => cb.Type)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
