await BuildWebHost(args).RunAsync();

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
                .WriteTo.Seq("http://seq")
                .ReadFrom.Configuration(builderContext.Configuration)
                .WriteTo.Console();
        })
        .Build();
