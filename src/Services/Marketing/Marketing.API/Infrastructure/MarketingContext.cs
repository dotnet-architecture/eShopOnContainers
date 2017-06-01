namespace Microsoft.eShopOnContainers.Services.Marketing.API.Infrastructure
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.eShopOnContainers.Services.Marketing.API.Model;

    public class MarketingContext : DbContext
    {
        public MarketingContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Campaing> Campaings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Campaing>(ConfigureCampaings);
        }

        void ConfigureCampaings(EntityTypeBuilder<Campaing> builder)
        {

        }
    }
}
