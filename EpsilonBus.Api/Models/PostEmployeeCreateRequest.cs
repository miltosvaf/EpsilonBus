namespace EpsilonBus.Api.Models
{
    public class PostEmployeeCreateRequest
    {
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string Email { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public int IsActive { get; set; }
        public int UserName { get; set; }
        public string EntraIDCode { get; set; }
        public string UserRole { get; set; }
    }
}