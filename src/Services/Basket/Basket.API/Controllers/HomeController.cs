using Microsoft.AspNetCore.Mvc;

namespace Microsoft.eShopOnContainers.Services.Basket.API.Controllers
{
	public class HomeController : Controller
	{
		// GET: /<controller>/
		public IActionResult Index() 
			=> new RedirectResult("~/swagger");
	}
}
