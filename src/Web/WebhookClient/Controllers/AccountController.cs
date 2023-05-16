namespace WebhookClient.Controllers;

[Authorize]
public class AccountController : Controller
{
    public async Task<IActionResult> SignIn(string returnUrl)
    {
        var token = await HttpContext.GetTokenAsync("access_token");

        if (token != null)
        {
            ViewData["access_token"] = token;
        }

        return RedirectToPage("/Index");
    }

    public async Task<IActionResult> Signout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        var homeUrl = Url.Page("/Index");
        return new SignOutResult(OpenIdConnectDefaults.AuthenticationScheme,
            new AuthenticationProperties { RedirectUri = homeUrl });
    }
}
