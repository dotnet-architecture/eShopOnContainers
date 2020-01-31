using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TenantACustomisations.Database;
using TenantACustomisations.ExternalServices;

namespace TenantACustomisations.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {

        private readonly TenantAContext _context;

        public ValuesController(TenantAContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }



        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShippingInformation>>> GetShippingInformation()
        {
            return await _context.ShippingInformation.ToListAsync();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
