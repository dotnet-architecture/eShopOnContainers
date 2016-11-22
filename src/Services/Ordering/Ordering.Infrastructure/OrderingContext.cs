namespace Microsoft.eShopOnContainers.Services.Ordering.Infrastructure
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain;

    public class OrderingContext
       : DbContext

    {
        const string DEFAULT_SCHEMA = "ordering";

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<Buyer> Buyers { get; set; }

        public DbSet<CardType> Cards { get; set; }

        public DbSet<OrderStatus> OrderStatus { get; set; }

        public DbSet<Address> Addresses { get; set; }

        public OrderingContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Buyer>(ConfigureBuyer);
            modelBuilder.Entity<Payment>(ConfigurePayment);
            modelBuilder.Entity<Order>(ConfigureOrder);
            modelBuilder.Entity<OrderItem>(ConfigureOrderItems);

            modelBuilder.Entity<OrderStatus>()
                .ToTable("orderstatus", DEFAULT_SCHEMA);

            modelBuilder.Entity<CardType>()
                .ToTable("cardtypes", DEFAULT_SCHEMA);

            modelBuilder.Entity<Address>()
                .ToTable("address", DEFAULT_SCHEMA);
        }

        void ConfigureBuyer(EntityTypeBuilder<Buyer> buyerConfiguration)
        {
            buyerConfiguration.ToTable("buyers", DEFAULT_SCHEMA);

            buyerConfiguration.HasKey(b => b.Id);

            buyerConfiguration.Property(b => b.Id)
                .ForSqlServerUseSequenceHiLo("buyerseq", DEFAULT_SCHEMA);

            buyerConfiguration.Property(b => b.FullName)
                .HasMaxLength(200)
                .IsRequired();
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
                .HasForeignKey(o => o.PaymentId);

            orderConfiguration.HasOne(o => o.BillingAddress)
                .WithMany()
                .HasForeignKey(o => o.BillingAddressId)
                .OnDelete(EntityFrameworkCore.Metadata.DeleteBehavior.SetNull);

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
    }
}
