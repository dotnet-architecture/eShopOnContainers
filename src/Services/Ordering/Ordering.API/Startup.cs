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
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. 
        // Use this method to add services to the IoC container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            //Add EF Core Context (UnitOfWork)
            //SQL LocalDB 
            // var connString = @"Server=(localdb)\mssqllocaldb;Database=Microsoft.eShopOnContainers.Services.OrderingDb;Trusted_Connection=True;";

            //SQL SERVER on-premises
            //(Integrated Security)
            //var connString = @"Server=CESARDLBOOKVHD;Database=Microsoft.eShopOnContainers.Services.OrderingDb;Trusted_Connection=True;";

            //(SQL Server Authentication)
            //var connString = @"Server=10.0.75.1;Database=Microsoft.eShopOnContainers.Services.OrderingDb;User Id=sa;Password=Pass@word;";

            var connString = Configuration["ConnectionString"];

            //(CDLTLL) To use only for EF Migrations
            //connString = @"Server=10.0.75.1;Database=Microsoft.eShopOnContainers.Services.OrderingDb;User Id=sa;Password=Pass@word;";

            services.AddDbContext<OrderingDbContext>(options => options.UseSqlServer(connString)
                                                                       .UseSqlServer(connString, b => b.MigrationsAssembly("Ordering.API"))
                                                    //(CDLTLL) MigrationsAssembly will be Ordering.SqlData, but when supported
                                                    //Standard Library 1.6 by "Microsoft.EntityFrameworkCore.Tools"
                                                    //Version "1.0.0-preview2-final" just supports .NET Core 
                                                    );

            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<IOrderdingQueries, OrderingQueries>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            //if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            
            app.UseMvc();
        }
    }
}
