var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddReverseProxy(builder.Configuration);
builder.Services.AddControllers();

builder.Services.AddHealthChecks(builder.Configuration);

builder.Services.AddApplicationServices();
builder.Services.AddGrpcServices();

builder.Services.Configure<UrlsConfig>(builder.Configuration.GetSection("urls"));

var app = builder.Build();

app.UseServiceDefaults();

app.UseHttpsRedirection();

app.MapControllers();
app.MapReverseProxy();

await app.RunAsync();
