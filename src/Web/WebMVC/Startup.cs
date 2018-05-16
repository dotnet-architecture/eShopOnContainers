using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.ServiceFabric;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.eShopOnContainers.BuildingBlocks.Resilience.Http;
using Microsoft.eShopOnContainers.WebMVC.Infrastructure;
using Microsoft.eShopOnContainers.WebMVC.Services;
using Microsoft.eShopOnContainers.WebMVC.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Registry;
using StackExchange.Redis;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using WebMVC.Infrastructure;
using WebMVC.Infrastructure.Middlewares;
using WebMVC.Services;

namespace Microsoft.eShopOnContainers.WebMVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAppInsight(Configuration)
                .AddHealthChecks(Configuration)
                .AddCustomMvc(Configuration)
                .AddCustomApplicationServices(Configuration)
                .AddCustomAuthentication(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            loggerFactory.AddAzureWebAppDiagnostics();
            loggerFactory.AddApplicationInsights(app.ApplicationServices, LogLevel.Trace);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                loggerFactory.CreateLogger("init").LogDebug($"Using PATH BASE '{pathBase}'");
                app.UsePathBase(pathBase);
            }


#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            app.Map("/liveness", lapp => lapp.Run(async ctx => ctx.Response.StatusCode = 200));
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

            app.UseSession();
            app.UseStaticFiles();

            if (Configuration.GetValue<bool>("UseLoadTest"))
            {
                app.UseMiddleware<ByPassAuthMiddleware>();
            }

            app.UseAuthentication();

            var log = loggerFactory.CreateLogger("identity");

            WebContextSeed.Seed(app, env, loggerFactory);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Catalog}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "defaultError",
                    template: "{controller=Error}/{action=Error}");
            });
        }
    }

    static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddAppInsight(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationInsightsTelemetry(configuration);
            var orchestratorType = configuration.GetValue<string>("OrchestratorType");

            if (orchestratorType?.ToUpper() == "K8S")
            {
                // Enable K8s telemetry initializer
                services.EnableKubernetes();
            }

            if (orchestratorType?.ToUpper() == "SF")
            {
                // Enable SF telemetry initializer
                services.AddSingleton<ITelemetryInitializer>((serviceProvider) =>
                    new FabricTelemetryInitializer());
            }

            return services;
        }

        public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks(checks =>
            {
                var minutes = 1;
                if (int.TryParse(configuration["HealthCheck:Timeout"], out var minutesParsed))
                {
                    minutes = minutesParsed;
                }

                checks.AddUrlCheck(configuration["CatalogUrlHC"], TimeSpan.FromMinutes(minutes));
                checks.AddUrlCheck(configuration["OrderingUrlHC"], TimeSpan.FromMinutes(minutes));
                checks.AddUrlCheck(configuration["BasketUrlHC"], TimeSpan.Zero); //No cache for this HealthCheck, better just for demos 
                checks.AddUrlCheck(configuration["IdentityUrlHC"], TimeSpan.FromMinutes(minutes));
                checks.AddUrlCheck(configuration["MarketingUrlHC"], TimeSpan.FromMinutes(minutes));
            });

            return services;
        }

        public static IServiceCollection AddCustomMvc(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMvc();

            services.AddSession();

            if (configuration.GetValue<string>("IsClusterEnv") == bool.TrueString)
            {
                services.AddDataProtection(opts =>
                {
                    opts.ApplicationDiscriminator = "eshop.webmvc";
                })
                .PersistKeysToRedis(ConnectionMultiplexer.Connect(configuration["DPConnectionString"]), "DataProtection-Keys");
            }

            services.Configure<AppSettings>(configuration);

            return services;
        }

        public static IServiceCollection AddCustomApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<HttpClientDefaultPolicies>();
            var defaultPolicies = services.BuildServiceProvider().GetService<HttpClientDefaultPolicies>();

            var registry = services.AddPolicyRegistry();
            registry.Add("WaitAndRetry", defaultPolicies.GetWaitAndRetryPolicy());
            registry.Add("CircuitBreaker", defaultPolicies.GetCircuitBreakerPolicy());

            services.AddHttpClient<IBasketService, BasketService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AddPolicyHandlerFromRegistry("WaitAndRetry")
                .AddPolicyHandlerFromRegistry("CircuitBreaker");

            services.AddTransient<ICatalogService, CatalogService>();
            services.AddTransient<IOrderingService, OrderingService>();

            services.AddTransient<ICampaignService, CampaignService>();
            services.AddTransient<ILocationService, LocationService>();
            services.AddTransient<IIdentityParser<ApplicationUser>, IdentityParser>();

            if (configuration.GetValue<string>("UseResilientHttp") == bool.TrueString)
            {
                services.AddSingleton<IResilientHttpClientFactory, ResilientHttpClientFactory>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<ResilientHttpClient>>();
                    var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();

                    var retryCount = 6;
                    if (!string.IsNullOrEmpty(configuration["HttpClientRetryCount"]))
                    {
                        retryCount = int.Parse(configuration["HttpClientRetryCount"]);
                    }

                    var exceptionsAllowedBeforeBreaking = 5;
                    if (!string.IsNullOrEmpty(configuration["HttpClientExceptionsAllowedBeforeBreaking"]))
                    {
                        exceptionsAllowedBeforeBreaking = int.Parse(configuration["HttpClientExceptionsAllowedBeforeBreaking"]);
                    }

                    return new ResilientHttpClientFactory(logger, httpContextAccessor, exceptionsAllowedBeforeBreaking, retryCount);
                });
                services.AddSingleton<IHttpClient, ResilientHttpClient>(sp => sp.GetService<IResilientHttpClientFactory>().CreateResilientHttpClient());
            }
            else
            {
                services.AddSingleton<IHttpClient, StandardHttpClient>();
            }

            return services;
        }
        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var useLoadTest = configuration.GetValue<bool>("UseLoadTest");
            var identityUrl = configuration.GetValue<string>("IdentityUrl");
            var callBackUrl = configuration.GetValue<string>("CallBackUrl");

            // Add Authentication services          

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect(options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = identityUrl.ToString();
                options.SignedOutRedirectUri = callBackUrl.ToString();
                options.ClientId = useLoadTest ? "mvctest" : "mvc";
                options.ClientSecret = "secret";
                options.ResponseType = useLoadTest ? "code id_token token" : "code id_token";
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.RequireHttpsMetadata = false;
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("orders");
                options.Scope.Add("basket");
                options.Scope.Add("marketing");
                options.Scope.Add("locations");
                options.Scope.Add("webshoppingagg");
                options.Scope.Add("orders.signalrhub");
            });

            return services;
        }

    }

}
