using Basket.API.Infrastructure.Filters;
using Basket.API.IntegrationEvents.EventHandling;
using Basket.API.IntegrationEvents.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBusRabbitMQ;
using Microsoft.eShopOnContainers.Services.Basket.API.Auth.Server;
using Microsoft.eShopOnContainers.Services.Basket.API.IntegrationEvents.EventHandling;
using Microsoft.eShopOnContainers.Services.Basket.API.IntegrationEvents.Events;
using Microsoft.eShopOnContainers.Services.Basket.API.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

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
            services.AddSingleton<ConnectionMultiplexer>(sp => {
                var settings = sp.GetRequiredService<IOptions<BasketSettings>>().Value;
                var ips = Dns.GetHostAddressesAsync(settings.ConnectionString).Result;

                return ConnectionMultiplexer.Connect(ips.First().ToString());
            });

            services.AddSingleton<IEventBus>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<BasketSettings>>().Value;

                return new EventBusRabbitMQ(settings.EventBusConnection);
            });

            services.AddSwaggerGen();
            
            services.ConfigureSwaggerGen(options =>
            {
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
            services.AddTransient<IIntegrationEventHandler<OrderStartedIntegrationEvent>, OrderStartedIntegrationEventHandler>();

          
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

            ConfigureEventBus(app);

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

        protected virtual void ConfigureEventBus(IApplicationBuilder app)
        {
            var catalogPriceHandler = app.ApplicationServices
                .GetService<IIntegrationEventHandler<ProductPriceChangedIntegrationEvent>>();

            var orderStartedHandler = app.ApplicationServices
                .GetService<IIntegrationEventHandler<OrderStartedIntegrationEvent>>();

            var eventBus = app.ApplicationServices
                .GetRequiredService<IEventBus>();

            eventBus.Subscribe<ProductPriceChangedIntegrationEvent>(catalogPriceHandler);
            eventBus.Subscribe<OrderStartedIntegrationEvent>(orderStartedHandler);
        }
    }
}
