
namespace Microsoft.eShopOnContainers.Services.Catalog.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.eShopOnContainers.Services.Catalog.API.Model;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    [Route("api/v1/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogContext _context;

        public CatalogController(CatalogContext context)
        {
            _context = context;
        }

        // GET api/v1/[controller]/all

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> All()
        {
            var items = await _context.CatalogItems
                .ToListAsync();

            return Ok(items);
        }

        // GET api/v1/[controller]/FindByName/samplename

        [HttpGet]
        [Route("FindByName/{name:minlength(1)}")]
        public async Task<IActionResult> Find(string name)
        {
            var items = await _context.CatalogItems
                .Where(c => c.Name.StartsWith(name, StringComparison.CurrentCultureIgnoreCase))
                .ToListAsync();

            return Ok();
        }
    }
}
