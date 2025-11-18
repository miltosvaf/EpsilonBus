namespace EpsilonBus.Api.Models
{
    // Keyless DTO for getStopsActiveDetails SP
    public class StopActiveDetailsDto
    {
        public int StopID { get; set; }
        public string StopCode { get; set; }
        public string StopName { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}