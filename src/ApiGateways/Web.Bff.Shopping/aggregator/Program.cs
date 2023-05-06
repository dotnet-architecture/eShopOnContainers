var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllers();

builder.Services.AddHealthChecks(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
        .SetIsOriginAllowed((host) => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

builder.Services.AddApplicationServices();
builder.Services.AddGrpcServices();

builder.Services.Configure<UrlsConfig>(builder.Configuration.GetSection("urls"));

var app = builder.Build();

if (!await app.CheckHealthAsync())
{
    return;
}

app.UseServiceDefaults();

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapControllers();

await app.RunAsync();
