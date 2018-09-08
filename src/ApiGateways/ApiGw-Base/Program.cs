using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static Microsoft.AspNetCore.Hosting.WebHostBuilderExtensions;
using static Microsoft.AspNetCore.Hosting.WebHostExtensions;
using IWebHost = Microsoft.AspNetCore.Hosting.IWebHost;
using Path = System.IO.Path;
using WebHost = Microsoft.AspNetCore.WebHost;

namespace OcelotApiGw
{
	public class Program
	{
		public static void Main(string[] args)
		{
			BuildWebHost(args).Run();
		}

		public static IWebHost BuildWebHost(string[] args)
		{
			var builder = WebHost.CreateDefaultBuilder(args);
			builder
				.ConfigureServices(s => s.AddSingleton(builder))
				.ConfigureAppConfiguration(ic => ic.AddJsonFile(Path.Combine("configuration", "configuration.json")))
				.UseStartup<Startup>();
			var host = builder.Build();
			return host;
		}
	}
}
