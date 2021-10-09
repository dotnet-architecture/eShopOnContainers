﻿namespace Microsoft.eShopOnContainers.Services.Ordering.Infrastructure.EntityConfigurations;

class CardTypeEntityTypeConfiguration
    : IEntityTypeConfiguration<CardType>
{
    public void Configure(EntityTypeBuilder<CardType> cardTypesConfiguration)
    {
        cardTypesConfiguration.ToTable("cardtypes", OrderingContext.DEFAULT_SCHEMA);

        cardTypesConfiguration.HasKey(ct => ct.Id);

        cardTypesConfiguration.Property(ct => ct.Id)
            .HasDefaultValue(1)
            .ValueGeneratedNever()
            .IsRequired();

        cardTypesConfiguration.Property(ct => ct.Name)
            .HasMaxLength(200)
            .IsRequired();
    }
}
