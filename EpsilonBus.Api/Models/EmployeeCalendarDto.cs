using System;

namespace EpsilonBus.Api.Models
{
    // Keyless DTO for getEmployeeCalendar SP
    public class EmployeeCalendarDto
    {
        public DateTime CalendarDate { get; set; }
        public string? OutboundStopCode { get; set; }
        public string? OutboundStopName { get; set; }
        public string? InboundStopCode { get; set; }
        public string? InboundStopName { get; set; }
    }
}