using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TenantAShippingInformation.Database;
using TenantAShippingInformation.Models;

namespace TenantAShippingInformation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingInformationsController : ControllerBase
    {
        private readonly TenantAContext _context;

        public ShippingInformationsController(TenantAContext context)
        {
            _context = context;
        }

        // GET: api/ShippingInformations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShippingInformation>>> GetShippingInformation()
        {
            return await _context.ShippingInformation.ToListAsync();
        }

        // GET: api/ShippingInformations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ShippingInformation>> GetShippingInformation(int id)
        {
            var shippingInformation = await _context.ShippingInformation.FindAsync(id);

            if (shippingInformation == null)
            {
                return NotFound();
            }

            return shippingInformation;
        }

        // PUT: api/ShippingInformations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutShippingInformation(int id, ShippingInformation shippingInformation)
        {
            if (id != shippingInformation.ShippingInformationId)
            {
                return BadRequest();
            }

            _context.Entry(shippingInformation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShippingInformationExists(id))
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

        // POST: api/ShippingInformations
        [HttpPost]
        public async Task<ActionResult<ShippingInformation>> PostShippingInformation(ShippingInformation shippingInformation)
        {
            _context.ShippingInformation.Add(shippingInformation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetShippingInformation", new { id = shippingInformation.ShippingInformationId }, shippingInformation);
        }

        // DELETE: api/ShippingInformations/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ShippingInformation>> DeleteShippingInformation(int id)
        {
            var shippingInformation = await _context.ShippingInformation.FindAsync(id);
            if (shippingInformation == null)
            {
                return NotFound();
            }

            _context.ShippingInformation.Remove(shippingInformation);
            await _context.SaveChangesAsync();

            return shippingInformation;
        }

        private bool ShippingInformationExists(int id)
        {
            return _context.ShippingInformation.Any(e => e.ShippingInformationId == id);
        }
    }
}
