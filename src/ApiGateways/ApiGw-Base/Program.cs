using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace OcelotApiGw
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile(Path.Combine("configuration", "configuration.json"));
                    config.AddCommandLine(args);
                })
                .UseStartup<Startup>()
                .Build()
                .Run();
        }
    }
}
