using Microsoft.AspNetCore.Mvc;

namespace WebMVC.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Privacy()
		{
			return View();
		}

	}
}