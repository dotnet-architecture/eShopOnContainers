var builder = WebApplication.CreateBuilder(args);
if (builder.Configuration.GetValue("UseVault", false))
{
    TokenCredential credential = new ClientSecretCredential(
        builder.Configuration["Vault:TenantId"],
        builder.Configuration["Vault:ClientId"],
        builder.Configuration["Vault:ClientSecret"]);
    builder.Configuration.AddAzureKeyVault(new Uri($"https://{builder.Configuration["Vault:Name"]}.vault.azure.net/"), credential);
}

builder.WebHost.CaptureStartupErrors(false);
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration);
builder.Services.AddApplicationInsightsKubernetesEnricher();
builder.Services.AddMvc();
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy());
builder.Services
    .AddHealthChecksUI()
    .AddInMemoryStorage();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
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
