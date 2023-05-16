var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHealthChecks(builder.Configuration);
builder.Services.AddApplicationOptions(builder.Configuration);
builder.Services.AddHostedService<GracePeriodManagerService>();

var app = builder.Build();

app.UseServiceDefaults();

await app.RunAsync();
