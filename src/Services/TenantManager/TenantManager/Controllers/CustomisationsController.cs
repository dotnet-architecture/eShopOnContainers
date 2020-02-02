using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TenantManager.Database;
using TenantManager.Models;

namespace TenantManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomisationsController : ControllerBase
    {
        private readonly TenantManagerContext _context;

        public CustomisationsController(TenantManagerContext context)
        {
            _context = context;
        }

        // GET: api/Customisations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customisation>>> GetCustomisation()
        {
            return await _context.Customisation.ToListAsync();
        }

        // GET: api/Customisations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customisation>> GetCustomisation(int id)
        {
            var customisation = await _context.Customisation.FindAsync(id);

            if (customisation == null)
            {
                return NotFound();
            }

            return customisation;
        }
        
        // GET: api/Customisations/5
        [HttpGet("isCustomised")]
        public async Task<ActionResult<string>> IsCustomised(String eventName, int tenantId)
        {
            var customisation = await _context.Customisation.Include(c => c.Method).Include(c => c.Tenant).Where(c => c.Method.MethodName.Equals(eventName) && c.TenantId == tenantId).FirstOrDefaultAsync();

            if (customisation == null)
            {
                return NotFound();
            }

            return customisation.CustomisationUrl;
        }

        // PUT: api/Customisations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomisation(int id, Customisation customisation)
        {
            if (id != customisation.CustomisationId)
            {
                return BadRequest();
            }

            _context.Entry(customisation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomisationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Customisations
        [HttpPost]
        public async Task<ActionResult<Customisation>> PostCustomisation(Customisation customisation)
        {
            _context.Customisation.Add(customisation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCustomisation", new { id = customisation.CustomisationId }, customisation);
        }

        // DELETE: api/Customisations/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Customisation>> DeleteCustomisation(int id)
        {
            var customisation = await _context.Customisation.FindAsync(id);
            if (customisation == null)
            {
                return NotFound();
            }

            _context.Customisation.Remove(customisation);
            await _context.SaveChangesAsync();

            return customisation;
        }

        private bool CustomisationExists(int id)
        {
            return _context.Customisation.Any(e => e.CustomisationId == id);
        }
    }
}
