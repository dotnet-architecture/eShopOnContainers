namespace Microsoft.eShopOnContainers.Services.Locations.API.Infrastructure
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.eShopOnContainers.Services.Locations.API.Model;

    public class LocationsContext : DbContext
    {
        public LocationsContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Locations> Locations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Locations>(ConfigureLocations);
            builder.Entity<FrontierPoints>(ConfigureFrontierPoints);
        }

        void ConfigureLocations(EntityTypeBuilder<Locations> builder)
        {
            builder.ToTable("Locations");

            builder.HasKey(cl => cl.Id);

            builder.Property(cl => cl.Id)
               .ForSqlServerUseSequenceHiLo("locations_hilo")
               .IsRequired();

            builder.Property(cb => cb.Code)
                .IsRequired()
                .HasColumnName("LocationCode")
                .HasMaxLength(15);

            builder.HasMany(f => f.Polygon)
                .WithOne(l => l.Location)
                .IsRequired();          

            builder.Property(cb => cb.Description)
                .HasMaxLength(100);
        }

        void ConfigureFrontierPoints(EntityTypeBuilder<FrontierPoints> builder)
        {
            builder.ToTable("FrontierPoints");

            builder.HasKey(fp => fp.Id);

            builder.Property(fp => fp.Id)
               .ForSqlServerUseSequenceHiLo("frontier_hilo")
               .IsRequired();           
        }        
    }
}
