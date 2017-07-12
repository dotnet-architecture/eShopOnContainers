namespace Microsoft.eShopOnContainers.Services.Marketing.API
{
    using System.IO;
    using AspNetCore.Hosting;

    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseHealthChecks("/hc")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseWebRoot("Pics")
                .Build();

            host.Run();
        }
    }
}
