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
    public class LanguagesController : ControllerBase
    {
        private readonly EpsilonBusDbContext _context;

        public LanguagesController(EpsilonBusDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Language>>> GetLanguages()
        {
            return await _context.Languages.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Language>> GetLanguage(int id)
        {
            var language = await _context.Languages.FindAsync(id);
            if (language == null)
                return NotFound();
            return language;
        }

        [HttpPost]
        public async Task<ActionResult<Language>> PostLanguage(Language language)
        {
            _context.Languages.Add(language);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetLanguage), new { id = language.ID }, language);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutLanguage(int id, Language language)
        {
            if (id != language.ID)
                return BadRequest();
            _context.Entry(language).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LanguageExists(id))
                    return NotFound();
                else
                    throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLanguage(int id)
        {
            var language = await _context.Languages.FindAsync(id);
            if (language == null)
                return NotFound();
            _context.Languages.Remove(language);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool LanguageExists(int id)
        {
            return _context.Languages.Any(e => e.ID == id);
        }
    }
}