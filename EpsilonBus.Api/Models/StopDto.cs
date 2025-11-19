namespace EpsilonBus.Api.Models
{
    // Keyless DTO for getStops SP
    public class StopDto
    {
        public int StopID { get; set; }
        public string StopCode { get; set; }
        public string StopName { get; set; }
        public int? IsActive { get; set; }
        public int? OrderNum { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}