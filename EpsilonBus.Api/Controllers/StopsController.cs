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
    public class StopsController : ControllerBase
    {
        private readonly EpsilonBusDbContext _context;

        public StopsController(EpsilonBusDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Stop>>> GetStops()
        {
            return await _context.Stops.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Stop>> GetStop(int id)
        {
            var stop = await _context.Stops.FindAsync(id);
            if (stop == null)
                return NotFound();
            return stop;
        }

        [HttpPost]
        public async Task<ActionResult<Stop>> PostStop(Stop stop)
        {
            _context.Stops.Add(stop);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetStop), new { id = stop.ID }, stop);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutStop(int id, Stop stop)
        {
            if (id != stop.ID)
                return BadRequest();
            _context.Entry(stop).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StopExists(id))
                    return NotFound();
                else
                    throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStop(int id)
        {
            var stop = await _context.Stops.FindAsync(id);
            if (stop == null)
                return NotFound();
            _context.Stops.Remove(stop);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool StopExists(int id)
        {
            return _context.Stops.Any(e => e.ID == id);
        }
    }
}