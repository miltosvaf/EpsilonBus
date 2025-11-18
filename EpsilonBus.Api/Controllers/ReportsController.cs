using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EpsilonBus.Api.Data;
using EpsilonBus.Api.Models;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly EpsilonBusDbContext _context;

    public ReportsController(EpsilonBusDbContext context)
    {
        _context = context;
    }

    [HttpGet("per-date-direction-stop")]
    public async Task<ActionResult<IEnumerable<ReportPerDateDirectionStopResult>>> GetReport(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] int branchId,
        [FromQuery] int languageId = 1)
    {
        var results = await _context.Set<ReportPerDateDirectionStopResult>()
            .FromSqlRaw(
                "EXEC [dbo].[getReportPerDateDirectionStop] @StartDate = {0}, @EndDate = {1}, @BranchID = {2}, @LanguageID = {3}",
                startDate, endDate, branchId, languageId)
            .ToListAsync();

        return Ok(results);
    }

    [HttpGet("single-date-per-direction-stop-detailed-list")]
    public async Task<ActionResult<IEnumerable<ReportSingleDatePerDirectionStopDetailedListResult>>> GetSingleDatePerDirectionStopDetailedList(
        [FromQuery] DateTime specificDate,
        [FromQuery] int branchId,
        [FromQuery] int languageId = 1)
    {
        var results = await _context.ReportSingleDatePerDirectionStopDetailedListResults
            .FromSqlRaw(
                "EXEC [dbo].[getReportSingleDatePerDirectionStopDetailedList] @SpecificDate = {0}, @BranchID = {1}, @LanguageID = {2}",
                specificDate, branchId, languageId)
            .ToListAsync();

        return Ok(results);
    }
}