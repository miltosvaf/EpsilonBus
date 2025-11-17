using System;

namespace EpsilonBus.Api.Models
{
    public class NonWorkingDay
    {
        public int ID { get; set; }
        public int BranchID { get; set; }
        public DateTime CalendarDate { get; set; }
        public Branch? Branch { get; set; }
    }
}