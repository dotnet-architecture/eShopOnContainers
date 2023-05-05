var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHealthChecks(builder.Configuration);
builder.Services.AddApplicationOptions(builder.Configuration);
builder.Services.AddHostedService<GracePeriodManagerService>();

var app = builder.Build();

if (!await app.CheckHealthAsync())
{
    return;
}

app.UseServiceDefaults();

await app.RunAsync();
