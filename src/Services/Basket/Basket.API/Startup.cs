using Autofac;
using Autofac.Extensions.DependencyInjection;
using Basket.API.Infrastructure.Filters;
using Basket.API.Infrastructure.Middlewares;
using Basket.API.IntegrationEvents.EventHandling;
using HealthChecks.UI.Client;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.ServiceFabric;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.Services.Basket.API.IntegrationEvents.EventHandling;
using Microsoft.eShopOnContainers.Services.Basket.API.Model;
using Microsoft.eShopOnContainers.Services.Basket.API.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace Microsoft.eShopOnContainers.Services.Basket.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            RegisterAppInsights(services);

            // Add framework services.
            services.AddMvc(options =>
                {
                    options.Filters.Add(typeof(HttpGlobalExceptionFilter));
                    options.Filters.Add(typeof(ValidateModelStateFilter));

                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddControllersAsServices();

            ConfigureAuthService(services);

            services.AddCustomHealthCheck(Configuration);

            services.Configure<BasketSettings>(Configuration);

            //By connecting here we are making sure that our service
            //cannot start until redis is ready. This might slow down startup,
            //but given that there is a delay on resolving the ip address
            //and then creating the connection it seems reasonable to move
            //that cost to startup instead of having the first request pay the
            //penalty.
            services.AddSingleton<ConnectionMultiplexer>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<BasketSettings>>().Value;
                var configuration = ConfigurationOptions.Parse(settings.ConnectionString, true);

                configuration.ResolveDns = true;

                return ConnectionMultiplexer.Connect(configuration);
            });

            services.AddCap(options =>
            {
                options.UseInMemoryStorage();
                if (Configuration.GetValue<bool>("AzureServiceBusEnabled"))
                {
                    options.UseAzureServiceBus(Configuration["EventBusConnection"]);
                }
                else
                {
                    options.UseRabbitMQ(conf =>
                    {
                        conf.HostName = Configuration["EventBusConnection"];
                        if (!string.IsNullOrEmpty(Configuration["EventBusUserName"]))
                        {
                            conf.UserName = Configuration["EventBusUserName"];
                        }
                        if (!string.IsNullOrEmpty(Configuration["EventBusPassword"]))
                        {
                            conf.Password = Configuration["EventBusPassword"];
                        }
                    });
                }

                if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
                {
                    options.FailedRetryCount = int.Parse(Configuration["EventBusRetryCount"]);
                }

                if (!string.IsNullOrEmpty(Configuration["SubscriptionClientName"]))
                {
                    options.DefaultGroup = Configuration["SubscriptionClientName"];
                }
            });

            RegisterEventBus(services);

            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new Info
                {
                    Title = "Basket HTTP API",
                    Version = "v1",
                    Description = "The Basket Service HTTP API",
                    TermsOfService = "Terms Of Service"
                });

                options.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Type = "oauth2",
                    Flow = "implicit",
                    AuthorizationUrl = $"{Configuration.GetValue<string>("IdentityUrlExternal")}/connect/authorize",
                    TokenUrl = $"{Configuration.GetValue<string>("IdentityUrlExternal")}/connect/token",
                    Scopes = new Dictionary<string, string>()
                    {
                        { "basket", "Basket API" }
                    }
                });

                options.OperationFilter<AuthorizeCheckOperationFilter>();
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .SetIsOriginAllowed((host) => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IBasketRepository, RedisBasketRepository>();
            services.AddTransient<IIdentityService, IdentityService>();

            services.AddOptions();

            var container = new ContainerBuilder();
            container.Populate(services);

            return new AutofacServiceProvider(container.Build());
        }
    

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //loggerFactory.AddAzureWebAppDiagnostics();
            //loggerFactory.AddApplicationInsights(app.ApplicationServices, LogLevel.Trace);

            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }

            app.UseHealthChecks("/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseHealthChecks("/liveness", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });

            app.UseStaticFiles();          
            app.UseCors("CorsPolicy");

            ConfigureAuth(app);

            app.UseMvcWithDefaultRoute();

            app.UseSwagger()
               .UseSwaggerUI(c =>
               {
                   c.SwaggerEndpoint($"{ (!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty) }/swagger/v1/swagger.json", "Basket.API V1");
                   c.OAuthClientId ("basketswaggerui");
                   c.OAuthAppName("Basket Swagger UI");
               });
        }

        private void RegisterAppInsights(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(Configuration);
            var orchestratorType = Configuration.GetValue<string>("OrchestratorType");
            
            if (orchestratorType?.ToUpper() == "K8S")
            {
                // Enable K8s telemetry initializer
                services.AddApplicationInsightsKubernetesEnricher();
            }
            if (orchestratorType?.ToUpper() == "SF")
            {
                // Enable SF telemetry initializer
                services.AddSingleton<ITelemetryInitializer>((serviceProvider) =>
                    new FabricTelemetryInitializer());
            }
        }

        private void ConfigureAuthService(IServiceCollection services)
        {
            // prevent from mapping "sub" claim to nameidentifier.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var identityUrl = Configuration.GetValue<string>("IdentityUrl"); 
                
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.Authority = identityUrl;
                options.RequireHttpsMetadata = false;
                options.Audience = "basket";
            });
        }

        protected virtual void ConfigureAuth(IApplicationBuilder app)
        {
            if (Configuration.GetValue<bool>("UseLoadTest"))
            {
                app.UseMiddleware<ByPassAuthMiddleware>();
            }

            app.UseAuthentication();
        }

        private void RegisterEventBus(IServiceCollection services)
        {
            services.AddTransient<ProductPriceChangedIntegrationEventHandler>();
            services.AddTransient<OrderStartedIntegrationEventHandler>();
        }    
    }

    public static class CustomExtensionMethods
    {
        public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
        {
            var hcBuilder = services.AddHealthChecks();

            hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

            hcBuilder                
                .AddRedis(
                    configuration["ConnectionString"],
                    name: "redis-check",
                    tags: new string[] { "redis" });

            if (configuration.GetValue<bool>("AzureServiceBusEnabled"))
            {
                hcBuilder
                    .AddAzureServiceBusTopic(
                        configuration["EventBusConnection"],
                        topicName: "eshop_event_bus",
                        name: "basket-servicebus-check",
                        tags: new string[] { "servicebus" });
            }
            else
            {
                hcBuilder
                    .AddRabbitMQ(
                        $"amqp://{configuration["EventBusConnection"]}",
                        name: "basket-rabbitmqbus-check",
                        tags: new string[] { "rabbitmqbus" });
            }

            return services;
        }
    }
}
