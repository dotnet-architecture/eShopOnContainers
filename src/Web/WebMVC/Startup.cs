using Devspaces.Support;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.WebMVC.Services;
using Microsoft.eShopOnContainers.WebMVC.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
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

        // This method gets called by the runtime. Use this method to add services to the IoC container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews()
                .Services
                .AddAppInsight(Configuration)
                .AddHealthChecks(Configuration)
                .AddCustomMvc(Configuration)
                .AddDevspaces()
                .AddHttpClientServices(Configuration);

            IdentityModelEventSource.ShowPII  = true;       // Caution! Do NOT use in production: https://aka.ms/IdentityModel/PII
            
            services.AddControllers();

            services.AddCustomAuthentication(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
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
                app.UsePathBase(pathBase);
            }

            app.UseStaticFiles();
            app.UseSession();

            if (Configuration.GetValue<bool>("UseLoadTest"))
            {
                app.UseMiddleware<ByPassAuthMiddleware>();
            }

            WebContextSeed.Seed(app, env);

            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Catalog}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("defaultError", "{controller=Error}/{action=Error}");
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self")
                });
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            });
        }
    }

    static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddAppInsight(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationInsightsTelemetry(configuration);
            services.AddApplicationInsightsKubernetesEnricher();

            return services;
        }

        public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddUrlGroup(new Uri(configuration["IdentityUrlHC"]), name: "identityapi-check", tags: new string[] { "identityapi" });

            return services;
        }

        public static IServiceCollection AddCustomMvc(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<AppSettings>(configuration);
            services.AddSession();
            services.AddDistributedMemoryCache();

            if (configuration.GetValue<string>("IsClusterEnv") == bool.TrueString)
            {
                services.AddDataProtection(opts =>
                {
                    opts.ApplicationDiscriminator = "eshop.webmvc";
                })
                .PersistKeysToRedis(ConnectionMultiplexer.Connect(configuration["DPConnectionString"]), "DataProtection-Keys");
            }

            return services;
        }

        // Adds all Http client services
        public static IServiceCollection AddHttpClientServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //register delegating handlers
            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();
            services.AddTransient<HttpClientRequestIdDelegatingHandler>();

            //set 5 min as the lifetime for each HttpMessageHandler int the pool
            services.AddHttpClient("extendedhandlerlifetime").SetHandlerLifetime(TimeSpan.FromMinutes(5)).AddDevspacesSupport();

            //add http client services
            services.AddHttpClient<IBasketService, BasketService>()
                   .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Sample. Default lifetime is 2 minutes
                   .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                   .AddDevspacesSupport();

            services.AddHttpClient<ICatalogService, CatalogService>()
                   .AddDevspacesSupport();

            services.AddHttpClient<IOrderingService, OrderingService>()
                 .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                 .AddHttpMessageHandler<HttpClientRequestIdDelegatingHandler>()
                 .AddDevspacesSupport();

            services.AddHttpClient<ICampaignService, CampaignService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AddDevspacesSupport();

            services.AddHttpClient<ILocationService, LocationService>()
               .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
               .AddDevspacesSupport();

            //add custom application services
            services.AddTransient<IIdentityParser<ApplicationUser>, IdentityParser>();

            return services;
        }


        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var useLoadTest = configuration.GetValue<bool>("UseLoadTest");
            var identityUrl = configuration.GetValue<string>("IdentityUrl");
            var callBackUrl = configuration.GetValue<string>("CallBackUrl");
            var sessionCookieLifetime = configuration.GetValue("SessionCookieLifetimeMinutes", 60);

            // Add Authentication services          

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie(setup => setup.ExpireTimeSpan = TimeSpan.FromMinutes(sessionCookieLifetime))
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
