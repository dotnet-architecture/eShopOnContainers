namespace Microsoft.eShopOnContainers.Services.Marketing.API.Infrastructure
{
    using System;
    using System.IO;
    using EntityConfigurations;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.eShopOnContainers.Services.Marketing.API.Model;
    using Microsoft.Extensions.Configuration;

    public class MarketingContext : DbContext
    {
        public MarketingContext(DbContextOptions<MarketingContext> options) : base(options)
        {    
        }

        public DbSet<Campaign> Campaigns { get; set; }

        public DbSet<Rule> Rules { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new CampaignEntityTypeConfiguration());
            builder.ApplyConfiguration(new RuleEntityTypeConfiguration());
            builder.ApplyConfiguration(new UserLocationRuleEntityTypeConfiguration());
        }
    }

    public class MarketingContextDesignFactory : IDesignTimeDbContextFactory<MarketingContext>
    {
        public MarketingContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            var connectionString = configuration["ConnectionString"];
            Console.WriteLine(" -- Connection string");
            Console.WriteLine(connectionString);

            var optionsBuilder = new DbContextOptionsBuilder<MarketingContext>()
                .UseSqlServer(connectionString);
                // .UseSqlServer("Server=.;Initial Catalog=Microsoft.eShopOnContainers.Services.MarketingDb;Integrated Security=true");
            return new MarketingContext(optionsBuilder.Options);
        }
    }
}