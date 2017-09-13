using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.eShopOnContainers.Services.Marketing.API.Model;

namespace Microsoft.eShopOnContainers.Services.Marketing.API.Infrastructure.EntityConfigurations
{
    class CampaignEntityTypeConfiguration
        : IEntityTypeConfiguration<Campaign>
    {
        public void Configure(EntityTypeBuilder<Campaign> builder)
        {
            builder.ToTable("Campaign");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .ForSqlServerUseSequenceHiLo("campaign_hilo")
                .IsRequired();

            builder.Property(m => m.Name)
                .HasColumnName("Name")
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

            builder.Property(m => m.PictureUri)
                .HasColumnName("PictureUri")
                .IsRequired();

            builder.HasMany(m => m.Rules)
                .WithOne(r => r.Campaign)
                .HasForeignKey(r => r.CampaignId)
                .IsRequired();
        }
    }
}
