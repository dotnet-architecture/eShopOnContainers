namespace Microsoft.eShopOnContainers.Services.Catalog.API
{
    using AspNetCore.Http;
    using Extensions.FileProviders;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Data.SqlClient;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

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

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
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

            var context = (CatalogContext)app
                        .ApplicationServices.GetService(typeof(CatalogContext));

            WaitForSqlAvailability(context, loggerFactory);
            //Seed Data
            CatalogContextSeed.SeedAsync(app, loggerFactory)
            .Wait();            
        }

        private void WaitForSqlAvailability(CatalogContext ctx, ILoggerFactory loggerFactory, int? retry = 0)
        {
            int retryForAvailability = retry.Value;            
            try
            {
                ctx.Database.OpenConnection();
            }
            catch(SqlException ex)
            {
                if (retryForAvailability < 10)
                {
                    retryForAvailability++;
                    var log = loggerFactory.CreateLogger(nameof(Startup));
                    log.LogError(ex.Message);
                    WaitForSqlAvailability(ctx, loggerFactory, retryForAvailability);
                }
            }
            finally {
                ctx.Database.CloseConnection(); 
            }
        }
    }
}
