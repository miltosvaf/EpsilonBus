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
    public class BusinessLogicRulesController : ControllerBase
    {
        private readonly EpsilonBusDbContext _context;

        public BusinessLogicRulesController(EpsilonBusDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BusinessLogicRule>>> GetBusinessLogicRules()
        {
            return await _context.BusinessLogicRules.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BusinessLogicRule>> GetBusinessLogicRule(int id)
        {
            var rule = await _context.BusinessLogicRules.FindAsync(id);
            if (rule == null)
                return NotFound();
            return rule;
        }

        [HttpPost]
        public async Task<ActionResult<BusinessLogicRule>> PostBusinessLogicRule(BusinessLogicRule rule)
        {
            _context.BusinessLogicRules.Add(rule);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBusinessLogicRule), new { id = rule.ID }, rule);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBusinessLogicRule(int id, BusinessLogicRule rule)
        {
            if (id != rule.ID)
                return BadRequest();
            _context.Entry(rule).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BusinessLogicRuleExists(id))
                    return NotFound();
                else
                    throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBusinessLogicRule(int id)
        {
            var rule = await _context.BusinessLogicRules.FindAsync(id);
            if (rule == null)
                return NotFound();
            _context.BusinessLogicRules.Remove(rule);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool BusinessLogicRuleExists(int id)
        {
            return _context.BusinessLogicRules.Any(e => e.ID == id);
        }
    }
}