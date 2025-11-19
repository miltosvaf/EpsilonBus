namespace EpsilonBus.Api.Models
{
    // Keyless DTO for postEmployeeFetch SP result
    public class PostEmployeeFetchResult
    {
        public int EmployeeID { get; set; } // key
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public int IsActive { get; set; }
        public string UserRole { get; set; } // newly added
    }
}