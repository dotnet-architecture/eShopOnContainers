using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Microsoft.eShopOnContainers.Services.Catalog.API.Controllers
{
    [Route("api/v1/[controller]")]
    public class PicController : Controller
    {
        private readonly IHostingEnvironment _env;
        public PicController(IHostingEnvironment env)
        {
            _env = env;
        }

        [HttpGet("{filename}")]
        // GET: /<controller>/
        public IActionResult GetImage(string filename)
        {
            var webRoot = _env.WebRootPath;
            var path = Path.Combine(webRoot, filename);

            string imageFileExtension = Path.GetExtension(filename);
            string mimetype = GetImageMimeTypeFromImageFileExtension(imageFileExtension);

            var buffer = System.IO.File.ReadAllBytes(path);

            return File(buffer, mimetype);
        }

        private string GetImageMimeTypeFromImageFileExtension(string extension)
        {
            string mimetype;

            switch (extension)
            {
                case "png":
                    mimetype = "image/png";
                    break;
                case "gif":
                    mimetype = "image/gif";
                    break;
                case "jpg":
                case "jpeg":
                    mimetype = "image/jpeg";
                    break;
                case "bmp":
                    mimetype = "image/bmp";
                    break;
                case "tiff":
                    mimetype = "image/tiff";
                    break;
                case "wmf":
                    mimetype = "image/wmf";
                    break;
                case "jp2":
                    mimetype = "image/jp2";
                    break;
                case "svg":
                    mimetype = "image/svg+xml";
                    break;
                default:
                    mimetype = "application/octet-stream";
                    break;
            }

            return mimetype;
        }
    }
}
