using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.WebMVC.Models;
using Microsoft.eShopOnContainers.WebMVC.Services;
using Microsoft.AspNetCore.Http.Authentication;

namespace Microsoft.eShopOnContainers.WebMVC.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IIdentityParser<ApplicationUser> _identityParser;
        public AccountController(IIdentityParser<ApplicationUser> identityParser)
        {
            _identityParser = identityParser;
        }

        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult SignIn(string returnUrl)
        {
            var user = User as ClaimsPrincipal;
            
            //TODO - Not retrieving AccessToken yet
            var token = user.FindFirst("access_token");
            if (token != null)
            {
                ViewData["access_token"] = token.Value;
            }

            return Redirect("/");
        }

        public IActionResult Signout()
        {
            HttpContext.Authentication.SignOutAsync("Cookies");
            HttpContext.Authentication.SignOutAsync("oidc");
             
            return new SignOutResult("oidc", new AuthenticationProperties { RedirectUri = "/" });
        }
    }
}
