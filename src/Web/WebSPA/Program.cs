using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Serilog;
using eShopConContainers.WebSPA;

BuildWebHost(args).Run();

IWebHost BuildWebHost(string[] args) =>
    WebHost.CreateDefaultBuilder(args)
     .UseStartup<Startup>()
        .UseContentRoot(Directory.GetCurrentDirectory())
        .ConfigureAppConfiguration((builderContext, config) =>
        {
            config.AddEnvironmentVariables();
        })
        .ConfigureLogging((hostingContext, builder) =>
        {
            builder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
            builder.AddConsole();
            builder.AddDebug();
            builder.AddAzureWebAppDiagnostics();
        })
        .UseSerilog((builderContext, config) =>
        {
            config
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console();
        })
        .Build();
