using System;

namespace EpsilonBus.Api.Models
{
    public class Booking
    {
        public int ID { get; set; }
        public int EmployeeID { get; set; }
        public int? OutboundStopID { get; set; }
        public int? InboundStopID { get; set; }
        public int IsDefault { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime SubmittedDate { get; set; }
        public int? UseMonday { get; set; }
        public int? UseTuesday { get; set; }
        public int? UseWednesday { get; set; }
        public int? UseThursday { get; set; }
        public int? UseFriday { get; set; }
        public int? UseSaturday { get; set; }
        public int? UseSunday { get; set; }
        public Employee Employee { get; set; }
        public Stop OutboundStop { get; set; }
        public Stop InboundStop { get; set; }
    }
}