using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.eShopOnContainers.Services.Basket.API.Model;
using StackExchange.Redis;
using Microsoft.Extensions.Options;
using System.Net;
using Microsoft.eShopOnContainers.Services.Basket.API.Auth.Server;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.Services.Basket.API.IntegrationEvents.Events;
using Microsoft.eShopOnContainers.Services.Basket.API.IntegrationEvents.EventHandling;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBusRabbitMQ;
using System;
using Microsoft.Extensions.HealthChecks;
using System.Threading.Tasks;
using Basket.API.Infrastructure.Filters;

namespace Microsoft.eShopOnContainers.Services.Basket.API
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddHealthChecks(checks =>
            {
                checks.AddValueTaskCheck("HTTP Endpoint", () => new ValueTask<IHealthCheckResult>(HealthCheckResult.Healthy("Ok")));
            });

            // Add framework services.
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            }).AddControllersAsServices();

            services.Configure<BasketSettings>(Configuration);

            //By connecting here we are making sure that our service
            //cannot start until redis is ready. This might slow down startup,
            //but given that there is a delay on resolving the ip address
            //and then creating the connection it seems reasonable to move
            //that cost to startup instead of having the first request pay the
            //penalty.
            services.AddSingleton<ConnectionMultiplexer>((sp) => {
                var config = sp.GetRequiredService<IOptionsSnapshot<BasketSettings>>().Value;
                var ips = Dns.GetHostAddressesAsync(config.ConnectionString).Result;
                return ConnectionMultiplexer.Connect(ips.First().ToString());
            });

            services.AddSwaggerGen();
            //var sch = new IdentitySecurityScheme();
            services.ConfigureSwaggerGen(options =>
            {
                //options.AddSecurityDefinition("IdentityServer", sch);
                options.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
                options.DescribeAllEnumsAsStrings();
                options.SingleApiVersion(new Swashbuckle.Swagger.Model.Info()
                {
                    Title = "Basket HTTP API",
                    Version = "v1",
                    Description = "The Basket Service HTTP API",
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

            services.AddTransient<IBasketRepository, RedisBasketRepository>();
            services.AddTransient<IIntegrationEventHandler<ProductPriceChangedIntegrationEvent>, ProductPriceChangedIntegrationEventHandler>();

            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetRequiredService<IOptionsSnapshot<BasketSettings>>().Value;
            services.AddSingleton<IEventBus>(provider => new EventBusRabbitMQ(configuration.EventBusConnection));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseStaticFiles();

            // Use frameworks
            app.UseCors("CorsPolicy");

            ConfigureAuth(app);

            app.UseMvcWithDefaultRoute();

            app.UseSwagger()
                .UseSwaggerUi();

            var catalogPriceHandler = app.ApplicationServices.GetService<IIntegrationEventHandler<ProductPriceChangedIntegrationEvent>>();
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.Subscribe<ProductPriceChangedIntegrationEvent>(catalogPriceHandler);

        }

        protected virtual void ConfigureAuth(IApplicationBuilder app)
        {
            var identityUrl = Configuration.GetValue<string>("IdentityUrl");
            app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
            {
                Authority = identityUrl.ToString(),
                ScopeName = "basket",
                RequireHttpsMetadata = false
            });
        }       
    }
}
