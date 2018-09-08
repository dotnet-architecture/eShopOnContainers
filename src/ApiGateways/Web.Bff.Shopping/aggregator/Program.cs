using static Microsoft.AspNetCore.Hosting.WebHostBuilderExtensions;
using static Microsoft.AspNetCore.Hosting.WebHostExtensions;
using IWebHost = Microsoft.AspNetCore.Hosting.IWebHost;
using WebHost = Microsoft.AspNetCore.WebHost;

namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator
{
	public class Program
	{
		public static void Main(string[] args) 
			=> BuildWebHost(args).Run();

		public static IWebHost BuildWebHost(string[] args) =>
		    WebHost
			   .CreateDefaultBuilder(args)
			   .ConfigureAppConfiguration(cb =>
			   {
				   var sources = cb.Sources;
				   sources.Insert(3, new Extensions.Configuration.Json.JsonConfigurationSource()
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
