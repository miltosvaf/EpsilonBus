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
    public class NonWorkingDaysController : ControllerBase
    {
        private readonly EpsilonBusDbContext _context;

        public NonWorkingDaysController(EpsilonBusDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NonWorkingDay>>> GetNonWorkingDays()
        {
            return await _context.NonWorkingDays.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NonWorkingDay>> GetNonWorkingDay(int id)
        {
            var day = await _context.NonWorkingDays.FindAsync(id);
            if (day == null)
                return NotFound();
            return day;
        }

        [HttpPost]
        public async Task<ActionResult<NonWorkingDay>> PostNonWorkingDay(NonWorkingDay day)
        {
            _context.NonWorkingDays.Add(day);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetNonWorkingDay), new { id = day.ID }, day);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutNonWorkingDay(int id, NonWorkingDay day)
        {
            if (id != day.ID)
                return BadRequest();
            _context.Entry(day).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NonWorkingDayExists(id))
                    return NotFound();
                else
                    throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNonWorkingDay(int id)
        {
            var day = await _context.NonWorkingDays.FindAsync(id);
            if (day == null)
                return NotFound();
            _context.NonWorkingDays.Remove(day);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool NonWorkingDayExists(int id)
        {
            return _context.NonWorkingDays.Any(e => e.ID == id);
        }
    }
}