using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace  Microsoft.eShopOnContainers.Services.Catalog.API.Controllers
{
    [Route("api/v1/[controller]")]
    public class ProductInfoController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string fileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
            string productVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;

            return Ok($"assemblyVersion: {assemblyVersion }, fileVersion: {fileVersion}, productVersion: {productVersion}");
        }
    }
}
