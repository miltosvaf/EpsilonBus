namespace EpsilonBus.Api.Models
{
    public class Stop
    {
        public int ID { get; set; }
        public int BranchID { get; set; }
        public string StopCode { get; set; }
        public string StopName { get; set; }
        public int? OrderNum { get; set; }
        public int IsActive { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public Branch Branch { get; set; }
    }
}