using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ordering.BackgroundTasks.Configuration;
using Ordering.BackgroundTasks.Tasks;
using System;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Ordering.BackgroundTasks
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
            //add health check for this service
            services.AddCustomHealthCheck(Configuration);

            //configure settings

            services.Configure<BackgroundTaskSettings>(Configuration);
            services.AddOptions();

            //configure background task

            services.AddSingleton<IHostedService, GracePeriodManagerService>();

            //configure event bus related services
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

            //create autofac based service provider
            var container = new ContainerBuilder();
            container.Populate(services);
            return new AutofacServiceProvider(container.Build());
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseHealthChecks("/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseHealthChecks("/liveness", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });
        }
    }

    public static class CustomExtensionMethods
    {
        public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
        {
            var hcBuilder = services.AddHealthChecks();

            hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

            hcBuilder
                .AddSqlServer(
                    configuration["ConnectionString"],
                    name: "OrderingTaskDB-check",
                    tags: new string[] { "orderingtaskdb" });

            if (configuration.GetValue<bool>("AzureServiceBusEnabled"))
            {
                hcBuilder
                    .AddAzureServiceBusTopic(
                        configuration["EventBusConnection"],
                        topicName: "eshop_event_bus",
                        name: "orderingtask-servicebus-check",
                        tags: new string[] { "servicebus" });
            }
            else
            {
                hcBuilder
                    .AddRabbitMQ(
                        $"amqp://{configuration["EventBusConnection"]}",
                        name: "orderingtask-rabbitmqbus-check",
                        tags: new string[] { "rabbitmqbus" });
            }

            return services;
        }
    }
}
