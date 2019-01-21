using Microsoft.AspNetCore.Mvc;

namespace Webhooks.API.Controllers
{

    public class HomeController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return new RedirectResult("~/swagger");
        }

    }
}
