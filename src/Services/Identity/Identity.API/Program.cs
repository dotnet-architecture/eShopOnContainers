var appName = "Identity.API";
var builder = WebApplication.CreateBuilder();

if (builder.Configuration.GetValue<bool>("UseVault", false))
{
    TokenCredential credential = new ClientSecretCredential(
        builder.Configuration["Vault:TenantId"],
        builder.Configuration["Vault:ClientId"],
        builder.Configuration["Vault:ClientSecret"]);
    builder.Configuration.AddAzureKeyVault(new Uri($"https://{builder.Configuration["Vault:Name"]}.vault.azure.net/"), credential);
}

builder.AddCustomConfiguration();
builder.AddCustomSerilog();
builder.AddCustomMvc();
builder.AddCustomDatabase();
builder.AddCustomIdentity();
builder.AddCustomIdentityServer();
builder.AddCustomAuthentication();
builder.AddCustomHealthChecks();
builder.AddCustomApplicationServices();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

var pathBase = builder.Configuration["PATH_BASE"];
if (!string.IsNullOrEmpty(pathBase))
{
    app.UsePathBase(pathBase);
}
app.UseStaticFiles();

// This cookie policy fixes login issues with Chrome 80+ using HHTP
app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });

app.UseRouting();

app.UseIdentityServer();


app.UseAuthorization();

app.MapDefaultControllerRoute();

app.MapHealthChecks("/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecks("/liveness", new HealthCheckOptions
{
    Predicate = r => r.Name.Contains("self")
});
try
{
    app.Logger.LogInformation("Seeding database ({ApplicationName})...", appName);

    // Apply database migration automatically. Note that this approach is not
    // recommended for production scenarios. Consider generating SQL scripts from
    // migrations instead.
    using (var scope = app.Services.CreateScope())
    {
        await SeedData.EnsureSeedData(scope, app.Configuration, app.Logger);
    }

    app.Logger.LogInformation("Starting web host ({ApplicationName})...", appName);
    app.Run();

    return 0;
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Host terminated unexpectedly ({ApplicationName})...", appName);
    return 1;
}
finally
{
    Serilog.Log.CloseAndFlush();
}