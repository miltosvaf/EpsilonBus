using System;

namespace EpsilonBus.Api.Models
{
    public class PostBookingCancellationMassRequest
    {
        public int EmployeeID { get; set; }
        public DateTime StartDate { get; set; }
        public int LanguageID { get; set; } = 1;
    }
}