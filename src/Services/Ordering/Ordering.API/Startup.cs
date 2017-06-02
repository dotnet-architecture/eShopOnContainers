namespace Microsoft.eShopOnContainers.Services.Ordering.API
{
    using AspNetCore.Http;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using global::Ordering.API.Infrastructure.Middlewares;
    using global::Ordering.API.IntegrationEvents;
    using Infrastructure;
    using Infrastructure.Auth;
    using Infrastructure.AutofacModules;
    using Infrastructure.Filters;
    using Infrastructure.Services;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBusRabbitMQ;
    using Microsoft.eShopOnContainers.BuildingBlocks.IntegrationEventLogEF;
    using Microsoft.eShopOnContainers.BuildingBlocks.IntegrationEventLogEF.Services;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.HealthChecks;
    using Microsoft.Extensions.Logging;
    using Ordering.Infrastructure;
    using Polly;
    using RabbitMQ.Client;
    using System;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Reflection;
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
                builder.AddUserSecrets(typeof(Startup).GetTypeInfo().Assembly);
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
            }).AddControllersAsServices();  //Injecting Controllers themselves thru DI
                                            //For further info see: http://docs.autofac.org/en/latest/integration/aspnetcore.html#controllers-as-services


            services.AddHealthChecks(checks =>
            {
                var minutes = 1;
                if (int.TryParse(Configuration["HealthCheck:Timeout"], out var minutesParsed))
                {
                    minutes = minutesParsed;
                }
                checks.AddSqlCheck("OrderingDb", Configuration["ConnectionString"], TimeSpan.FromMinutes(minutes));
            });
            
            services.AddEntityFrameworkSqlServer()
                    .AddDbContext<OrderingContext>(options =>
                    {
                        options.UseSqlServer(Configuration["ConnectionString"],
                            sqlServerOptionsAction: sqlOptions =>
                            {
                                sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                                sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                            });                                       
                        },
                        ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
                    );

            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Title = "Ordering HTTP API",
                    Version = "v1",
                    Description = "The Ordering Service HTTP API",
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

            // Add application services.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
                sp => (DbConnection c) => new IntegrationEventLogService(c));            
            var serviceProvider = services.BuildServiceProvider();
            services.AddTransient<IOrderingIntegrationEventService, OrderingIntegrationEventService>();

            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = Configuration["EventBusConnection"]
                };

                return new DefaultRabbitMQPersistentConnection(factory, logger);
            });

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            services.AddSingleton<IEventBus, EventBusRabbitMQ>();

            services.AddOptions();

            //configure autofac

            var container = new ContainerBuilder();
            container.Populate(services);

            container.RegisterModule(new MediatorModule());
            container.RegisterModule(new ApplicationModule(Configuration["ConnectionString"]));

            return new AutofacServiceProvider(container.Build());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            
            app.UseCors("CorsPolicy");

            ConfigureAuth(app);

            app.UseMvcWithDefaultRoute();

            app.UseSwagger()
               .UseSwaggerUI(c =>
               {
                   c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
               });

            WaitForSqlAvailabilityAsync(loggerFactory, app).Wait();

            var integrationEventLogContext = new IntegrationEventLogContext(
                new DbContextOptionsBuilder<IntegrationEventLogContext>()
                .UseSqlServer(Configuration["ConnectionString"], b => b.MigrationsAssembly("Ordering.API"))
                .Options);
            integrationEventLogContext.Database.Migrate();
        }

        protected virtual void ConfigureAuth(IApplicationBuilder app)
        {
            var identityUrl = Configuration.GetValue<string>("IdentityUrl");
            app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
            {
                Authority = identityUrl.ToString(),
                ApiName = "orders",
                RequireHttpsMetadata = false
            });
        }


        private async Task WaitForSqlAvailabilityAsync(ILoggerFactory loggerFactory, IApplicationBuilder app, int retries = 0)
        {
            var logger = loggerFactory.CreateLogger(nameof(Startup));
            var policy = CreatePolicy(retries, logger, nameof(WaitForSqlAvailabilityAsync));
            await policy.ExecuteAsync(async () =>
            {
                await OrderingContextSeed.SeedAsync(app);
            });

        }

        private Policy CreatePolicy(int retries, ILogger logger, string prefix)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogTrace($"[{prefix}] Exception {exception.GetType().Name} with message ${exception.Message} detected on attempt {retry} of {retries}");
                    }
                );
        }
    }
}
