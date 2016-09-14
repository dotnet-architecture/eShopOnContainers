using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.Services.Catalog.API.Model;

namespace Microsoft.eShopOnContainers.Services.Catalog.API.Controllers
{
    [Route("/")]
    public class CatalogController : ControllerBase
    {
        private CatalogContext _context;

        public CatalogController(CatalogContext context)
        {
            _context = context;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<CatalogItem> Get()
        {
            return _context.CatalogItems.ToList();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var item = _context.CatalogItems.FirstOrDefault(x=> x.Id == id);

            if(item == null)
            {
                return NotFound();
            }

            return new OkObjectResult(item);
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]CatalogItem item)
        {
            try
            {
                _context.CatalogItems.Add(item);
                _context.SaveChanges();
                return Ok();
            }
            catch
            {
                return StatusCode(500, "Unable to add new catalog item");
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]CatalogItem item)
        {
            _context.CatalogItems.Update(item);
            _context.SaveChanges();
            return Ok();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            return Ok();
        }
    }
}
