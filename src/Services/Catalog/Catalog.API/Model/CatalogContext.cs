namespace Microsoft.eShopOnContainers.Services.Catalog.API.Model
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasSequence("idseq")
                .StartsAt(1)
                .IncrementsBy(1);

            builder.Entity<CatalogItem>(ConfigureCatalogItem);

            builder.HasPostgresExtension("uuid-ossp");
        }

        void ConfigureCatalogItem(EntityTypeBuilder<CatalogItem> builder)
        {
            builder.ForNpgsqlToTable("catalog");

            builder.Property(ci => ci.Id)
                .HasDefaultValueSql("nextval('idseq')")
                .IsRequired();

            builder.Property(ci => ci.Name)
                .IsRequired(true)
                .HasMaxLength(50);

            builder.Property(ci => ci.Price)
                .IsRequired(true);

            builder.Property(ci => ci.PictureUri)
                .IsRequired(false);

        }
    }
}
