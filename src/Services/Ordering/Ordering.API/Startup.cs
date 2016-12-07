namespace Microsoft.eShopOnContainers.Services.Ordering.API
{
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Infrastructure;
    using Infrastructure.AutofacModules;
    using Infrastructure.Filters;
    using MediatR;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Ordering.Infrastructure;
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("settings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"settings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add framework services.

            services.AddMvc(options =>
            {

                options.Filters.Add(typeof(HttpGlobalExceptionFilter));

            }).AddControllersAsServices();

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<OrderingContext>(options =>
                {
                    options.UseSqlServer(Configuration["ConnectionString"],
                        sqlop => sqlop.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name));
                });

            services.AddSwaggerGen();
            services.ConfigureSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SingleApiVersion(new Swashbuckle.Swagger.Model.Info()
                {
                    Title = "Ordering HTTP API",
                    Version = "v1",
                    Description = "The Ordering Service HTTP API",
                    TermsOfService = "Terms Of Service"
                });
            });

            services.AddSingleton<IConfiguration>(this.Configuration);

            services.AddOptions();



            //configure autofac

            var container = new ContainerBuilder();
            container.Populate(services);

            container.RegisterModule(new MediatorModule());
            container.RegisterModule(new ApplicationModule());


            return new AutofacServiceProvider(container.Build());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseMvcWithDefaultRoute();

            app.UseSwagger()
                .UseSwaggerUi();

            OrderingContextSeed.SeedAsync(app).Wait();
        }
    }
}
