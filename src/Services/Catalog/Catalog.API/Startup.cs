namespace Microsoft.eShopOnContainers.Services.Catalog.API
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBusRabbitMQ;
    using Microsoft.eShopOnContainers.BuildingBlocks.IntegrationEventLogEF;
    using Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System.Reflection;

    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile($"settings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"settings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets(typeof(Startup).GetTypeInfo().Assembly);
            }

            builder.AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CatalogContext>(c =>
            {
                c.UseSqlServer(Configuration["ConnectionString"]);
                // Changing default behavior when client evaluation occurs to throw. 
                // Default in EF Core would be to log a warning when client evaluation is performed.
                c.ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning));
                //Check Client vs. Server evaluation: https://docs.microsoft.com/en-us/ef/core/querying/client-eval
            });

            services.AddDbContext<IntegrationEventLogContext>(c =>
            {
                c.UseSqlServer(Configuration["ConnectionString"], b => b.MigrationsAssembly("Catalog.API"));               
                c.ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning));                
            });

            services.Configure<Settings>(Configuration);

            // Add framework services.
            services.AddSwaggerGen();
            services.ConfigureSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SingleApiVersion(new Swashbuckle.Swagger.Model.Info()
                {
                    Title = "eShopOnContainers - Catalog HTTP API",
                    Version = "v1",
                    Description = "The Catalog Microservice HTTP API. This is a Data-Driven/CRUD microservice sample",
                    TermsOfService = "Terms Of Service"
                });
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetRequiredService<IOptionsSnapshot<Settings>>().Value;
            services.AddSingleton<IEventBus>(new EventBusRabbitMQ(configuration.EventBusConnection));
            
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IntegrationEventLogContext integrationEventLogContext)
        {
            //Configure logs

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseCors("CorsPolicy");

            app.UseMvcWithDefaultRoute();

            app.UseSwagger()
              .UseSwaggerUi();

            //Seed Data
            CatalogContextSeed.SeedAsync(app, loggerFactory)
                .Wait();

            // TODO: move this creation to a db initializer
             integrationEventLogContext.Database.Migrate();

        }
    }
}
