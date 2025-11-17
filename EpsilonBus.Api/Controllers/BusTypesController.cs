using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EpsilonBus.Api.Data;
using EpsilonBus.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EpsilonBus.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusTypesController : ControllerBase
    {
        private readonly EpsilonBusDbContext _context;

        public BusTypesController(EpsilonBusDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BusType>>> GetBusTypes()
        {
            return await _context.BusTypes.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BusType>> GetBusType(int id)
        {
            var busType = await _context.BusTypes.FindAsync(id);
            if (busType == null)
                return NotFound();
            return busType;
        }

        [HttpPost]
        public async Task<ActionResult<BusType>> PostBusType(BusType busType)
        {
            _context.BusTypes.Add(busType);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBusType), new { id = busType.ID }, busType);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBusType(int id, BusType busType)
        {
            if (id != busType.ID)
                return BadRequest();
            _context.Entry(busType).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BusTypeExists(id))
                    return NotFound();
                else
                    throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBusType(int id)
        {
            var busType = await _context.BusTypes.FindAsync(id);
            if (busType == null)
                return NotFound();
            _context.BusTypes.Remove(busType);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool BusTypeExists(int id)
        {
            return _context.BusTypes.Any(e => e.ID == id);
        }
    }
}