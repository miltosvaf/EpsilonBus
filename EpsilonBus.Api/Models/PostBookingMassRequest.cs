using System;

namespace EpsilonBus.Api.Models
{
    public class PostBookingMassRequest
    {
        public int EmployeeID { get; set; }
        public int Option { get; set; }
        public int StopID { get; set; }
        public int Direction { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ApplyMonday { get; set; }
        public int? ApplyTuesday { get; set; }
        public int? ApplyWednesday { get; set; }
        public int? ApplyThursday { get; set; }
        public int? ApplyFriday { get; set; }
        public int? ApplySaturday { get; set; }
        public int? ApplySunday { get; set; }
        public int LanguageID { get; set; } = 1;
    }
}