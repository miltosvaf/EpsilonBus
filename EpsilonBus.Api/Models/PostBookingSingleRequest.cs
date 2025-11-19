using System;

namespace EpsilonBus.Api.Models
{
    public class PostBookingSingleRequest
    {
        public int EmployeeID { get; set; }
        public DateTime SpecificDate { get; set; }
        public int StopID { get; set; }
        public int Direction { get; set; }
        public int LanguageID { get; set; } = 1;
    }
}