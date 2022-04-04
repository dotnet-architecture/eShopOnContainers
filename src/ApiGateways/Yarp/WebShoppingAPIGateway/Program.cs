var builder = WebApplication.CreateBuilder(args);
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddCors(options =>
{
    options.AddPolicy("customPolicy", builder =>
    {
        builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});
var app = builder.Build();
app.UseCors();
app.MapGet("/", () => "Hello World!");

app.MapReverseProxy();

app.Run();
