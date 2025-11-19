using System;

namespace EpsilonBus.Api.Models
{
    public class PostSingleCancelationRequest
    {
        public int EmployeeID { get; set; }
        public DateTime SpecificDate { get; set; }
        public int LanguageID { get; set; } = 1;
    }
}