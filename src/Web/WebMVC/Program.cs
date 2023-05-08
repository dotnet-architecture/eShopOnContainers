var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllersWithViews();

builder.AddHealthChecks();
builder.AddApplicationSevices();
builder.AddHttpClientServices();
builder.AddAuthenticationServices();

var app = builder.Build();

app.UseServiceDefaults();

app.UseStaticFiles();
app.UseSession();

// Fix samesite issue when running eShop from docker-compose locally as by default http protocol is being used
// Refer to https://github.com/dotnet-architecture/eShopOnContainers/issues/1391
app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute("default", "{controller=Catalog}/{action=Index}/{id?}");
app.MapControllerRoute("defaultError", "{controller=Error}/{action=Error}");
app.MapControllers();

WebContextSeed.Seed(app, app.Environment);

await app.RunAsync();
