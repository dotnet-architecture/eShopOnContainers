using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.WebMVC.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Microsoft.eShopOnContainers.WebMVC.Controllers
{
    public class HomeController : Controller
    {
        private HttpClient _http;
        private AppSettings _settings;

        public HomeController(IOptions<AppSettings> options)
        {
            _http = new HttpClient();
            _settings = options.Value;
        }
        public async Task<IActionResult> Index()
        {
            var dataString = await _http.GetStringAsync(_settings.CatalogUrl);
            var items = JsonConvert.DeserializeObject<List<CatalogItem>>(dataString);
            return View(items);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
