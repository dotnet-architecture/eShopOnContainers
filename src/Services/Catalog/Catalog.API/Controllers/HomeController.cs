using Microsoft.AspNetCore.Mvc;

namespace Microsoft.eShopOnContainers.Services.Catalog.API.Controllers
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
