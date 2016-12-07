using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using eShopOnContainers.Identity.Data;
using eShopOnContainers.Identity.Models;
using eShopOnContainers.Identity.Services;
using eShopOnContainers.Identity.Configuration;
using IdentityServer4.Services;
using System.Threading;

namespace eShopOnContainers.Identity
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
             options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
         

            services.AddMvc();

            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddTransient<ILoginService<ApplicationUser>, EFLoginService>();

            //callbacks urls from config:
            Dictionary<string, string> clientUrls = new Dictionary<string, string>();
            clientUrls.Add("Mvc", Configuration.GetValue<string>("MvcClient"));
            clientUrls.Add("Spa", Configuration.GetValue<string>("SpaClient"));

            // Adds IdentityServer
            services.AddIdentityServer()
                .AddTemporarySigningCredential()
                .AddInMemoryScopes(Config.GetScopes())
                .AddInMemoryClients(Config.GetClients(clientUrls))
                .AddAspNetIdentity<ApplicationUser>()
                .Services.AddTransient<IProfileService, ProfileService>(); 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();

            // Adds IdentityServer
            app.UseIdentityServer();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            MigrateDb(app);
        }

        int retries = 0;

        private void MigrateDb(IApplicationBuilder app)
        {
            try
            {
                var context = (ApplicationDbContext)app
                    .ApplicationServices.GetService(typeof(ApplicationDbContext));
                using (context)
                {
                    context.Database.Migrate();
                }

            }
            catch (Exception)
            {
                retries++;
                if (retries < 2)
                    MigrateDb(app);
            }
        }
    }
}
