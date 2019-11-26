using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ordering.BackgroundTasks.Extensions;
using Ordering.BackgroundTasks.Tasks;
using Serilog;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Ordering.BackgroundTasks
{
    public class Program
    {
        public static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string AppName = Namespace;

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Run();
        }

        public static IWebHost CreateHostBuilder(string[] args) =>
           WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((host, builder) =>
                {
                    builder.SetBasePath(Directory.GetCurrentDirectory());
                    builder.AddJsonFile("appsettings.json", optional: true);
                    builder.AddJsonFile($"appsettings.{host.HostingEnvironment.EnvironmentName}.json", optional: true);
                    builder.AddEnvironmentVariables();
                    builder.AddCommandLine(args);
                })
                .ConfigureLogging((host, builder) => builder.UseSerilog(host.Configuration).AddSerilog())
                .UseStartup<Startup>()
                .Build();
    }
}
