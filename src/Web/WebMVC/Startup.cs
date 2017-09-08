using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.eShopOnContainers.BuildingBlocks;
using Microsoft.eShopOnContainers.BuildingBlocks.Resilience.Http;
using Microsoft.eShopOnContainers.WebMVC.Infrastructure;
using Microsoft.eShopOnContainers.WebMVC.Services;
using Microsoft.eShopOnContainers.WebMVC.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.IdentityModel.Tokens.Jwt;
using WebMVC.Infrastructure;

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
            services.AddMvc();            
            services.AddSession();

            if (Configuration.GetValue<string>("IsClusterEnv") == bool.TrueString)
            {
                services.AddDataProtection(opts =>
                {
                    opts.ApplicationDiscriminator = "eshop.webmvc";
                })
                .PersistKeysToRedis(Configuration["DPConnectionString"]);
            }

            services.Configure<AppSettings>(Configuration);

            services.AddHealthChecks(checks =>
            {
                var minutes = 1;
                if (int.TryParse(Configuration["HealthCheck:Timeout"], out var minutesParsed))
                {
                    minutes = minutesParsed;
                }
                checks.AddUrlCheck(Configuration["CatalogUrl"] + "/hc", TimeSpan.FromMinutes(minutes));
                checks.AddUrlCheck(Configuration["OrderingUrl"] + "/hc", TimeSpan.FromMinutes(minutes));
                checks.AddUrlCheck(Configuration["BasketUrl"] + "/hc", TimeSpan.FromMinutes(minutes));
                checks.AddUrlCheck(Configuration["IdentityUrl"] + "/hc", TimeSpan.FromMinutes(minutes));
                checks.AddUrlCheck(Configuration["MarketingUrl"] + "/hc", TimeSpan.FromMinutes(minutes));
            });

            // Add application services.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<ICatalogService, CatalogService>();
            services.AddTransient<IOrderingService, OrderingService>();
            services.AddTransient<IBasketService, BasketService>();
            services.AddTransient<ICampaignService, CampaignService>();
            services.AddTransient<IIdentityParser<ApplicationUser>, IdentityParser>();

            if (Configuration.GetValue<string>("UseResilientHttp") == bool.TrueString)
            {
                services.AddSingleton<IResilientHttpClientFactory, ResilientHttpClientFactory>();
                services.AddSingleton<IHttpClient, ResilientHttpClient>(sp => sp.GetService<IResilientHttpClientFactory>().CreateResilientHttpClient());
            }
            else
            {
                services.AddSingleton<IHttpClient, StandardHttpClient>();
            }
            var useLoadTest = Configuration.GetValue<bool>("UseLoadTest");
            var identityUrl = Configuration.GetValue<string>("IdentityUrl");
            var callBackUrl = Configuration.GetValue<string>("CallBackUrl");
            
            // Add Authentication services          
            
            services.AddAuthentication(options => {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect(options => {
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
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
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

            app.UseSession();
            app.UseStaticFiles();

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
}
