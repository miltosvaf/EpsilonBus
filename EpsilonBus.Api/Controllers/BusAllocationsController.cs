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
    public class BusAllocationsController : ControllerBase
    {
        private readonly EpsilonBusDbContext _context;

        public BusAllocationsController(EpsilonBusDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BusAllocation>>> GetBusAllocations()
        {
            return await _context.BusAllocations.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BusAllocation>> GetBusAllocation(int id)
        {
            var allocation = await _context.BusAllocations.FindAsync(id);
            if (allocation == null)
                return NotFound();
            return allocation;
        }

        [HttpPost]
        public async Task<ActionResult<BusAllocation>> PostBusAllocation(BusAllocation allocation)
        {
            _context.BusAllocations.Add(allocation);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBusAllocation), new { id = allocation.ID }, allocation);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBusAllocation(int id, BusAllocation allocation)
        {
            if (id != allocation.ID)
                return BadRequest();
            _context.Entry(allocation).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BusAllocationExists(id))
                    return NotFound();
                else
                    throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBusAllocation(int id)
        {
            var allocation = await _context.BusAllocations.FindAsync(id);
            if (allocation == null)
                return NotFound();
            _context.BusAllocations.Remove(allocation);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool BusAllocationExists(int id)
        {
            return _context.BusAllocations.Any(e => e.ID == id);
        }
    }
}