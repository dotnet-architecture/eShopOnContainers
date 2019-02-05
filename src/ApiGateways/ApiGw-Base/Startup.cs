using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using OcelotApiGw.Enums;
using OcelotApiGw.Models;
using OcelotApiGw.Orchestrators;
using OcelotApiGw.Providers;
using OcelotApiGw.Services;
using System.Linq;
using System;
using HealthChecks.UI.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace OcelotApiGw
{
    public class Startup
    {
        private IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var identityUrl = _configuration.GetValue<string>("IdentityUrl");
            var authenticationProviderKey = "IdentityApiKey";

            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddUrlGroup(new Uri(_cfg["CatalogUrlHC"]), name: "catalogapi-check", tags: new string[] { "catalogapi" })
                .AddUrlGroup(new Uri(_cfg["OrderingUrlHC"]), name: "orderingapi-check", tags: new string[] { "orderingapi" })
                .AddUrlGroup(new Uri(_cfg["BasketUrlHC"]), name: "basketapi-check", tags: new string[] { "basketapi" })
                .AddUrlGroup(new Uri(_cfg["IdentityUrlHC"]), name: "identityapi-check", tags: new string[] { "identityapi" })
                .AddUrlGroup(new Uri(_cfg["MarketingUrlHC"]), name: "marketingapi-check", tags: new string[] { "marketingapi" })
                .AddUrlGroup(new Uri(_cfg["PaymentUrlHC"]), name: "paymentapi-check", tags: new string[] { "paymentapi" })
                .AddUrlGroup(new Uri(_cfg["LocationUrlHC"]), name: "locationapi-check", tags: new string[] { "locationapi" });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            })
            .AddTransient<IOrchestratorStrategy, Kubernetes>()
            .AddTransient<IOrchestratorStrategy, ServiceFabric>()
            .AddTransient<IFileProvider, InMemoryFileProvider>()
            .AddAuthentication()
            .AddJwtBearer(authenticationProviderKey, options =>
            {
                options.Authority = identityUrl;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidAudiences = new[] { "orders", "basket", "locations", "marketing", "mobileshoppingagg", "webshoppingagg" }
                };
            });

            services.Configure<ServiceFabricSettings>(_configuration);
            services.AddHttpClient<ISettingService, SettingService>();

            var orchestratorType = _configuration.GetValue<OrchestratorType>("OrchestratorType");

            var config = new ConfigurationBuilder()
                .AddConfiguration(_configuration);

            _configuration = services.BuildServiceProvider()
                .GetServices<IOrchestratorStrategy>()
                .Single(strategy => strategy.OrchestratorType == orchestratorType)
                .ConfigureOrchestrator(config)
                .Build();

            services.AddOcelot(_configuration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var pathBase = _configuration.GetValue<string>("PATH_BASE");

            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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

            loggerFactory.AddConsole(_cfg.GetSection("Logging"));

            app.UseCors("CorsPolicy");

            app.UseCors("CorsPolicy")
                .UseOcelot()
                .Wait();
        }
    }
}
