using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;

namespace Microsoft.eShopOnContainers.Services.Catalog.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://0.0.0.0:5101")
                .UseIISIntegration()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseWebRoot("Pics")
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
