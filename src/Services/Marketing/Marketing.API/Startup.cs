namespace Microsoft.eShopOnContainers.Services.Marketing.API
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.eShopOnContainers.Services.Marketing.API.Infrastructure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System.Reflection;
    using System;
    using Microsoft.eShopOnContainers.Services.Marketing.API.Infrastructure.Filters;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBusRabbitMQ;
    using RabbitMQ.Client;
    using BuildingBlocks.EventBus.Abstractions;
    using BuildingBlocks.EventBus;
    using IntegrationEvents.Events;
    using IntegrationEvents.Handlers;
    using Infrastructure.Repositories;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Polly;
    using System.Threading.Tasks;
    using System.Data.SqlClient;

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets(typeof(Startup).GetTypeInfo().Assembly);
            }

            builder.AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            }).AddControllersAsServices();  //Injecting Controllers themselves thru DIFor further info see: http://docs.autofac.org/en/latest/integration/aspnetcore.html#controllers-as-services

            services.Configure<MarketingSettings>(Configuration);

            services.AddDbContext<MarketingContext>(options =>
            {
                options.UseSqlServer(Configuration["ConnectionString"],
                                     sqlServerOptionsAction: sqlOptions =>
                                     {
                                         sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                                         //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                                         sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                     });

                // Changing default behavior when client evaluation occurs to throw. 
                // Default in EF Core would be to log a warning when client evaluation is performed.
                options.ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning));
                //Check Client vs. Server evaluation: https://docs.microsoft.com/en-us/ef/core/querying/client-eval
            });

            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = Configuration["EventBusConnection"]
                };

                return new DefaultRabbitMQPersistentConnection(factory, logger);
            });

            RegisterServiceBus(services);

            // Add framework services.
            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Title = "Marketing HTTP API",
                    Version = "v1",
                    Description = "The Marketing Service HTTP API",
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

            services.AddTransient<IMarketingDataRepository, MarketingDataRepository>();

            //configure autofac
            var container = new ContainerBuilder();
            container.Populate(services);

            return new AutofacServiceProvider(container.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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

            var context = (MarketingContext)app
                        .ApplicationServices.GetService(typeof(MarketingContext));

            WaitForSqlAvailabilityAsync(context, loggerFactory, app).Wait();

            ConfigureEventBus(app);
        }

        protected virtual void ConfigureAuth(IApplicationBuilder app)
        {
            var identityUrl = Configuration.GetValue<string>("IdentityUrl");
            app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
            {
                Authority = identityUrl.ToString(),
                ApiName = "marketing",
                RequireHttpsMetadata = false
            });
        }

        private void RegisterServiceBus(IServiceCollection services)
        {
            services.AddSingleton<IEventBus, EventBusRabbitMQ>();
            services.AddSingleton<IEventBusSubscriptionsManager, 
                InMemoryEventBusSubscriptionsManager>();

            services.AddTransient<IIntegrationEventHandler<UserLocationUpdatedIntegrationEvent>,
                UserLocationUpdatedIntegrationEventHandler>();
        }

        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.Subscribe<UserLocationUpdatedIntegrationEvent, 
                IIntegrationEventHandler<UserLocationUpdatedIntegrationEvent>>();
        }

        private async Task WaitForSqlAvailabilityAsync(MarketingContext ctx, ILoggerFactory loggerFactory, IApplicationBuilder app, int retries = 0)
        {
            var logger = loggerFactory.CreateLogger(nameof(Startup));
            var policy = CreatePolicy(retries, logger, nameof(WaitForSqlAvailabilityAsync));
            await policy.ExecuteAsync(async () =>
            {
                await MarketingContextSeed.SeedAsync(app, loggerFactory);
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
