using Microsoft.eShopWeb.Infrastructure;
using Microsoft.eShopWeb.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.eShopWeb
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CatalogContext>(c =>
            {
                try
                {
                    var text = Configuration["ConnectionString"];
                    c.UseSqlServer(Configuration["ConnectionString"]);
                    c.ConfigureWarnings(wb =>
                    {
                        //By default, in this application, we don't want to have client evaluations
                        wb.Log(RelationalEventId.QueryClientEvaluationWarning);
                    });
                }
                catch (System.Exception ex )
                {
                    var message = ex.Message;
                }                
            });

            services.AddTransient<ICatalogService, CatalogService>();
            services.Configure<CatalogSettings>(Configuration);
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Catalog/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Catalog}/{action=Index}/{id?}");
            });

            //Seed Data
            CatalogContextSeed.SeedAsync(app, loggerFactory)
            .Wait();
        }
    }
}
