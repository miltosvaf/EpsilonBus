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
    public class BranchesController : ControllerBase
    {
        private readonly EpsilonBusDbContext _context;

        public BranchesController(EpsilonBusDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Branch>>> GetBranches()
        {
            return await _context.Branches.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Branch>> GetBranch(int id)
        {
            var branch = await _context.Branches.FindAsync(id);
            if (branch == null)
                return NotFound();
            return branch;
        }

        [HttpPost]
        public async Task<ActionResult<Branch>> PostBranch(Branch branch)
        {
            _context.Branches.Add(branch);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBranch), new { id = branch.ID }, branch);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBranch(int id, Branch branch)
        {
            if (id != branch.ID)
                return BadRequest();
            _context.Entry(branch).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BranchExists(id))
                    return NotFound();
                else
                    throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            var branch = await _context.Branches.FindAsync(id);
            if (branch == null)
                return NotFound();
            _context.Branches.Remove(branch);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool BranchExists(int id)
        {
            return _context.Branches.Any(e => e.ID == id);
        }
    }
}