using Basket.API.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using static Microsoft.AspNetCore.Hosting.ApplicationInsightsWebHostBuilderExtensions;
using static Microsoft.AspNetCore.Hosting.HealthCheckWebHostBuilderExtension;
using static Microsoft.AspNetCore.Hosting.HostingAbstractionsWebHostBuilderExtensions;
using static Microsoft.AspNetCore.Hosting.WebHostBuilderExtensions;
using static Microsoft.AspNetCore.Hosting.WebHostExtensions;
using static Microsoft.Extensions.Configuration.AzureKeyVaultConfigurationExtensions;
using static Microsoft.Extensions.Configuration.ChainedBuilderExtensions;
using static Microsoft.Extensions.Configuration.EnvironmentVariablesExtensions;
using ConfigurationBuilder = Microsoft.Extensions.Configuration.ConfigurationBuilder;
using Convert = System.Convert;
using Directory = System.IO.Directory;
using IWebHost = Microsoft.AspNetCore.Hosting.IWebHost;
using WebHost = Microsoft.AspNetCore.WebHost;

namespace Microsoft.eShopOnContainers.Services.Basket.API
{
	public class Program
	{
		public static void Main(string[] args)
			=> BuildWebHost(args).Run();

		public static IWebHost BuildWebHost(string[] args) =>
		    WebHost.CreateDefaultBuilder(args)
			   .UseFailing(options =>
			   {
				   options.ConfigPath = "/Failing";
			   })
			   .UseHealthChecks("/hc")
			   .UseContentRoot(Directory.GetCurrentDirectory())
			   .UseStartup<Startup>()
			   .ConfigureAppConfiguration((builderContext, config) =>
			   {
				   var builtConfig = config.Build();

				   var configurationBuilder = new ConfigurationBuilder();

				   if (Convert.ToBoolean(builtConfig["UseVault"]))
				   {
					   configurationBuilder.AddAzureKeyVault(
						$"https://{builtConfig["Vault:Name"]}.vault.azure.net/",
						builtConfig["Vault:ClientId"],
						builtConfig["Vault:ClientSecret"]);
				   }

				   configurationBuilder.AddEnvironmentVariables();

				   config.AddConfiguration(configurationBuilder.Build());
			   })
			   .ConfigureLogging((hostingContext, builder) =>
			   {
				   builder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
				   builder.AddConsole();
				   builder.AddDebug();
			   })
			   .UseApplicationInsights()
			   .Build();
	}
}
