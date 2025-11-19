namespace EpsilonBus.Api.Models
{
    // Keyless DTO for getEmployeeDetailsByEmployeeCode SP
    public class EmployeeDetailsDto
    {
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public int CompanyID { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public int BranchID { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? OperatesMonday { get; set; }
        public int? OperatesTuesday { get; set; }
        public int? OperatesWednesday { get; set; }
        public int? OperatesThursday { get; set; }
        public int? OperatesFriday { get; set; }
        public int? OperatesSaturday { get; set; }
        public int? OperatesSunday { get; set; }
    }
}