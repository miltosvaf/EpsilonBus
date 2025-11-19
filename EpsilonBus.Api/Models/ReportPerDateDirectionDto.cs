using System;

namespace EpsilonBus.Api.Models
{
    public class ReportPerDateDirectionDto
    {
        public DateTime CalendarDate { get; set; }
        public string Direction { get; set; }
        public int EmployeeNum { get; set; }
    }
}