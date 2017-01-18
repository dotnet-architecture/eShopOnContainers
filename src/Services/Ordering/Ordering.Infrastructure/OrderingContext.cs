namespace Microsoft.eShopOnContainers.Services.Ordering.Infrastructure
{
    using System;
    using System.Threading.Tasks;
    using Domain.SeedWork;
    using EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.BuyerAggregate;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;

    public class OrderingContext
       : DbContext,IUnitOfWork

    {
        const string DEFAULT_SCHEMA = "ordering";

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<Buyer> Buyers { get; set; }

        public DbSet<CardType> CardTypes { get; set; }

        public DbSet<OrderStatus> OrderStatus { get; set; }

        public DbSet<Address> Addresses { get; set; }

        public OrderingContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Buyer>(ConfigureBuyer);
            modelBuilder.Entity<Payment>(ConfigurePayment);
            modelBuilder.Entity<Order>(ConfigureOrder);
            modelBuilder.Entity<OrderItem>(ConfigureOrderItems);
            modelBuilder.Entity<CardType>(ConfigureCardTypes);
            modelBuilder.Entity<OrderStatus>(ConfigureOrderStatus);
            modelBuilder.Entity<Address>(ConfigureAddress);
        }

        void ConfigureBuyer(EntityTypeBuilder<Buyer> buyerConfiguration)
        {
            buyerConfiguration.ToTable("buyers", DEFAULT_SCHEMA);

            buyerConfiguration.HasIndex(b => b.FullName)
                .IsUnique(true);

            buyerConfiguration.HasKey(b => b.Id);

            buyerConfiguration.Property(b => b.Id)
                .ForSqlServerUseSequenceHiLo("buyerseq", DEFAULT_SCHEMA);

            buyerConfiguration.Property(b => b.FullName)
                .HasMaxLength(200)
                .IsRequired();

            buyerConfiguration.HasMany(b => b.Payments)
                .WithOne()
                .HasForeignKey(p => p.BuyerId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        void ConfigurePayment(EntityTypeBuilder<Payment> paymentConfiguration)
        {
            paymentConfiguration.ToTable("payments", DEFAULT_SCHEMA);

            paymentConfiguration.HasKey(b => b.Id);

            paymentConfiguration.Property(b => b.Id)
                .ForSqlServerUseSequenceHiLo("paymentseq", DEFAULT_SCHEMA);

            paymentConfiguration.Property(p => p.CardHolderName)
                .HasMaxLength(200)
                .IsRequired();

            paymentConfiguration.Property(p => p.CardNumber)
                .HasMaxLength(25)
                .IsRequired();

            paymentConfiguration.Property(p => p.Expiration)
                .IsRequired();

            paymentConfiguration.HasOne(p => p.CardType)
                .WithMany()
                .HasForeignKey(p => p.CardTypeId);
        }

        void ConfigureOrder(EntityTypeBuilder<Order> orderConfiguration)
        {
            orderConfiguration.ToTable("orders", DEFAULT_SCHEMA);

            orderConfiguration.HasKey(o => o.Id);

            orderConfiguration.Property(o => o.Id)
                .ForSqlServerUseSequenceHiLo("orderseq", DEFAULT_SCHEMA);

            orderConfiguration.Property(o => o.OrderDate)
                .IsRequired();

            orderConfiguration.HasOne(o => o.Payment)
                .WithMany()
                .HasForeignKey(o => o.PaymentId)
                .OnDelete(DeleteBehavior.Restrict);

            orderConfiguration.HasOne(o => o.Buyer)
                .WithMany()
                .HasForeignKey(o => o.BuyerId);

            orderConfiguration.HasOne(o => o.Status)
                .WithMany()
                .HasForeignKey(o => o.StatusId);
        }

        void ConfigureOrderItems(EntityTypeBuilder<OrderItem> orderItemConfiguration)
        {
            orderItemConfiguration.ToTable("orderItems", DEFAULT_SCHEMA);

            orderItemConfiguration.HasKey(o => o.Id);

            orderItemConfiguration.Property(o => o.Discount)
                .IsRequired();

            orderItemConfiguration.Property(o => o.ProductId)
                .IsRequired();

            orderItemConfiguration.Property(o => o.ProductName)
                .IsRequired();

            orderItemConfiguration.Property(o => o.UnitPrice)
                .IsRequired();

            orderItemConfiguration.Property(o => o.Units)
                .ForSqlServerHasDefaultValue(1)
                .IsRequired();
        }

        void ConfigureOrderStatus(EntityTypeBuilder<OrderStatus> orderStatusConfiguration)
        {
            orderStatusConfiguration.ToTable("orderstatus", DEFAULT_SCHEMA);

            orderStatusConfiguration.HasKey(o => o.Id);

            orderStatusConfiguration.Property(o => o.Id)
                .HasDefaultValue(1)
                .IsRequired();

            orderStatusConfiguration.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
        }

        void ConfigureCardTypes(EntityTypeBuilder<CardType> cardTypesConfiguration)
        {
            cardTypesConfiguration.ToTable("cardtypes", DEFAULT_SCHEMA);

            cardTypesConfiguration.HasKey(ct => ct.Id);

            cardTypesConfiguration.Property(ct => ct.Id)
                .HasDefaultValue(1)
                .IsRequired();

            cardTypesConfiguration.Property(ct => ct.Name)
                .HasMaxLength(200)
                .IsRequired();
        }

        void ConfigureAddress(EntityTypeBuilder<Address> addressConfiguration)
        {
            addressConfiguration.ToTable("address", DEFAULT_SCHEMA);
        }
    }
}
