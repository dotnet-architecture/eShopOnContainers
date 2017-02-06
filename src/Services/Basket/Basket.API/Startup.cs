using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.eShopOnContainers.Services.Basket.API.Model;
using StackExchange.Redis;
using Microsoft.Extensions.Options;
using System.Net;
using Swashbuckle.Swagger.Model;
using Microsoft.eShopOnContainers.Services.Basket.API.Auth.Server;

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
            // Add framework services.
            services.AddMvc();
            services.Configure<BasketSettings>(Configuration);

            //By connection here we are making sure that our service
            //cannot start until redis is ready. This might slow down startup,
            //but given that it is there is a delay on resolving the ip address
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseStaticFiles();

            // Use frameworks
            app.UseCors("CorsPolicy");

            var identityUrl = Configuration.GetValue<string>("IdentityUrl");

            app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
            {
                Authority = identityUrl.ToString(),
                ScopeName = "basket",
                RequireHttpsMetadata = false
            });

            app.UseMvcWithDefaultRoute();

            app.UseSwagger()
                .UseSwaggerUi();

        }
    }
}
