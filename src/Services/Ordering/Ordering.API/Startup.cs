using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Microsoft.eShopOnContainers.Services.Ordering.SqlData.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.Contracts;
using Microsoft.eShopOnContainers.Services.Ordering.SqlData.Repositories;
using Microsoft.eShopOnContainers.Services.Ordering.SqlData.Queries;

namespace Microsoft.eShopOnContainers.Services.Ordering.API
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
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. 
        // Use this method to add services to the IoC container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            
            var connString = Configuration["ConnectionString"];

            services.AddDbContext<OrderingDbContext>(options =>
            {
                options.UseSqlServer(connString)
                    .UseSqlServer(connString, b => b.MigrationsAssembly("Ordering.API"));
            });

            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<IOrderdingQueries, OrderingQueries>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
             
            app.UseMvc();
        }
    }
}
