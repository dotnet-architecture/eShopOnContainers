var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseContentRoot(Directory.GetCurrentDirectory());

builder.Services.AddApplicationInsightsTelemetry(builder.Configuration);
builder.Services.AddApplicationInsightsKubernetesEnricher();
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy())
    .AddUrlGroup(new Uri(builder.Configuration["IdentityUrlHC"]), name: "identityapi-check", tags: new string[] { "identityapi" });

builder.Services.Configure<AppSettings>(builder.Configuration);
if (builder.Configuration.GetValue<string>("IsClusterEnv") == bool.TrueString)
{
    builder.Services.AddDataProtection(opts =>
    {
        opts.ApplicationDiscriminator = "eshop.webspa";
    })
    .PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect(builder.Configuration["DPConnectionString"]), "DataProtection-Keys");
}


// Add Anti-forgery services and configure the header name that angular will use by default.
builder.Services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

// Add controllers support and add a global AutoValidateAntiforgeryTokenFilter that will make the application check for an Anti-forgery token on all "mutating" requests (POST, PUT, DELETE).
// The AutoValidateAntiforgeryTokenFilter is an internal class registered when we register views, so we need to register controllers and views also.
builder.Services.AddControllersWithViews(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()))
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// Setup where the compiled version of our spa application will be, when in production. 
builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "wwwroot";
});

builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddAzureWebAppDiagnostics();
builder.Host.UseSerilog((builderContext, config) =>
{
    config
        .MinimumLevel.Information()
        .Enrich.FromLogContext()
        .WriteTo.Seq("http://seq")
        .ReadFrom.Configuration(builderContext.Configuration)
        .WriteTo.Console();
})
.UseConsoleLifetime();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Here we add Angular default Anti-forgery cookie name on first load. https://angular.io/guide/http#security-xsrf-protection
// This cookie will be read by Angular app and its value will be sent back to the application as the header configured in .AddAntiforgery()
var antiForgery = app.Services.GetRequiredService<IAntiforgery>();
app.Use(next => context =>
{
    string path = context.Request.Path.Value;

    if (string.Equals(path, "/", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(path, "/index.html", StringComparison.OrdinalIgnoreCase))
    {
        // The request token has to be sent as a JavaScript-readable cookie, 
        // and Angular uses it by default.
        var tokens = antiForgery.GetAndStoreTokens(context);
        context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken,
            new CookieOptions() { HttpOnly = false });
    }

    return next(context);
});

// Seed Data
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
WebContextSeed.Seed(app, app.Environment, loggerFactory);

var pathBase = app.Configuration["PATH_BASE"];

if (!string.IsNullOrEmpty(pathBase))
{
    loggerFactory.CreateLogger<Program>().LogDebug("Using PATH_BASE '{PathBase}'", pathBase);
    app.UsePathBase(pathBase);
}

app.UseDefaultFiles();
app.UseStaticFiles();

// This will make the application to respond with the index.html and the rest of the assets present on the configured folder (at AddSpaStaticFiles() (wwwroot))
if (!app.Environment.IsDevelopment())
{
    app.UseSpaStaticFiles();
}

app.UseRouting();
app.MapDefaultControllerRoute();
app.MapControllers();
app.MapHealthChecks("/liveness", new HealthCheckOptions
{
    Predicate = r => r.Name.Contains("self")
});
app.MapHealthChecks("/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// Handles all still unattended (by any other middleware) requests by returning the default page of the SPA (wwwroot/index.html).
app.UseSpa(spa =>
{
    // To learn more about options for serving an Angular SPA from ASP.NET Core,
    // see https://go.microsoft.com/fwlink/?linkid=864501

    // the root of the angular app. (Where the package.json lives)
    spa.Options.SourcePath = "Client";

    if (app.Environment.IsDevelopment())
    {
        // use the SpaServices extension method for angular, that will make the application to run "ng serve" for us, when in development.
        spa.UseAngularCliServer(npmScript: "start");
    }
});

await app.RunAsync();
