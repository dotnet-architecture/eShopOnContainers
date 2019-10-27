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
    public class MethodsController : ControllerBase
    {
        private readonly TenantManagerContext _context;

        public MethodsController(TenantManagerContext context)
        {
            _context = context;
        }

        // GET: api/Methods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Method>>> GetMethod()
        {
            return await _context.Method.ToListAsync();
        }

        // GET: api/Methods/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Method>> GetMethod(int id)
        {
            var @method = await _context.Method.FindAsync(id);

            if (@method == null)
            {
                return NotFound();
            }

            return @method;
        }

        // PUT: api/Methods/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMethod(int id, Method @method)
        {
            if (id != @method.MethodId)
            {
                return BadRequest();
            }

            _context.Entry(@method).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MethodExists(id))
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

        // POST: api/Methods
        [HttpPost]
        public async Task<ActionResult<Method>> PostMethod(Method @method)
        {
            _context.Method.Add(@method);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMethod", new { id = @method.MethodId }, @method);
        }

        // DELETE: api/Methods/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Method>> DeleteMethod(int id)
        {
            var @method = await _context.Method.FindAsync(id);
            if (@method == null)
            {
                return NotFound();
            }

            _context.Method.Remove(@method);
            await _context.SaveChangesAsync();

            return @method;
        }

        private bool MethodExists(int id)
        {
            return _context.Method.Any(e => e.MethodId == id);
        }
    }
}
