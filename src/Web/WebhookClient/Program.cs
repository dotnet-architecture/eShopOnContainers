var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.Configure<WebhookClientOptions>(builder.Configuration);
builder.Services.AddHttpClientServices(builder.Configuration);
builder.Services.AddCustomAuthentication(builder.Configuration);
builder.Services.AddTransient<IWebhooksClient, WebhooksClient>();
builder.Services.AddSingleton<IHooksRepository, InMemoryHooksRepository>();

builder.Services.AddRazorPages();
builder.Services.AddControllers();

var app = builder.Build();
app.UseServiceDefaults();

// Fix samesite issue when running eShop from docker-compose locally as by default http protocol is being used
// Refer to https://github.com/dotnet-architecture/eShopOnContainers/issues/1391
app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultControllerRoute();
app.MapRazorPages();

await app.RunAsync();
