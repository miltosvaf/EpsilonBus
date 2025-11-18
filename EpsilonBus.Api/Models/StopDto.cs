namespace EpsilonBus.Api.Models
{
    // Keyless DTO for getStops SP
    public class StopDto
    {
        public string StopCode { get; set; }
        public string StopName { get; set; }
        public int? IsActive { get; set; }
    }
}