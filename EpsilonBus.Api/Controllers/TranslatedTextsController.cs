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
    public class TranslatedTextsController : ControllerBase
    {
        private readonly EpsilonBusDbContext _context;

        public TranslatedTextsController(EpsilonBusDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TranslatedText>>> GetTranslatedTexts()
        {
            return await _context.TranslatedTexts.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TranslatedText>> GetTranslatedText(int id)
        {
            var translatedText = await _context.TranslatedTexts.FindAsync(id);
            if (translatedText == null)
                return NotFound();
            return translatedText;
        }

        [HttpPost]
        public async Task<ActionResult<TranslatedText>> PostTranslatedText(TranslatedText translatedText)
        {
            _context.TranslatedTexts.Add(translatedText);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTranslatedText), new { id = translatedText.ID }, translatedText);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTranslatedText(int id, TranslatedText translatedText)
        {
            if (id != translatedText.ID)
                return BadRequest();
            _context.Entry(translatedText).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TranslatedTextExists(id))
                    return NotFound();
                else
                    throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTranslatedText(int id)
        {
            var translatedText = await _context.TranslatedTexts.FindAsync(id);
            if (translatedText == null)
                return NotFound();
            _context.TranslatedTexts.Remove(translatedText);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool TranslatedTextExists(int id)
        {
            return _context.TranslatedTexts.Any(e => e.ID == id);
        }
    }
}