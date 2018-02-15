using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost
                .CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(cb =>
                {
                    var sources = cb.Sources;
                    sources.Insert(3, new Microsoft.Extensions.Configuration.Json.JsonConfigurationSource()
                    {
                        Optional = true,
                        Path = "appsettings.localhost.json",
                        ReloadOnChange = false
                    });
                })
                .UseStartup<Startup>()
                .Build();
    }
}
