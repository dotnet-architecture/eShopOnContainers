try
{
    var builder = WebApplication.CreateBuilder(args);
    if (builder.Configuration.GetValue<bool>("UseVault", false))
    {
        TokenCredential credential = new ClientSecretCredential(
            builder.Configuration["Vault:TenantId"],
            builder.Configuration["Vault:ClientId"],
            builder.Configuration["Vault:ClientSecret"]);
        builder.Configuration.AddAzureKeyVault(new Uri($"https://{builder.Configuration["Vault:Name"]}.vault.azure.net/"), credential);
    }

    builder.Host.UseSerilog(CreateLogger(builder.Configuration));
    builder.WebHost.CaptureStartupErrors(false);
    builder.Services.AddApplicationInsightsTelemetry(builder.Configuration);
    builder.Services.AddApplicationInsightsKubernetesEnricher();
    builder.Services.AddMvc();
    builder.Services.AddOptions();
    builder.Services.AddHealthChecks()
        .AddCheck("self", () => HealthCheckResult.Healthy());
    builder.Services
        .AddHealthChecksUI()
        .AddInMemoryStorage();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
    }

    var pathBase = app.Configuration["PATH_BASE"];
    if (!string.IsNullOrEmpty(pathBase))
    {
        app.UsePathBase(pathBase);
    }

    app.UseHealthChecksUI(config =>
    {
        config.ResourcesPath = string.IsNullOrEmpty(pathBase) ? "/ui/resources" : $"{pathBase}/ui/resources";
        config.UIPath = "/hc-ui";
    });

    app.UseStaticFiles();

    app.UseRouting();
    app.MapDefaultControllerRoute();
    app.MapHealthChecks("/liveness", new HealthCheckOptions
    {
        Predicate = r => r.Name.Contains("self")
    });

    await app.RunAsync();
}
catch (Exception exception)
{
    Console.WriteLine(exception);
}

Serilog.ILogger CreateLogger(IConfiguration configuration)
{
    var seqServerUrl = configuration["Serilog:SeqServerUrl"];
    var logstashUrl = configuration["Serilog:LogstashgUrl"];
    return new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .Enrich.WithProperty("ApplicationContext", Program.AppName)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.Seq(string.IsNullOrWhiteSpace(seqServerUrl) ? "http://seq" : seqServerUrl)
        .WriteTo.Http(string.IsNullOrWhiteSpace(logstashUrl) ? "http://logstash:8080" : logstashUrl, null)
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}

public partial class Program
{
    private static readonly string _namespace = typeof(Program).Assembly.GetName().Name;
    public static readonly string AppName = _namespace;
}