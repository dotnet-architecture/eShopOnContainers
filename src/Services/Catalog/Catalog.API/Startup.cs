namespace Microsoft.eShopOnContainers.Services.Catalog.API
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile($"settings.{env.EnvironmentName}.json", optional: false)
                .AddEnvironmentVariables();


            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(Configuration);

            services.AddDbContext<CatalogContext>(c =>
            {
                c.UseSqlServer(Configuration["ConnectionString"]);
                c.ConfigureWarnings(wb =>
                {
                    wb.Throw(RelationalEventId.QueryClientEvaluationWarning);
                });
            });

            // Add framework services.

            services.AddSwaggerGen();
            services.ConfigureSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SingleApiVersion(new Swashbuckle.Swagger.Model.Info()
                {
                    Title = "Values API",
                    Version = "v1",
                    Description = "An API API With Swagger for RC2",
                    TermsOfService = "None"
                });
            });

            services.AddCors();

            services.AddMvc(mvcoptions =>
            {
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            //Configure logs

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            //Seed Data

            CatalogContextSeed.SeedAsync(app)
                .Wait();

            // Use frameworks
            app.UseCors(policyBuilder => policyBuilder.AllowAnyOrigin());

            app.UseMvc();

            app.UseSwagger()
              .UseSwaggerUi();
        }
    }
}
