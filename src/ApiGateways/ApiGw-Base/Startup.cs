using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using OcelotApiGw.Enums;
using OcelotApiGw.Orchestrators;
using OcelotApiGw.Services;
using System.Linq;

namespace OcelotApiGw
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var identityUrl = _configuration.GetValue<string>("IdentityUrl");
            var authenticationProviderKey = "IdentityApiKey";

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

            services.AddHttpClient<ISettingService, SettingService>();

            var orchestratorType = _configuration.GetValue<OrchestratorType>("OrchestratorType");

            var config = new ConfigurationBuilder()
                .AddConfiguration(_configuration);

            var cfg = services.BuildServiceProvider()
                .GetServices<IOrchestratorStrategy>()
                .Single(strategy => strategy.OrchestratorType == orchestratorType)
                .ConfigureOrchestrator(config)
                .Build();

            services.AddOcelot(cfg);
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

            loggerFactory.AddConsole(_configuration.GetSection("Logging"));

            app.UseCors("CorsPolicy")
                .UseOcelot()
                .Wait();
        }
    }
}
