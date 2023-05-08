var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddSession(opt =>
    {
        opt.Cookie.Name = ".eShopWebhooks.Session";
    })
    .Configure<WebhookClientOptions>(builder.Configuration)
    .AddHttpClientServices(builder.Configuration)
    .AddCustomAuthentication(builder.Configuration)
    .AddTransient<IWebhooksClient, WebhooksClient>()
    .AddSingleton<IHooksRepository, InMemoryHooksRepository>()
    .AddMvc();
builder.Services.AddControllers();
var app = builder.Build();
app.UseServiceDefaults();

app.Map("/check", capp =>
{
    capp.Run(async (context) =>
    {
        if ("OPTIONS".Equals(context.Request.Method, StringComparison.InvariantCultureIgnoreCase))
        {
            var validateToken = bool.TrueString.Equals(builder.Configuration["ValidateToken"], StringComparison.InvariantCultureIgnoreCase);
            var header = context.Request.Headers[HeaderNames.WebHookCheckHeader];
            var value = header.FirstOrDefault();
            var tokenToValidate = builder.Configuration["Token"];
            if (!validateToken || value == tokenToValidate)
            {
                if (!string.IsNullOrWhiteSpace(tokenToValidate))
                {
                    context.Response.Headers.Add(HeaderNames.WebHookCheckHeader, tokenToValidate);
                }
                context.Response.StatusCode = (int)HttpStatusCode.OK;
            }
            else
            {
                await context.Response.WriteAsync("Invalid token");
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
    });
});

// Fix samesite issue when running eShop from docker-compose locally as by default http protocol is being used
// Refer to https://github.com/dotnet-architecture/eShopOnContainers/issues/1391
app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });

app.UseStaticFiles();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultControllerRoute();
app.MapRazorPages();

await app.RunAsync();
