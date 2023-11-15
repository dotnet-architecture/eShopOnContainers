// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860
namespace eShopConContainers.WebSPA.Server.Controllers;

public class HomeController : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly IOptionsSnapshot<AppSettings> _settings;

    public HomeController(IWebHostEnvironment env, IOptionsSnapshot<AppSettings> settings)
    {
        _env = env;
        _settings = settings;
    }
    public IActionResult Configuration()
    {
        return Json(_settings.Value);
    } 
}
