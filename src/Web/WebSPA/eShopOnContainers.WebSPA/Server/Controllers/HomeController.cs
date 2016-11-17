// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace eShopConContainers.WebSPA.Server.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _env;

        public HomeController(IHostingEnvironment env)
        {
            _env = env;
        }

        public IActionResult Index()
        {
            ViewBag.HashedMain = GetHashedMainDotJs();

            return View();
        }

        public string GetHashedMainDotJs()
        {
            var basePath = _env.WebRootPath + "//dist//";
            var info = new System.IO.DirectoryInfo(basePath);
            var file = info.GetFiles().Where(f => f.Name.StartsWith("main.") && !f.Name.EndsWith("bundle.map")).FirstOrDefault();

            return file.Name;
        }

    }
}
