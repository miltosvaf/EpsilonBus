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
    public class BookingsController : ControllerBase
    {
        private readonly EpsilonBusDbContext _context;

        public BookingsController(EpsilonBusDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            return await _context.Bookings.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return NotFound();
            return booking;
        }

        [HttpPost]
        public async Task<ActionResult<Booking>> PostBooking(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBooking), new { id = booking.ID }, booking);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBooking(int id, Booking booking)
        {
            if (id != booking.ID)
                return BadRequest();
            _context.Entry(booking).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(id))
                    return NotFound();
                else
                    throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return NotFound();
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/bookings/single
        [HttpPost("single")]
        public async Task<ActionResult<PostBookingSingleResponse>> PostBookingSingle([FromBody] PostBookingSingleRequest request)
        {
            var result = await _context.PostBookingSingleAsync(request);
            if (result.IsSuccess == 1)
                return Ok(result);
            else
                return BadRequest(result);
        }

        // POST: api/bookings/single-cancelation
        [HttpPost("single-cancelation")]
        public async Task<ActionResult<PostSingleCancelationResponse>> PostSingleCancelation([FromBody] PostSingleCancelationRequest request)
        {
            var result = await _context.PostSingleCancelationAsync(request);
            if (result.IsSuccess == 1)
                return Ok(result);
            else
                return BadRequest(result);
        }

        // POST: api/bookings/mass-cancelation
        [HttpPost("mass-cancelation")]
        public async Task<ActionResult<PostBookingCancellationMassResponse>> PostBookingCancellationMass([FromBody] PostBookingCancellationMassRequest request)
        {
            var result = await _context.PostBookingCancellationMassAsync(request);
            if (result.IsSuccess == 1)
                return Ok(result);
            else
                return BadRequest(result);
        }

        // POST: api/bookings/mass
        [HttpPost("mass")]
        public async Task<ActionResult<PostBookingMassResponse>> PostBookingMass([FromBody] PostBookingMassRequest request)
        {
            var result = await _context.PostBookingMassAsync(request);
            if (result.IsSuccess == 1)
                return Ok(result);
            else
                return BadRequest(result);
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.ID == id);
        }
    }
}