using Autofac.Extensions.DependencyInjection;
using Catalog.API.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.eShopOnContainers.BuildingBlocks.IntegrationEventLogEF;
using Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Microsoft.eShopOnContainers.Services.Catalog.API
{
    public class Program
    {
        public static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);

        public static int Main(string[] args)
        {
            var configuration = GetConfiguration();

            Log.Logger = CreateSerilogLogger(configuration);

            try
            {
                Log.Information("Configuring web host ({ApplicationContext})...", AppName);
                var host = CreateHostBuilder(configuration, args).Build();

                Log.Information("Applying migrations ({ApplicationContext})...", AppName);
                host.MigrateDbContext<CatalogContext>((context, services) =>
                {
                    var env = services.GetService<IWebHostEnvironment>();
                    var settings = services.GetService<IOptions<CatalogSettings>>();
                    var logger = services.GetService<ILogger<CatalogContextSeed>>();

                    new CatalogContextSeed()
                        .SeedAsync(context, env, settings, logger)
                        .Wait();
                })
                .MigrateDbContext<IntegrationEventLogContext>((_, __) => { });

                Log.Information("Starting web host ({ApplicationContext})...", AppName);
                host.Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }


        private static IHostBuilder CreateHostBuilder(IConfiguration configuration, string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(services => services.AddAutofac())
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.CaptureStartupErrors(false)
                    .UseConfiguration(configuration)
                    .ConfigureKestrel(options =>
                    {
                        var ports = GetDefinedPorts(configuration);
                        foreach (var port in ports.Distinct())
                        {
                            options.ListenAnyIP(port.portNumber, listenOptions =>
                            {
                                Console.WriteLine($"Binding to port {port.portNumber} (https is {port.https})");
                                if (port.https)
                                {
                                    listenOptions.UseHttps("eshop.pfx");
                                    listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                                }
                            });
                        }
                    })
                    .UseStartup<Startup>()
                    .UseApplicationInsights()
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseWebRoot("Pics")
                    .UseSerilog();
                });

        private static Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            var seqServerUrl = configuration["Serilog:SeqServerUrl"];
            var logstashUrl = configuration["Serilog:LogstashgUrl"];
            return new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.WithProperty("ApplicationContext", AppName)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq(string.IsNullOrWhiteSpace(seqServerUrl) ? "http://seq" : seqServerUrl)
                .WriteTo.Http(string.IsNullOrWhiteSpace(logstashUrl) ? "http://logstash:8080" : logstashUrl)
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        private static IEnumerable<(int portNumber, bool https)> GetDefinedPorts(IConfiguration config)
        {
            const string https = "https://";
            const string http = "http://";
            var defport = config.GetValue("ASPNETCORE_HTTPS_PORT", 0);
            if (defport != 0)
            {
                yield return (defport, true);
            }

            var urls = config.GetValue<string>("ASPNETCORE_URLS", null)?.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (urls?.Any() == true)
            {
                foreach (var urlString in urls)
                {
                    var uri = urlString.ToLowerInvariant().Trim();
                    var isHttps = uri.StartsWith(https);
                    var isHttp = uri.StartsWith(http);
                    if (!isHttp && !isHttps)
                    {
                        throw new ArgumentException($"Url {uri} must start with https:// or http://");
                    }

                    uri = uri.Substring(isHttps ? https.Length : http.Length);
                    var lastdots = uri.LastIndexOf(':');
                    if (lastdots != -1)
                    {
                        var sport = uri.Substring(lastdots + 1);
                        yield return (int.TryParse(sport, out var nport) ? nport : isHttps ? 443 : 80, isHttps);
                    }
                }
            }
        }

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var config = builder.Build();

            if (config.GetValue<bool>("UseVault", false))
            {
                builder.AddAzureKeyVault(
                    $"https://{config["Vault:Name"]}.vault.azure.net/",
                    config["Vault:ClientId"],
                    config["Vault:ClientSecret"]);
            }

            return builder.Build();
        }
    }
}
