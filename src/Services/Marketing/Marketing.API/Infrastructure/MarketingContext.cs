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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Campaign>(ConfigureCampaigns);
            builder.Entity<Rule>(ConfigureRules);
            builder.Entity<UserLocationRule>(ConfigureUserLocationRules);
        }

        void ConfigureCampaigns(EntityTypeBuilder<Campaign> builder)
        {
            builder.ToTable("Campaign");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
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
                .HasForeignKey(r => r.CampaignId)
                .IsRequired();
        }

        void ConfigureRules(EntityTypeBuilder<Rule> builder)
        {
            builder.ToTable("Rule");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
               .ForSqlServerUseSequenceHiLo("rule_hilo")
               .IsRequired();

            builder.HasDiscriminator<int>("RuleTypeId")
                .HasValue<UserProfileRule>((int)RuleTypeEnum.UserProfileRule)
                .HasValue<PurchaseHistoryRule>((int)RuleTypeEnum.PurchaseHistoryRule)
                .HasValue<UserLocationRule>((int)RuleTypeEnum.UserLocationRule);

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