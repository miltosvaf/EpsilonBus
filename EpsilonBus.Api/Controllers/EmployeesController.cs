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
    public class EmployeesController : ControllerBase
    {
        private readonly EpsilonBusDbContext _context;

        public EmployeesController(EpsilonBusDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            return await _context.Employees.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound();
            return employee;
        }

        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEmployee), new { id = employee.ID }, employee);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, Employee employee)
        {
            if (id != employee.ID)
                return BadRequest();
            _context.Entry(employee).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                    return NotFound();
                else
                    throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound();
            _context.Employees.Remove(employee);
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

        // GET: api/employees/bycode/{employeeCode}?languageId=1
        [HttpGet("bycode/{employeeCode}")]
        public async Task<ActionResult<EmployeeDetailsDto>> GetEmployeeDetailsByCode(string employeeCode, [FromQuery] int languageId = 1)
        {
            var result = await _context.GetEmployeeDetailsByCodeAsync(employeeCode, languageId);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        // GET: api/employees/calendar?startDate=2024-01-01&endDate=2024-01-31&employeeId=1&languageId=1
        [HttpGet("calendar")]
        public async Task<ActionResult<IEnumerable<EmployeeCalendarDto>>> GetEmployeeCalendar(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] int employeeId,
            [FromQuery] int languageId = 1)
        {
            var result = await _context.GetEmployeeCalendarSPAsync(startDate, endDate, employeeId, languageId);
            return Ok(result);
        }

        // POST: api/employees/fetch
        [HttpPost("fetch")]
        public async Task<ActionResult<IEnumerable<PostEmployeeFetchResult>>> PostEmployeeFetch([FromBody] PostEmployeeFetchRequest request)
        {
            var result = await _context.PostEmployeeFetchSPAsync(request.CompanyID, request.BranchID, request.LanguageID);
            return Ok(result);
        }

        // POST: api/employees/create
        [HttpPost("create")]
        public async Task<ActionResult<PostEmployeeCreateResponse>> PostEmployeeCreate([FromBody] PostEmployeeCreateRequest request)
        {
            var result = await _context.PostEmployeeCreateAsync(request);
            if (result.IsSuccess == 1)
                return Ok(result);
            else
                return BadRequest(result);
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.ID == id);
        }
    }
}