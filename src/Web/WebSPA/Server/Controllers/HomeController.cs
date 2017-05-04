// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using eShopOnContainers.WebSPA;

namespace eShopConContainers.WebSPA.Server.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _env;
        private readonly IOptionsSnapshot<AppSettings> _settings;

        public HomeController(IHostingEnvironment env, IOptionsSnapshot<AppSettings> settings)
        {
            _env = env;
            _settings = settings;
        }
        public IActionResult Configuration()
        {
            return Json(_settings.Value);
        } 
    }
}
