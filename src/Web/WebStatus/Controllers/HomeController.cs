using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebStatus.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
        }

        public IActionResult Index()
        {
            return Redirect("/hc-ui");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
