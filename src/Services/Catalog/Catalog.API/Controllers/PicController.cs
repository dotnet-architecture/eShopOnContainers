using Catalog.API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopOnContainers.Services.Catalog.API.Extensions;
using Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure;
using Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure.Services;
using Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure.Validators;
using Microsoft.Extensions.Options;
using System.IO;
using System.Net;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Microsoft.eShopOnContainers.Services.Catalog.API.Controllers
{
    [ApiController]
    public class PicController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly CatalogContext _catalogContext;
        private readonly IOptions<CatalogSettings> _options;
        private readonly IPicService _picService;
        private readonly IPicServicesHandler _picServicesHandler;

        public PicController(IWebHostEnvironment env,
                    CatalogContext catalogContext,
                    IOptions<CatalogSettings> options,
                    IPicService picService,
                    IPicServicesHandler picServicesHandler)
        {
            _env = env;
            _options = options;
            _picService = picService;
            _catalogContext = catalogContext;
            _picServicesHandler = picServicesHandler;
            _picServicesHandler.Subscrib(_picService);
        }

        [HttpGet]
        [Route("api/v1/catalog/items/{catalogItemId:int}/pic")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        // GET: /<controller>/
        public async Task<ActionResult> GetImageAsync(int catalogItemId)
        {
            if (catalogItemId <= 0)
            {
                return BadRequest();
            }

            var item = await _catalogContext.CatalogItems
                .SingleOrDefaultAsync(ci => ci.Id == catalogItemId);

            if (item != null)
            {
                var webRoot = _env.WebRootPath;
                var path = Path.Combine(webRoot, item.PictureFileName);

                string imageFileExtension = Path.GetExtension(item.PictureFileName);
                string mimetype = GetImageMimeTypeFromImageFileExtension(imageFileExtension);

                var buffer = await System.IO.File.ReadAllBytesAsync(path);

                return File(buffer, mimetype);
            }

            return NotFound();
        }

        [HttpPost]
        [Authorize]
        [Route("api/v1/catalog/items/{catalogItemId:int}/pic")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public ActionResult SaveImage(IFormFile imgfile, int catalogItemId)
        {
            var rule = new IsFileNotNull().And(new IsFileSizeSuitable(_options)).And(new IsFileExtntionSuitable()).And(new IsFileSignatureSuitable());
            if (!rule.IsSatisfiedBy(imgfile)) return BadRequest("File size should less than 2Mb, Type should be [JPG, JPEG, PNG] and not empty.");
            _picService.UploadFile(new Payload(imgfile, catalogItemId));
            return Ok("The file went to storage.");
        }

        private string GetImageMimeTypeFromImageFileExtension(string extension)
        {
            string mimetype;

            switch (extension)
            {
                case ".png":
                    mimetype = "image/png";
                    break;
                case ".gif":
                    mimetype = "image/gif";
                    break;
                case ".jpg":
                case ".jpeg":
                    mimetype = "image/jpeg";
                    break;
                case ".bmp":
                    mimetype = "image/bmp";
                    break;
                case ".tiff":
                    mimetype = "image/tiff";
                    break;
                case ".wmf":
                    mimetype = "image/wmf";
                    break;
                case ".jp2":
                    mimetype = "image/jp2";
                    break;
                case ".svg":
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
