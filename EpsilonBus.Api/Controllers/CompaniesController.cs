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
    public class CompaniesController : ControllerBase
    {
        private readonly EpsilonBusDbContext _context;

        public CompaniesController(EpsilonBusDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
        {
            return await _context.Companies.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetCompany(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
                return NotFound();
            return company;
        }

        [HttpPost]
        public async Task<ActionResult<Company>> PostCompany(Company company)
        {
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCompany), new { id = company.ID }, company);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompany(int id, Company company)
        {
            if (id != company.ID)
                return BadRequest();
            _context.Entry(company).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(id))
                    return NotFound();
                else
                    throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
                return NotFound();
            _context.Companies.Remove(company);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Optionally log the exception here
                return Conflict(new { message = "Delete failed due to related data or database constraints." });
            }
            return NoContent();
        }

        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.ID == id);
        }
    }
}