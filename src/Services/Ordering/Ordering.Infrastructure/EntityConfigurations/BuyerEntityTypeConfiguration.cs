﻿namespace Microsoft.eShopOnContainers.Services.Ordering.Infrastructure.EntityConfigurations;

class BuyerEntityTypeConfiguration
    : IEntityTypeConfiguration<Buyer>
{
    public void Configure(EntityTypeBuilder<Buyer> buyerConfiguration)
    {
        buyerConfiguration.ToTable("buyers", OrderingContext.DEFAULT_SCHEMA);

        buyerConfiguration.HasKey(b => b.Id);

        buyerConfiguration.Ignore(b => b.DomainEvents);

        buyerConfiguration.Property(b => b.Id)
            .UseHiLo("buyerseq", OrderingContext.DEFAULT_SCHEMA);

        buyerConfiguration.Property(b => b.IdentityGuid)
            .HasMaxLength(200)
            .IsRequired();

        buyerConfiguration.HasIndex("IdentityGuid")
            .IsUnique(true);

        buyerConfiguration.Property(b => b.Name);

        buyerConfiguration.HasMany(b => b.PaymentMethods)
            .WithOne()
            .HasForeignKey("BuyerId")
            .OnDelete(DeleteBehavior.Cascade);

        var navigation = buyerConfiguration.Metadata.FindNavigation(nameof(Buyer.PaymentMethods));

        navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
