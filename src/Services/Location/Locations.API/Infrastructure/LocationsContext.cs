namespace Microsoft.eShopOnContainers.Services.Locations.API.Infrastructure
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.eShopOnContainers.Services.Locations.API.Infrastructure.Repositories;
    using Microsoft.eShopOnContainers.Services.Locations.API.Model;
    using System.Threading;
    using System.Threading.Tasks;

    public class LocationsContext : DbContext, IUnitOfWork
    {
        public LocationsContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Locations> Locations { get; set; }
        public DbSet<FrontierPoints> FrontierPoints { get; set; }
        public DbSet<UserLocation> UserLocation { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Locations>(ConfigureLocations);
            builder.Entity<FrontierPoints>(ConfigureFrontierPoints);
            builder.Entity<UserLocation>(ConfigureUserLocation);
        }

        void ConfigureLocations(EntityTypeBuilder<Locations> builder)
        {
            builder.ToTable("Locations");

            builder.HasKey(cl => cl.Id);

            builder.Property(cl => cl.Id)
               .ForSqlServerUseSequenceHiLo("locations_seq")
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
               .ForSqlServerUseSequenceHiLo("frontier_seq")
               .IsRequired();           
        }

        void ConfigureUserLocation(EntityTypeBuilder<UserLocation> builder)
        {
            builder.ToTable("UserLocation");

            builder.Property(ul => ul.Id)
               .ForSqlServerUseSequenceHiLo("UserLocation_seq")
               .IsRequired();

            builder.HasIndex(ul => ul.UserId).IsUnique();
        }        
    }
}
