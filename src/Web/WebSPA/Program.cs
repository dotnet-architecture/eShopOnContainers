var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHealthChecks(builder.Configuration);
builder.Services.Configure<AppSettings>(builder.Configuration);

// Setup where the compiled version of our spa application will be, when in production. 
builder.Services.AddSpaStaticFiles(options =>
{
    options.RootPath = "wwwroot";
});

var app = builder.Build();

app.UseServiceDefaults();

app.UseFileServer();

// This will make the application to respond with the index.html and the rest of the assets present on the configured folder (at AddSpaStaticFiles() (wwwroot))
if (!app.Environment.IsDevelopment())
{
    app.UseSpaStaticFiles();
}

app.UseRouting();

#pragma warning disable ASP0014 // Suggest using top level route registrations
app.UseEndpoints(routes =>
{
    // TODO: Change this route
    routes.MapGet("/home/configuration", (IOptions<AppSettings> options) => options.Value);
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

// Seed Data
WebContextSeed.Seed(app, app.Environment, app.Logger);

await app.RunAsync();
