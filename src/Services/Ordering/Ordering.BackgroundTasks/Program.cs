using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ordering.BackgroundTasks.Extensions;
using Ordering.BackgroundTasks.Tasks;
using Serilog;
using System.IO;

namespace Ordering.BackgroundTasks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((host, builder) =>
                {
                    builder.SetBasePath(Directory.GetCurrentDirectory());
                    builder.AddJsonFile("appsettings.json", optional: true);
                    builder.AddJsonFile($"appsettings.{host.HostingEnvironment.EnvironmentName}.json", optional: true);
                    builder.AddEnvironmentVariables();
                    builder.AddCommandLine(args);
                })
                .ConfigureLogging((host, builder) => builder.UseSerilog(host.Configuration).AddSerilog())
                .ConfigureServices((host, services) =>
                {
                    services.AddCustomHealthCheck(host.Configuration);
                    services.Configure<BackgroundTaskSettings>(host.Configuration);
                    services.AddOptions();
                    services.AddHostedService<GracePeriodManagerService>();
                    services.AddEventBus(host.Configuration);
                    services.AddAutofac(container => container.Populate(services));
                })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory());
    }
}
