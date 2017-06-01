namespace Microsoft.eShopOnContainers.Services.Marketing.API.Infrastructure
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.eShopOnContainers.Services.Marketing.API.Model;

    public class MarketingContext : DbContext
    {
        public MarketingContext(DbContextOptions<MarketingContext> options) : base(options)
        {    
        }

        public DbSet<Campaign> Campaigns { get; set; }

        public DbSet<Rule> Rules { get; set; }
        public DbSet<UserProfileRule> UserProfileRules { get; set; }
        public DbSet<PurchaseHistoryRule> PurchaseHistoryRules { get; set; }
        public DbSet<UserLocationRule> UserLocationRules { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Campaign>(ConfigureCampaigns);
            builder.Entity<Rule>(ConfigureRules);
            builder.Entity<UserLocationRule>(ConfigureUserLocationRules);
        }

        void ConfigureCampaigns(EntityTypeBuilder<Campaign> builder)
        {
            builder.ToTable("Campaign");

            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.Id)
                .ForSqlServerUseSequenceHiLo("campaign_hilo")
                .IsRequired();

            builder.Property(m => m.Description)
                .HasColumnName("Description")
                .IsRequired();

            builder.Property(m => m.From)
                .HasColumnName("From")
                .IsRequired();

            builder.Property(m => m.To)
                .HasColumnName("To")
                .IsRequired();

            builder.Property(m => m.Description)
                .HasColumnName("Description")
                .IsRequired();

            builder.HasMany(m => m.Rules)
                .WithOne(r => r.Campaign)
                .HasForeignKey(m => m.CampaignId)
                .IsRequired();
        }

        void ConfigureRules(EntityTypeBuilder<Rule> builder)
        {
            builder.ToTable("Rules");

            builder.HasKey(r => r.Id);

            builder.HasDiscriminator<int>("RuleTypeId")
                .HasValue<UserProfileRule>(1)
                .HasValue<PurchaseHistoryRule>(2)
                .HasValue<UserLocationRule>(3);

            builder.Property(r => r.Description)
                .HasColumnName("Description")
                .IsRequired();
        }

        void ConfigureUserLocationRules(EntityTypeBuilder<UserLocationRule> builder)
        {
            builder.Property(r => r.LocationId)
                .HasColumnName("LocationId")
                .IsRequired();
        }
    }
}